#region File Description
//-----------------------------------------------------------------------------
// SkinningData.cs
//
// Parts copyright (C) Microsoft XNA Community Game Platform
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace AssetData
{
    /// <summary>
    /// Combines all the data needed to render and animate a skinned object.
    /// This is typically stored in the Tag property of the Model being animated.
    /// This class needs a reader and writer to serialise
    /// </summary>
    public class SkinningData
    {
        private IDictionary<string, AnimationClip> animationClipsValue;
        private IList<Matrix> bindPoseValue;
        private IList<Matrix> inverseBindPoseValue;
        private IList<int> skeletonHierarchyValue;

        // To get bone names to match bone indexes
        // these are already available in the intermediate file but
        // not used in the original skinning sample
        IDictionary<string, int> boneMap;
        public IDictionary<string, int> BoneMap
        {
            get { return boneMap; }
        }

        /// <summary>
        /// Constructs a new skinning data object.
        /// </summary>
        public SkinningData(IDictionary<string, AnimationClip> animationClips,
                            IList<Matrix> bindPose, IList<Matrix> inverseBindPose,
                            IList<int> skeletonHierarchy,
                            IDictionary<string, int> boneMap)
        {
            this.boneMap = boneMap;
            animationClipsValue = animationClips;
            bindPoseValue = bindPose;
            inverseBindPoseValue = inverseBindPose;
            skeletonHierarchyValue = skeletonHierarchy;
        }


        /// <summary>
        /// Gets a collection of animation clips. These are stored by name in a
        /// dictionary, so there could for instance be clips for "Walk", "Run",
        /// "JumpReallyHigh", etc.
        /// </summary>
        public IDictionary<string, AnimationClip> AnimationClips
        {
            get { return animationClipsValue; }
        }


        /// <summary>
        /// Bindpose matrices for each bone in the skeleton,
        /// relative to the parent bone.
        /// </summary>
        public IList<Matrix> BindPose
        {
            get { return bindPoseValue; }
        }


        /// <summary>
        /// Vertex to bonespace transforms for each bone in the skeleton.
        /// </summary>
        public IList<Matrix> InverseBindPose
        {
            get { return inverseBindPoseValue; }
        }


        /// <summary>
        /// For each bone in the skeleton, stores the index of the parent bone.
        /// </summary>
        public IList<int> SkeletonHierarchy
        {
            get { return skeletonHierarchyValue; }
        }
    }
}
