#region File Description
//-----------------------------------------------------------------------------
// AnimationReaders.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
#endregion

namespace AssetData
{
    /// <summary>
    /// Loads SkinningData objects from compiled XNB format.
    /// </summary>
    public class SkinningDataReader : ContentTypeReader<SkinningData>
    {
        protected override SkinningData Read(ContentReader input,
                                             SkinningData existingInstance)
        {
            IDictionary<string, AnimationClip> animationClips;
            IList<Matrix> bindPose, inverseBindPose;
            IList<int> skeletonHierarchy;
            IDictionary<string, int> boneMap;

            animationClips = input.ReadObject<IDictionary<string, AnimationClip>>();
            bindPose = input.ReadObject<IList<Matrix>>();
            inverseBindPose = input.ReadObject<IList<Matrix>>();
            skeletonHierarchy = input.ReadObject<IList<int>>();
            boneMap = input.ReadObject<IDictionary<string, int>>();

            return new SkinningData(animationClips, bindPose,
                                    inverseBindPose, skeletonHierarchy,
                                    boneMap);

        }
    }


    /// <summary>
    /// Loads AnimationClip objects from compiled XNB format.
    /// </summary>
    public class AnimationClipReader : ContentTypeReader<AnimationClip>
    {
        protected override AnimationClip Read(ContentReader input,
                                              AnimationClip existingInstance)
        {
            int boneCount = input.ReadObject<int>();
            TimeSpan duration = input.ReadObject<TimeSpan>();
            List<TimeSpan> steps = input.ReadObject<List<TimeSpan>>();
            IList<Keyframe> keyframes = input.ReadObject < IList<Keyframe>>();

            return new AnimationClip(boneCount, duration, keyframes, steps);
        }
    }

    /// <summary>
    /// Loads AnimationPart objects from compiled XNB format.
    /// </summary>
    public class AnimationPartReader : ContentTypeReader<AnimationPart>
    {
        protected override AnimationPart Read(ContentReader input,
                                              AnimationPart existingInstance)
        {
            int count = input.ReadObject<int>();
            float max = input.ReadObject<float>();
            float min = input.ReadObject<float>();
            float rest = input.ReadObject<float>();
            IList<ReplaceBones> frames = input.ReadObject<IList<ReplaceBones>>();

            return new AnimationPart(count, frames, max, min, rest);
        }
    }

    /// <summary>
    /// Loads Keyframe objects from compiled XNB format.
    /// </summary>
    public class KeyframeReader : ContentTypeReader<Keyframe>
    {
        protected override Keyframe Read(ContentReader input,
                                         Keyframe existingInstance)
        {
            int bone = input.ReadObject<int>();
            TimeSpan time = input.ReadObject<TimeSpan>();
            Matrix transform = input.ReadObject<Matrix>();

            return new Keyframe(bone, time, transform);
        }
    }

    /*
    /// <summary>
    /// Loads XML model data from compiled XNB format.
    /// </summary>
    public class ModelDataReader : ContentTypeReader<ModelData>
    {
        protected override ModelData Read(ContentReader input,
                                         ModelData existingInstance)
        {
            ModelData modelData = new ModelData();
            modelData.PathModelAsset = input.ReadString();
            modelData.Options = input.ReadObject<List<string>>();

            return modelData;
        }
    }
     * */


}
