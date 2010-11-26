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
        MainForm form;

        // Used to store the bind pose
        Matrix[] bindTransforms;
        // The current transforms
        Matrix[] poseTransforms;
        // Each frame containing the bone and the transform
        // Just the change from the bind pose relative to the parent bone
        // this excludes the original bind pose
        IList<Keyframe> localKeyFrames = new List<Keyframe>();
        
        // The next line to read from the file
        // Used to keep track of the start of the next clip
        int readLine = 0;


        public ParseBlenderAction(MainForm parentForm)
        {
            form = parentForm;
        }

        /// <summary>
        /// Loads a text file and converts to animation clips
        /// </summary>
        public Dictionary<string, AnimationClip> Load(string fileName, Model aModel, string degX, string degY, string degZ)
        {
            // Start from the beginning of the file
            readLine = 0;
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

            Matrix rotate = Matrix.Identity *
                Matrix.CreateRotationX(MathHelper.ToRadians(ParseData.IntFromString(degX))) *
                Matrix.CreateRotationY(MathHelper.ToRadians(ParseData.IntFromString(degY))) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(ParseData.IntFromString(degZ)));

            return ProcessData(result, fileName, skinningData, rotate);
        }

        private Dictionary<string, AnimationClip> ProcessData(string[] input, string fullFile, SkinningData skinningData, Matrix rotation)
        {
            // If there is nothing do not process anything
            if (input.Length < 1)
            {
                return null;
            }

            Dictionary<string, AnimationClip> resultClips = new Dictionary<string,AnimationClip>();

            form.AddMessageLine("Reading file: " + fullFile);

            while (readLine < input.Length)
            {
                // First line of each clip should contain the clip name
                // in the format "=ClipName"
                string clipName = input[readLine];
                if (clipName.Length > 1 && clipName.Substring(0, 1) == "=")
                {
                    // Remove the "=" sign
                    clipName = clipName.Substring(1);
                    readLine++;
                }
                else
                {
                    // Unique clip name based on the current date and time
                    clipName = DateTime.Now.ToString(GlobalSettings.timeFormat);
                    readLine++;
                }
                // Create the animation clip
                form.AddMessageLine("Processing action: " + clipName);
                AnimationClip thisClip = ProcessOneClip(input, skinningData, rotation);
                if (thisClip != null)
                {
                    resultClips.Add(clipName, thisClip);
                }
            }
            return resultClips;
        }

        // The input only contains the local bone transform
        // This processor adds the bind pose
        private AnimationClip ProcessOneClip(string[] input, SkinningData skinningData, Matrix rotation)
        {
            // This should start at the first line FOLLOWING the clip name
            string[] data = ParseData.SplitNumbersAtSpaces(input[readLine]);
            readLine++;
            // This contains the number of bones in the animation...
            int count = ParseData.IntFromString(data[0]);
            // and the duration of the animation.
            TimeSpan duration = ParseData.TimeFromString(data[1]);
            // There will be no steps in a Blender Action this is just used as a placeholder
            List<TimeSpan> steps = new List<TimeSpan>();
            // Each frame containing the bone and the transform
            // This contains the transform including the bind pose relative to the parent bone
            IList<Keyframe> poseKeyFrames = new List<Keyframe>();
            if (readLine >= input.Length)
            {
                form.AddMessageLine("There are no key frames in this file!");
                return null;
            }

            // To store the current pose
            bindTransforms = new Matrix[skinningData.BindPose.Count];
            poseTransforms = new Matrix[skinningData.BindPose.Count];
            // Now process add all the frames
            localKeyFrames.Clear();
            // Start from the line following the header information
            while (readLine < input.Length)
            {
                // If the line starts with an "=" it is the next clip
                if (input[readLine].Substring(0, 1) == "=")
                {
                    break;
                }
                string[] item = ParseData.SplitItemByDivision(input[readLine]);
                data = ParseData.SplitNumbersAtSpaces(item[0]);
                // The Blender Action clip exports the name of the bone not the index
                // this is to avoid accidentally having a different bone map order
                AddSortedKeyFrame(skinningData.BoneMap[data[0]],
                                    ParseData.TimeFromString(data[1]),
                                    ParseData.StringToMatrix(item[1]));
                readLine++;
            }
            if (localKeyFrames.Count < 1)
            {
                return null;
            }

            // Rotate
            /*
            for (int r = localKeyFrames.Count - 1; r >= 0; r--)
            {
                Matrix changed = Matrix.Identity;
                if (localKeyFrames[r].Bone > 0)
                {
                    int parentBone = skinningData.SkeletonHierarchy[localKeyFrames[r].Bone];
                    Matrix parentInvert = Matrix.Invert(localKeyFrames[parentBone].Transform);
                    // Move to parent space
                    changed = localKeyFrames[r].Transform * parentInvert;
                    changed = rotation * changed;
                }
                else
                {
                    // Root
                    changed = rotation * localKeyFrames[r].Transform;
                }
                localKeyFrames[r] = new Keyframe(localKeyFrames[r].Bone, localKeyFrames[r].Time, changed);
            }
            */

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
                poseTransforms[boneID] = transform * bindTransforms[boneID];
                /*
                if (boneID == 0)
                {
                    // Root bone
                    poseTransforms[boneID] = transform * bindTransforms[boneID];
                }
                else
                {
                    int parentBone = skinningData.SkeletonHierarchy[boneID];
                    Matrix parentInvert = Matrix.Invert(rotation * bindTransforms[parentBone]);
                    // (rotation * parent[frame]).Invert * (rotation * self[frame])
                    // Tried
                    // Matrix parentInvert = Matrix.Invert(rotation * bindTransforms[parentBone]);
                    // (parentInvert * (rotation * transform)) * bindTransforms[boneID];
                    //poseTransforms[boneID] = transform * bindTransforms[boneID];
                    poseTransforms[boneID] = (parentInvert * (rotation * transform)) * bindTransforms[boneID];
                }
                 * */
                poseKeyFrames.Add(new Keyframe(boneID, time, poseTransforms[boneID]));
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
