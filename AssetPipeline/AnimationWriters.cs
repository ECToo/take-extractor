#region File Description
//-----------------------------------------------------------------------------
// AnimationWriters.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
// 
// This only runs at compile time to produce the intermediate model files
// Therefore it is the same code for both the Xbox360 and Windows
//
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using AssetData;
#endregion

namespace AssetPipeline
{
    /// <summary>
    /// Writes ModelAnimation objects into compiled XNB format.
    /// </summary>
    [ContentTypeWriter]
    public class SkinningDataWriter : ContentTypeWriter<SkinningData>
    {
        protected override void Write(ContentWriter output, SkinningData value)
        {
            output.WriteObject(value.AnimationClips);
            output.WriteObject(value.BindPose);
            output.WriteObject(value.InverseBindPose);
            output.WriteObject(value.SkeletonHierarchy);
            output.WriteObject(value.BoneMap);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(SkinningDataReader).AssemblyQualifiedName;
        }
    }


    /// <summary>
    /// Writes AnimationClip objects into compiled XNB format
    /// </summary>
    [ContentTypeWriter]
    public class AnimationClipWriter : ContentTypeWriter<AnimationClip>
    {
        protected override void Write(ContentWriter output, AnimationClip value)
        {
            output.WriteObject(value.BoneCount);
            output.WriteObject(value.Duration);
            output.WriteObject(value.SoundFrameTimes);
            output.WriteObject(value.Keyframes);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AnimationClipReader).AssemblyQualifiedName;
        }
    }

    /// <summary>
    /// Writes AnimationPart objects into compiled XNB format
    /// </summary>
    [ContentTypeWriter]
    public class AnimationPartWriter : ContentTypeWriter<AnimationPart>
    {
        protected override void Write(ContentWriter output, AnimationPart value)
        {
            output.WriteObject(value.BoneCount);
            output.WriteObject(value.Max);
            output.WriteObject(value.Min);
            output.WriteObject(value.RestFrame);
            output.WriteObject(value.Frames);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AnimationPartReader).AssemblyQualifiedName;
        }
    }

    /// <summary>
    /// Writes Keyframe objects into compiled XNB format
    /// </summary>
    [ContentTypeWriter]
    public class KeyframeWriter : ContentTypeWriter<Keyframe>
    {
        protected override void Write(ContentWriter output, Keyframe value)
        {
            output.WriteObject(value.Bone);
            output.WriteObject(value.Time);
            output.WriteObject(value.Transform);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(KeyframeReader).AssemblyQualifiedName;
        }
    }

    /*
    /// <summary>
    /// Writes the XML ModelData into compiled XNB format
    /// </summary>
    [ContentTypeWriter]
    public class ModelDataWriter : ContentTypeWriter<ModelData>
    {
        protected override void Write(ContentWriter output, ModelData value)
        {
            output.Write(value.PathModelAsset);
            output.WriteObject(value.Options);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ModelDataReader).AssemblyQualifiedName;
        }
    }
     * */

}
