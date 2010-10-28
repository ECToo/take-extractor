#region File Description
//-----------------------------------------------------------------------------
// Keyframe.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace AssetData
{
    // Describes the position of a single bone at a single point in time.
    public class Keyframe
    {
        int boneValue;
        TimeSpan timeValue;
        Matrix transformValue;

        /// <summary>
        /// Constructs a new keyframe object.
        /// </summary>
        public Keyframe(int bone, TimeSpan time, Matrix transform)
        {
            boneValue = bone;
            timeValue = time;
            transformValue = transform;
        }

        /// <summary>
        /// Gets the index of the target bone that is animated by this keyframe.
        /// </summary>
        public int Bone
        {
            get { return boneValue; }
        }


        /// <summary>
        /// Gets the time offset from the start of the animation to this keyframe.
        /// </summary>
        public TimeSpan Time
        {
            get { return timeValue; }
        }


        /// <summary>
        /// Gets the bone transform for this keyframe.
        /// </summary>
        public Matrix Transform
        {
            get { return transformValue; }
        }
    }
}
