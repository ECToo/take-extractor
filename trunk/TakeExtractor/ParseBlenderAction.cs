#region File Description
//-----------------------------------------------------------------------------
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AssetData;
#endregion

namespace Extractor
{
    public class ParseBlenderAction
    {
        // Main form used to display results
        private MainForm form;

        // Used to store the bind pose
        Matrix[] bindTransforms;
        // The current transforms
        Matrix[] poseTransforms;
        // Each frame containing the bone and the transform
        // Just the change from the bine pose relative to the parent bone
        // this excludes the original bind pose
        IList<Keyframe> localKeyFrames = new List<Keyframe>();



        public ParseBlenderAction(MainForm parentForm)
        {
            form = parentForm;
        }

        /// <summary>
        /// Loads a text file and converts to an animation clip
        /// </summary>
        public AnimationClip Load(string fileName, Model aModel)
        {
            string[] result = new string[0];

            if (aModel == null)
            {
                form.AddMessageLine("No model is loaded!");
                return null;
            }

            SkinningData skinningData = (SkinningData)aModel.Tag;
            if (skinningData == null)
            {
                form.AddMessageLine("The current model is not compatible with animation!");
                return null;
            }

            if (File.Exists(fileName))
            {
                result = File.ReadAllLines(fileName);
            }
            else
            {
                form.AddMessageLine("File not found: " + fileName);
                return null;
            }

            if (result == null || result.Length < 1)
            {
                form.AddMessageLine("Empty file: " + fileName);
                return null;
            }

            return ProcessData(result, fileName, skinningData);
        }

        private AnimationClip ProcessData(string[] input, string fullFile, SkinningData skinningData)
        {
            // If there is nothing do not process anything
            if (input.Length < 1)
            {
                return null;
            }

            form.AddMessageLine("Processing file: " + fullFile);

            // First line of the file
            string[] data = ParseData.SplitNumbersAtSpaces(input[0]);
            int count = ParseData.IntFromString(data[0]);
            TimeSpan duration = ParseData.TimeFromString(data[1]);
            // There will be no steps in a Blender Action this is just used as a placeholder
            List<TimeSpan> steps = new List<TimeSpan>();
            // Each frame containing the bone and the transform
            // This contains the transform including the bind pose relative to the parent bone
            IList<Keyframe> poseKeyFrames = new List<Keyframe>();
            if (input.Length < 2)
            {
                form.AddMessageLine("There are no key frames in this file!");
                return null;
            }

            // To store the current pose
            bindTransforms = new Matrix[skinningData.BindPose.Count];
            poseTransforms = new Matrix[skinningData.BindPose.Count];
            // Now process add all the frames
            localKeyFrames.Clear();
            for (int i = 1; i < input.Length; i++)
            {
                string[] item = ParseData.SplitItemByDivision(input[i]);
                data = ParseData.SplitNumbersAtSpaces(item[0]);
                // The Blender Action clip exports the name of the bone not the index
                // this is to avoid accidentally having a different bone map order
                AddSortedKeyFrame(skinningData.BoneMap[data[0]],
                                    ParseData.TimeFromString(data[1]),
                                    ParseData.StringToMatrix(item[1]));
            }
            // Get the bind pose
            skinningData.BindPose.CopyTo(bindTransforms, 0);
            // Start the pose off in the bind pose
            skinningData.BindPose.CopyTo(poseTransforms, 0);
            // Add the bind pose to the local key frames
            poseKeyFrames.Clear();
            // The local key frames are already sorted in to order
            for (int k = 0; k < localKeyFrames.Count; k++)
            {
                int boneID = localKeyFrames[k].Bone;
                TimeSpan time = localKeyFrames[k].Time;
                Matrix transform = localKeyFrames[k].Transform;
                if (boneID > 0)
                {
                    int parentBone = skinningData.SkeletonHierarchy[boneID];
                    // This matrix multiplication will need to be in the right order
                    poseTransforms[boneID] = poseTransforms[parentBone] * bindTransforms[boneID] * transform;
                    poseKeyFrames.Add(new Keyframe(boneID, time, poseTransforms[boneID]));
                }
                else
                {
                    // This is the root bone
                    poseTransforms[boneID] = bindTransforms[boneID] * transform;
                    poseKeyFrames.Add(new Keyframe(boneID, time, poseTransforms[boneID]));
                }
            }
            // Create the animation clip
            return new AnimationClip(count, duration, poseKeyFrames, steps);
        }

        // Keep the localKeyFrames list sorted by frame time and bone index
        // Do this as we go by always adding frames using this method
        private void AddSortedKeyFrame(int boneID, TimeSpan time, Matrix transform)
        {
            if (localKeyFrames.Count < 1)
            {
                localKeyFrames.Add(new Keyframe(boneID, time, transform));
                return;
            }
            // They are probably already in order or nearly in order so 
            // work backwards when searching
            for (int i = localKeyFrames.Count - 1; i >= 0; i--)
            {
                if (localKeyFrames[i].Time > time)
                {
                    // Keep going back through the list
                    continue;
                }

                if (localKeyFrames[i].Time < time)
                {
                    // The new one is after the existing one so add it immediately after
                    localKeyFrames.Insert(i + 1, new Keyframe(boneID, time, transform));
                    return;
                }

                // Time must be equal so sort by bone index    
                if (localKeyFrames[i].Bone < boneID)
                {
                    // Add it after the first lower bone
                    localKeyFrames.Insert(i + 1, new Keyframe(boneID, time, transform));
                    return;
                }
            }
            // If we get this far the only place the new keyframe can fit is at the very beginning of the list
            localKeyFrames.Insert(0, new Keyframe(boneID, time, transform));

        }

    }
}
