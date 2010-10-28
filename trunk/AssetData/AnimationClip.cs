#region File Description
//-----------------------------------------------------------------------------
// AnimationClip.cs
//
// Modified by: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
//
// Based on the examples provided by the Microsoft XNA Community Game Platform
//-----------------------------------------------------------------------------
// The bone transform data used for animation.
// Modified to allow for a central set of animation clips instead of unique ones
// for each model.
// Includes the bone count to compare to the number of bones in 
// the calling model so only suitable clips are used.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace AssetData
{
    // An animation clip is the runtime equivalent of the
    // Microsoft.Xna.Framework.Content.Pipeline.Graphics.AnimationContent type.
    // It holds all the keyframes needed to describe a single animation.
    // This class needs a reader and writer to serialise
    public class AnimationClip
    {
        // Used to try to ensure the animation was created for the same armature
        private int boneCount = 0;
        // For foot steps
        private List<TimeSpan> soundFrameTimes;
        private int currentSoundFrameID = 0;

        private TimeSpan durationValue;
        private IList<Keyframe> keyframesValue;

        public AnimationClip(int forBoneCount, TimeSpan duration, IList<Keyframe> keyframes, List<TimeSpan> stepFrames)
        {
            boneCount = forBoneCount;
            durationValue = duration;
            keyframesValue = keyframes;
            soundFrameTimes = stepFrames;
        }

        // The number of bones that is required for this clip to work.
        public int BoneCount
        {
            get { return boneCount; }
        }

        // Gets the total length of the animation.
        public TimeSpan Duration
        {
            get { return durationValue; }
        }

        // Gets a combined list containing all the keyframes for all bones,
        // sorted by time.
        public IList<Keyframe> Keyframes
        {
            get { return keyframesValue; }
        }

        // List of frames to play sound at
        // This must have been created in order to work as intended
        public List<TimeSpan> SoundFrameTimes
        {
            get { return soundFrameTimes; }
        }

        public void ResetSoundFrame()
        {
            currentSoundFrameID = 0;
        }

        public void IncrementSoundFrame()
        {
            currentSoundFrameID++;
            // DO NOT loop round otherwise the sound might play again because the animation
            // is further ahead
        }

        public TimeSpan NextSoundFrameTime()
        {
            if (soundFrameTimes.Count < 1 || currentSoundFrameID >= soundFrameTimes.Count)
            {
                // Sound is played when the animated frame time reaches the value returned
                // by returning a very high value no sound is ever played.
                return TimeSpan.MaxValue;
            }
            return soundFrameTimes[currentSoundFrameID];
        }

    }
}
