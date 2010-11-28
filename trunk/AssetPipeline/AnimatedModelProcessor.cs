#region File Description
//-----------------------------------------------------------------------------
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
// Modified from the samples provided by
// Microsoft XNA Community Game Platform
//-----------------------------------------------------------------------------
// 
// This only runs at compile time to produce the intermediate model files
// Therefore it is the same code for both the Xbox360 and Windows
//
// This has been modified to change exceptions to errors for use with 
// the model viewer.
//
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using AssetData;
#endregion

namespace AssetPipeline
{
    /// <summary>
    /// Custom processor extends the built in framework ModelProcessor class,
    /// adding animation support.
    /// </summary>
    [ContentProcessor]
    public class AnimatedModelProcessor : ModelProcessor
    {
        // The maximum number of bone matrices we can render using shader 2.0
        // in a single pass is 59. If you change this, update AnimatedModel.fx to match.
        // My human model uses 52 for the main skeleton.
        // After changes to the shader to add shadows I had to reduce 
        // the maximum to 52 for the Xbox 360 however the PC managed 56.
        private const int MaxBones = 52;
        // The built in ModelProcessor fails to rotate animations correctly.
        // These are added to prevent that by stopping rotations on this processor.
        private float degreesX;
        public float DegreesX
        {
            get { return degreesX; }
            set { degreesX = value; }
        }

        private float degreesY;
        public float DegreesY
        {
            get { return degreesY; }
            set { degreesY = value; }
        }

        private float degreesZ;
        public float DegreesZ
        {
            get { return degreesZ; }
            set { degreesZ = value; }
        }

        public override float RotationX
        {
            get
            {
                return base.RotationX;
            }
            set
            {
                DegreesX = value;
                base.RotationX = 0;
            }
        }

        public override float RotationY
        {
            get
            {
                return base.RotationY;
            }
            set
            {
                DegreesY = value;
                base.RotationY = 0;
            }
        }

        public override float RotationZ
        {
            get
            {
                return base.RotationZ;
            }
            set
            {
                DegreesZ = value;
                base.RotationZ = 0;
            }
        }

        /// <summary>
        /// The main Process method converts an intermediate format content pipeline
        /// NodeContent tree to a ModelContent object with embedded animation data.
        /// </summary>
        public override ModelContent Process(NodeContent input,
                                             ContentProcessorContext context)
        {
            // Before anything rotate the entire model and animations
            // Remember that the keyframes are separated so the animations
            // have to be extracted from a model rotated the same way as the 
            // recipient of those animations.
            RotateAll(input, DegreesX, DegreesY, DegreesZ);

            if (ValidateMesh(input, context, null))
            {
                // Find the skeleton.
                BoneContent skeleton = MeshHelper.FindSkeleton(input);

                if (skeleton == null)
                {
                    // this is a normal model not an animated one
                    // so do the base processing instead
                    context.Logger.LogWarning(null, null, "No skeleton found!");
                    // Not an animated model
                    return null;
                    //throw new InvalidContentException("Input skeleton not found.");
                }

                // We don't want to have to worry about different parts of the model being
                // in different local coordinate systems, so let's just bake everything.
                FlattenTransforms(input, skeleton);

                // Read the bind pose and skeleton hierarchy data.
                IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);

                if (bones.Count > MaxBones)
                {
                    //throw new InvalidContentException(string.Format(
                    //  "Skeleton has {0} bones, but the maximum supported is {1}.",
                    //bones.Count, MaxBones));
                    context.Logger.LogWarning(null, null,
                        string.Format(
                        "Skeleton has {0} bones.  The maximum supported with shadows is {1}.",
                        bones.Count, MaxBones));
                    // If we needed the animations to work we should stop here but for the 
                    // purposes of the viewer we can continue.
                }

                List<Matrix> bindPose = new List<Matrix>();
                List<Matrix> inverseBindPose = new List<Matrix>();
                List<int> skeletonHierarchy = new List<int>();

                foreach (BoneContent bone in bones)
                {
                    bindPose.Add(bone.Transform);
                    inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                    skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
                }

                // Build up a table mapping bone names to indices. JCB
                IDictionary<string, int> boneMap = new Dictionary<string, int>();

                // Convert animation data to our runtime format.
                Dictionary<string, AnimationClip> animationClips;
                animationClips = ProcessAnimations(skeleton.Animations, bones, boneMap, context);    // added bonemap

                // Catch any build errors!
                try
                {
                    // Chain to the base ModelProcessor class so it can convert the model data.
                    ModelContent model = base.Process(input, context);

                    // Store our custom animation data in the Tag property of the model.
                    model.Tag = new SkinningData(animationClips, bindPose,
                                                    inverseBindPose, skeletonHierarchy,
                                                    boneMap);
                    return model;
                }
                catch (Exception e)
                {
                    context.Logger.LogWarning(null, null, e.ToString());
                    return null;
                }
            }
            // Not an animated model
            return null;
        }

