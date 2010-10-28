#region File Description
//-----------------------------------------------------------------------------
// AnimationPart.cs
//
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
//-----------------------------------------------------------------------------
// Bone transform data used for blending in to other animations.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace AssetData
{
    // Holds replacement frames for some bones to merge in to animation clips.
    // These parts are manually edited from an AnimationClip file by removing unwanted bones
    // and unwanted frames.
    // The frames are not intended to have a duration but are specific movements, 
    // e.g. aim up and down, look left and right by a specific angle.
    // Only include the necessary bones which will replace the movement of the animation they 
    // are merged in to.
    public class AnimationPart
    {
        // Used to try to ensure the animation was created for the same armature
        private int boneCount = 0;

        // The list of frames to replace
        // Each frame consists of a list of bones and transforms
        private IList<ReplaceBones> frames;

        // For calculations
        // These MUST be added to the header line of the part when it is manually edited from a clip
        // Can be used for angles up down or left right or something else depending on clip.
        private float max = 0;
        private float min = 0;
        // Store the position of the default frame.  Fractions can be used to blend mid way between frames
        private float restFrame = 0;

        /// <summary>
        /// Constructs a new animation part object.
        /// </summary>
        public AnimationPart(int forBoneCount, IList<ReplaceBones> replacements, float maxValue, float minValue, float defaultFrame)
        {
            boneCount = forBoneCount;
            frames = replacements;
            max = maxValue;
            min = minValue;
            restFrame = defaultFrame;
        }

        /// <summary>
        /// The number of bones that is required for this part to work.
        /// </summary>
        public int BoneCount
        {
            get { return boneCount; }
        }

        /// <summary>
        /// Gets the total number of frames in the part.
        /// </summary>
        public int TotalFrames
        {
            get { return frames.Count; }
        }

        /// <summary>
        /// Gets a combined list containing all the frames containing bones and their transformation
        /// </summary>
        public IList<ReplaceBones> Frames
        {
            get { return frames; }
        }

        // Usage is specific to the individual set of transforms
        public float Max
        {
            get { return max; }
        }
        // Usage is specific to the individual set of transforms
        public float Min
        {
            get { return min; }
        }
        // Usage is specific to the individual set of transforms
        public float RestFrame
        {
            get { return restFrame; }
        }

    }
}
