#region File Description
//-----------------------------------------------------------------------------
// ModelViewerControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AssetData;
#endregion

namespace Extractor
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, and displays
    /// a spinning 3D model. The main form class is responsible for loading
    /// the model: this control just displays it.
    /// </summary>
    class ModelViewerControl : GraphicsDeviceControl
    {
        /// <summary>
        /// Gets or sets the current model.
        /// </summary>
        public Model Model
        {
            get { return model; }
        }

        Model model;
        AnimationPlayer animationPlayer;
        // Animations
        public List<string> ClipNames
        {
            get { return clipNames; }
        }
        List<string> clipNames = new List<string>();

        public bool IsAnimated
        {
            get { return isAnimated; }
            set { isAnimated = value; }
        }

        bool isAnimated = false;

        /// <summary>
        /// 1 = Y Up
        /// 2 = Z Up
        /// 3 = Z Down
        /// </summary>
        public int ViewUp
        {
            get { return viewUp; }
            set { viewUp = value; }
        }

        int viewUp = 1;

        // Used every frame
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.Identity;

        // Cache information about the model size and position.
        Matrix[] boneTransforms;
        Vector3 modelCenter;
        float modelRadius;


        // Timer controls the rotation speed.
        Stopwatch timer;
        // Keep track of elapsed time
        TimeSpan previousTime;
        TimeSpan currentTime;
        TimeSpan elapsedGameTime;

        /// <summary>
        /// Set the model and return any error messages
        /// </summary>
        public string SetModel(bool animated, Model aModel)
        {
            string errors = "";
            isAnimated = animated;
            if (aModel != null)
            {
                model = aModel;
                MeasureModel();
            }

            if (isAnimated)
            {
                // Look up our custom skinning information.
                SkinningData skinningData = model.Tag as SkinningData;

                if (skinningData == null)
                {
                    errors += "\nThis model does not contain a SkinningData tag.";
                    isAnimated = false;
                }
                // Chack again to make sure it is still treated as animated
                if (isAnimated)
                {
                    // Create an animation player, and start decoding an animation clip.
                    animationPlayer = new AnimationPlayer(skinningData);
                    clipNames.AddRange(skinningData.AnimationClips.Keys);

                    if (clipNames.Count > 0)
                    {
                        //AnimationClip clip = skinningData.AnimationClips["Take 001"];
                        AnimationClip clip = skinningData.AnimationClips[clipNames[0]];
                        animationPlayer.StartClip(clip);
                    }
                }

            }
            return errors;
        }

        public void SetClipName(string name)
        {
            if (isAnimated && model != null && animationPlayer != null)
            {
                // Look up our custom skinning information.
                SkinningData skinningData = model.Tag as SkinningData;
                // Make sure the animation exists in the model
                if (skinningData == null ||
                    !skinningData.AnimationClips.ContainsKey(name))
                {
                    return;
                }
                // Change the animation
                AnimationClip clip = skinningData.AnimationClips[name];
                animationPlayer.StartClip(clip);
            }
        }

        // returns any error message
        public string SetExternalClip(AnimationClip clip)
        {
            if (clip != null && isAnimated && model != null && animationPlayer != null)
            {
                // Change the animation
                return animationPlayer.StartClip(clip);
            }
            return "The model or the clip is null or not animated!";
        }

        public void UnloadModel()
        {
            isAnimated = false;
            clipNames.Clear();
            animationPlayer = null;
            model = null;
        }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            // Start the animation timer.
            timer = Stopwatch.StartNew();
            currentTime = timer.Elapsed;
            previousTime = currentTime;

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
        }

        /// <summary>
        /// Simulated update called prior to the draw method
        /// </summary>
        protected override void GameUpdate()
        {
            currentTime = timer.Elapsed;
            elapsedGameTime = currentTime - previousTime;
            previousTime = currentTime;

            if (isAnimated && model != null && animationPlayer != null)
            {
                animationPlayer.Update(elapsedGameTime, true, Matrix.Identity);
            }

        }


        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            if (model != null)
            {
                float aspectRatio = GraphicsDevice.Viewport.AspectRatio;
                
                float nearClip = modelRadius / 100;
                float farClip = modelRadius * 100;

                // Compute camera matrices.
                float rotation = (float)timer.Elapsed.TotalSeconds;

                Vector3 eyePosition = modelCenter;

                // Change which way up the model is viewed
                if (viewUp == 3)
                {
                    // Z Down
                    eyePosition.Y += modelRadius * 2;
                    eyePosition.Z += modelRadius;
                    world = Matrix.CreateRotationZ(rotation);
                    view = Matrix.CreateLookAt(eyePosition, modelCenter, Vector3.Forward);
                }
                else if (viewUp == 2)
                {
                    // Z Up (Blender default)
                    eyePosition.Y += modelRadius * 2;
                    eyePosition.Z += modelRadius;
                    world = Matrix.CreateRotationZ(rotation);
                    view = Matrix.CreateLookAt(eyePosition, modelCenter, Vector3.Backward);
                }
                else
                {
                    // XNA Default
                    eyePosition.Z += modelRadius * 2;
                    eyePosition.Y += modelRadius;
                    world = Matrix.CreateRotationY(rotation);
                    view = Matrix.CreateLookAt(eyePosition, modelCenter, Vector3.Up);
                }
                projection = Matrix.CreatePerspectiveFieldOfView(1, aspectRatio, nearClip, farClip);

                // Set states ready for 3D
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                if (isAnimated)
                {
                    DrawAnimated(world, view, projection);
                }
                else
                {
                    DrawRigid(world, view, projection);
                }
            }
        }

        private void DrawAnimated(Matrix world, Matrix view, Matrix projection)
        {
            Matrix[] bones = animationPlayer.GetSkinTransforms();

            // Render the skinned mesh.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    // Added to move model
                    effect.World = boneTransforms[mesh.ParentBone.Index] * world;

                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
        }

        private void DrawRigid(Matrix world, Matrix view, Matrix projection)
        {
            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                    effect.View = view;
                    effect.Projection = projection;

                    // Add a bit more light to our animated models
                    //effect.EmissiveColor = new Vector3(0.8f, 0.8f, 0.8f);
                    //effect.LightingEnabled = true;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
        }


        /// <summary>
        /// Whenever a new model is selected, we examine it to see how big
        /// it is and where it is centered. This lets us automatically zoom
        /// the display, so we can correctly handle models of any scale.
        /// </summary>
        void MeasureModel()
        {
            // Look up the absolute bone transforms for this model.
            boneTransforms = new Matrix[model.Bones.Count];
            
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Compute an (approximate) model center position by
            // averaging the center of each mesh bounding sphere.
            modelCenter = Vector3.Zero;

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                modelCenter += meshCenter;
            }

            modelCenter /= model.Meshes.Count;

            // Now we know the center point, we can compute the model radius
            // by examining the radius of each mesh bounding sphere.
            modelRadius = 0;

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                float transformScale = transform.Forward.Length();
                
                float meshRadius = (meshCenter - modelCenter).Length() +
                                   (meshBounds.Radius * transformScale);

                modelRadius = Math.Max(modelRadius,  meshRadius);
            }
        }
    }
}