        // Rotate all the content before anything else
        // see http://forums.xna.com/forums/p/60188/370817.aspx#370817
        // http://forums.create.msdn.com/forums/p/60188/370817.aspx
        // http://forums.create.msdn.com/forums/p/64690/395491.aspx#395491
        /*
         * By Shawn Hargreaves
         * If you look at the skinned model processor, you will see that it pulls out animation data from 
         * the model into its own keyframe data structures, then chains to the base ModelProcessor which 
         * converts the model itself from NodeContent into ModelContent format.
         * This base ModelProcessor call applies whatever rotation has been specified via these processor 
         * parameters, but this happens AFTER the keyframe data was extracted, so the keyframe values are 
         * not rotated.
         * There are several ways you could fix this:
         * Manually apply the necessary rotation to each keyframe matrix
         * Or, instead of using ModelProcessor to apply the rotation, do this yourself at the very start of 
         * your Process method (before you call ModelProcessor and before any of the keyframe extraction). 
         * The easiest way to do that is to call MeshHelper.TransformScene. 
         * */
        // This only works if the animation keyframes are also rotated
        // As my animations are separate the source model would need to be rotated first.
        public static void RotateAll(NodeContent node, float degX, float degY, float degZ)
        {
            Matrix rotate = Matrix.Identity *
                Matrix.CreateRotationX(MathHelper.ToRadians(degX)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(degY)) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(degZ));
            // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.content.pipeline.graphics.meshhelper.transformscene.aspx
            MeshHelper.TransformScene(node, rotate);
        }

        /// <summary>
        /// Converts an intermediate format content pipeline AnimationContentDictionary
        /// object to our runtime AnimationClip format.
        /// </summary>
        public static Dictionary<string, AnimationClip> ProcessAnimations(
            AnimationContentDictionary animations, IList<BoneContent> bones,
            IDictionary<string,int> boneMap, ContentProcessorContext context)
        {
            // Build up a table mapping bone names to indices.
            // Dictionary<string, int> boneMap
            for (int i = 0; i < bones.Count; i++)
            {
                string boneName = bones[i].Name;

                if (!string.IsNullOrEmpty(boneName))
                    boneMap.Add(boneName, i);
            }

            // Convert each animation in turn.
            Dictionary<string, AnimationClip> animationClips;
            animationClips = new Dictionary<string, AnimationClip>();

            foreach (KeyValuePair<string, AnimationContent> animation in animations)
            {
                AnimationClip processed = ProcessAnimation(animation.Value, boneMap, context);
                
                animationClips.Add(animation.Key, processed);
            }

            if (animationClips.Count == 0)
            {
                //throw new InvalidContentException(
                  //          "Input file does not contain any animations.");
                context.Logger.LogWarning(null, null,
                        "Input file does not contain any animations.");
            }

            return animationClips;
        }


        /// <summary>
        /// Converts an intermediate format content pipeline AnimationContent
        /// object to our runtime AnimationClip format.
        /// </summary>
        public static AnimationClip ProcessAnimation(AnimationContent animation,
                                              IDictionary<string, int> boneMap, 
                                              ContentProcessorContext context)
        {
            List<Keyframe> keyframes = new List<Keyframe>();

            // For each input animation channel.
            foreach (KeyValuePair<string, AnimationChannel> channel in
                animation.Channels)
            {
                // Look up what bone this channel is controlling.
                int boneIndex;

                if (!boneMap.TryGetValue(channel.Key, out boneIndex))
                {
                    //throw new InvalidContentException(string.Format(
                      //  "Found animation for bone '{0}', " +
                        //"which is not part of the skeleton.", channel.Key));
                    context.Logger.LogWarning(null, null,
                        string.Format("Found animation for bone '{0}', " +
                        "which is not part of the skeleton.", channel.Key));
                    // Skip this channel
                    continue;
                }

                // Convert the keyframe data.
                foreach (AnimationKeyframe keyframe in channel.Value)
                {
                    keyframes.Add(new Keyframe(boneIndex, keyframe.Time,
                                               keyframe.Transform));
                }
            }

            // Sort the merged keyframes by time.
            keyframes.Sort(CompareKeyframeTimes);

            if (keyframes.Count == 0)
            {
                //throw new InvalidContentException("Animation has no keyframes.");
                context.Logger.LogWarning(null, null, "Animation has no keyframes.");
                return null;
            }

            if (animation.Duration <= TimeSpan.Zero)
            {
                //throw new InvalidContentException("Animation has a zero duration.");
                context.Logger.LogWarning(null, null, "Animation has a zero duration.");
                return null;
            }

            // A model from a 3D modelling package does not have sound cues
            // We have to add them manually to our modified clip file
            List<TimeSpan> dummy = new List<TimeSpan>();

            return new AnimationClip(boneMap.Count, animation.Duration, keyframes, dummy);
        }


        /// <summary>
        /// Comparison function for sorting keyframes into ascending time order.
        /// </summary>
        public static int CompareKeyframeTimes(Keyframe a, Keyframe b)
        {
            return a.Time.CompareTo(b.Time);
        }


        /// <summary>
        /// Makes sure this mesh contains the kind of data we know how to animate.
        /// </summary>
        public static bool ValidateMesh(NodeContent node, ContentProcessorContext context,
                                 string parentBoneName)
        {
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // Validate the mesh.
                if (parentBoneName != null)
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} is a child of bone {1}. SkinnedModelProcessor " +
                        "does not correctly handle meshes that are children of bones.",
                        mesh.Name, parentBoneName);
                }

                // This test only works for animated models
                
                if (!MeshHasSkinning(mesh))
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} has no skinning information", // so it has been deleted.",
                        mesh.Name);

                    // This does not work for non-animated models
                    mesh.Parent.Children.Remove(mesh);
                    return false;
                }
                
            }
            else if (node is BoneContent)
            {
                // If this is a bone, remember that we are now looking inside it.
                parentBoneName = node.Name;
            }

            // Recurse (iterating over a copy of the child collection,
            // because validating children may delete some of them).
            foreach (NodeContent child in new List<NodeContent>(node.Children))
            {
                if (!ValidateMesh(child, context, parentBoneName))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Checks whether a mesh contains skininng information.
        /// </summary>
        public static bool MeshHasSkinning(MeshContent mesh)
        {
            foreach (GeometryContent geometry in mesh.Geometry)
            {
                if (!geometry.Vertices.Channels.Contains(VertexChannelNames.Weights()))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Bakes unwanted transforms into the model geometry,
        /// so everything ends up in the same coordinate system.
        /// </summary>
        public static void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            foreach (NodeContent child in node.Children)
            {
                // Don't process the skeleton, because that is special.
                if (child == skeleton)
                    continue;

                // Bake the local transform into the actual geometry.
                MeshHelper.TransformScene(child, child.Transform);

                // Having baked it, we can now set the local
                // coordinate system back to identity.
                child.Transform = Matrix.Identity;

                // Recurse.
                FlattenTransforms(child, skeleton);
            }
        }

        // == for animations
        /// <summary>
        /// Force all the materials to use our skinned model effect.
        /// </summary>
        [DefaultValue(MaterialProcessorDefaultEffect.SkinnedEffect)]
        public override MaterialProcessorDefaultEffect DefaultEffect
        {
            get { return MaterialProcessorDefaultEffect.SkinnedEffect; }
            set { }
        }

        // At the moment we are not displaying any animations so we can keep basic effect
        /*
        /// <summary>
        /// Changes all the materials to use our model effect.
        /// </summary>
        protected override MaterialContent ConvertMaterial(MaterialContent material,
                                                        ContentProcessorContext context)
        {
            BasicMaterialContent basicMaterial = material as BasicMaterialContent;

            if (basicMaterial == null)
            {
                throw new InvalidContentException(string.Format(
                    "AnimatedModelProcessor only supports BasicMaterialContent, " +
                    "but input mesh uses {0}.", material.GetType()));
            }

            EffectMaterialContent effectMaterial = new EffectMaterialContent();

            // Store a reference to our skinned mesh effect.
            string effectPath = Path.GetFullPath("../Content/Effects/Animated.fx");

            effectMaterial.Effect = new ExternalReference<EffectContent>(effectPath);

            // Copy texture settings from the input
            // BasicMaterialContent over to our new material.
            // Typically the Texture is named "Texture" in the model FBX file
            // so the parameter in the effect file should be named the same.
            if (basicMaterial.Texture != null)
                effectMaterial.Textures.Add("Texture", basicMaterial.Texture);

            // Chain to the base ModelProcessor converter.
            return base.ConvertMaterial(effectMaterial, context);
        }
         * */
    }
}
