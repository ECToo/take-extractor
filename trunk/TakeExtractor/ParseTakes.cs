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
    public class ParseTakes
    {
        // Main form used to display results
        private MainForm form;

        // The original data read from the file
        private List<string> source = new List<string>();

        // Load file paths
        private string fileFullPathToModel = "";
        // Save File paths
        private string pathToSaveFolder = "";

        public ParseTakes(MainForm parentForm)
        {
            form = parentForm;
        }

        /// <summary>
        /// Loads a text file into an array
        /// </summary>
        public void Load(string fileName)
        {
            string[] result = new string[0];

            if (File.Exists(fileName))
            {
                result = File.ReadAllLines(fileName);
            }
            else
            {
                form.AddMessageLine("File not found: " + fileName);
                return;
            }

            if (result == null || result.Length < 1)
            {
                form.AddMessageLine("Empty file: " + fileName);
                return;
            }

            ProcessData(result, fileName);
        }

        private void ProcessData(string[] data, string takeFullFile)
        {
            // If there is nothing do not process anything
            if (data.Length < 1)
            {
                return;
            }

            form.AddMessageLine("Processing file: " + takeFullFile);

            source.Clear();
            source.AddRange(data);

            // The first element in the data must be the model file name
            if (!ValidateModelFile(source[0], takeFullFile))
            {
                form.AddMessageLine("File not found: " + fileFullPathToModel);
                return;
            }

            ParseFBX fbx = new ParseFBX(form);
            fbx.LoadAsText(fileFullPathToModel);

            // For storing the rotations from the tkes file
            string[] items = new string[3] { "", "", "" };

            // Load the model as a model
            if (source.Count > 1)
            {
                items = ParseData.SplitItemByDivision(source[1]);
            }

            form.LoadAnimatedModel(fileFullPathToModel, items[0], items[1], items[2]);

            // Must save the takes to individual files and
            // the file names must be consistent
            fbx.SaveIndividualFBXtakes();
            // Now we can load each in turn to get the keyframe data
            ExportTakesToKeyframes(fbx, items[0], items[1], items[2]);
        }

        private struct Parts
        {
            public string partType;
            public string takeName;
            public string partName;
        }

        private void ExportTakesToKeyframes(ParseFBX fbx, string rotateXdeg, string rotateYdeg, string rotateZdeg)
        {
            string rigType = "unknown";
            List<string> headFilter = new List<string>();
            List<string> armsFilter = new List<string>();
            List<Parts> clipParts = new List<Parts>();

            // == Extract list of clips

            // For extractng the data
            string[] items;
            // Starting on the third line following the file name and the rotation
            for (int s = 2; s < source.Count; s++)
            {
                items = ParseData.SplitItemByDivision(source[s]);
                switch (items[0].ToLowerInvariant())
                {
                    case GlobalSettings.itemRigType:
                        if (items.Length > 1)
                        {
                            rigType = items[1].ToLowerInvariant();
                        }
                        break;
                    case GlobalSettings.itemHeadBones:
                        if (items.Length > 1)
                        {
                            for (int b = 1; b < items.Length; b++)
                            {
                                headFilter.Add(items[b]);
                            }
                        }
                        break;
                    case GlobalSettings.itemArmsBones:
                        if (items.Length > 1)
                        {
                            for (int b = 1; b < items.Length; b++)
                            {
                                armsFilter.Add(items[b]);
                            }
                        }
                        break;
                    case GlobalSettings.itemHeadTake:
                        if (items.Length > 2)
                        {
                            Parts part = new Parts();
                            part.partType = GlobalSettings.itemHeadTake;
                            part.takeName = items[1];
                            part.partName = items[2];
                            clipParts.Add(part);
                        }
                        break;
                    case GlobalSettings.itemArmsTake:
                        if (items.Length > 2)
                        {
                            Parts part = new Parts();
                            part.partType = GlobalSettings.itemArmsTake;
                            part.takeName = items[1];
                            part.partName = items[2];
                            clipParts.Add(part);
                        }
                        break;
                    case GlobalSettings.itemClipTake:
                        if (items.Length > 2)
                        {
                            Parts part = new Parts();
                            part.partType = GlobalSettings.itemClipTake;
                            part.takeName = items[1];
                            part.partName = items[2];
                            clipParts.Add(part);
                        }
                        break;
                }
            }

            // == Export each clip

            for (int c = 0; c < clipParts.Count; c++)
            {
                string fileName = fbx.GetTakeFileName(clipParts[c].takeName);
                form.LoadAnimatedModel(fileName, rotateXdeg, rotateYdeg, rotateZdeg);
                List<string> exportData;
                if (clipParts[c].partType == GlobalSettings.itemHeadTake)
                {
                    exportData = GetSaveClipData(form.GetModelSkinData(), clipParts[c].takeName, headFilter);
                }
                else if (clipParts[c].partType == GlobalSettings.itemArmsTake)
                {
                    exportData = GetSaveClipData(form.GetModelSkinData(), clipParts[c].takeName, armsFilter);
                }
                else
                {
                    exportData = GetSaveClipData(form.GetModelSkinData(), clipParts[c].takeName, null);
                }

                if (exportData == null || exportData.Count < 1)
                {
                    // Nothing to save
                    return;
                }
                // Save the file
                fileName = fbx.GetKeyframeFileName(rigType, clipParts[c].partName, clipParts[c].partType);
                form.AddMessageLine("Saving: " + fileName);
                File.WriteAllLines(fileName, exportData);
            }

        }

        // Convert each clip to a string array for saving
        // boneFilter is a list of bones to match that will be saved all other discarded
        // leave null or empty to select all bones
        private List<string> GetSaveClipData(SkinningData skinData, string clipName, List<string> bonesFilter)
        {
            List<string> data = new List<string>();
            // Parse and populate
            if (string.IsNullOrEmpty(clipName))
            {
                form.AddMessageLine("Animation name was blank!");
                return data;
            }

            if (skinData == null)
            {
                form.AddMessageLine("Animation skinning data is missing! " + clipName);
                return data;
            }

            AnimationClip clip = skinData.AnimationClips[clipName];
            if (clip == null)
            {
                form.AddMessageLine("Animation does not exist in the file: " + clipName);
                return data;
            }

            bool IsClip = false;
            if (bonesFilter == null || bonesFilter.Count < 1)
            {
                IsClip = true;
            }

            if (IsClip)
            {
                // CLIP
                // Add the details from the AnimationClip
                data.Add(String.Format("{0} {1}",
                    ParseData.IntToString(clip.BoneCount),
                    ParseData.TimeToString(clip.Duration)));
            }
            else
            {
                // PART, head or arms
                // Use the details from the AnimationClip to create the AnimationPart file
                // Add the extra header information for, max, min and defaultFrame
                data.Add(String.Format("{0} {1} {2} {3}",
                    ParseData.IntToString(clip.BoneCount),
                    ParseData.FloatToString(60.0f),
                    ParseData.FloatToString(-60.0f),
                    ParseData.FloatToString(((
                                (float)clip.Keyframes.Count /
                                (float)clip.BoneCount) + 1)
                                * 0.5f)));  // The middle frame if the first frame is zero
            }

            IList<Keyframe> frames = clip.Keyframes;
            if (frames != null)
            {
                data.Clear();
                form.AddMessageLine("Animation does not have any frames: " + clipName);
                return data;
            }

            // Add each keyframe
            WantedBones BoneTest = new WantedBones(skinData.BoneMap, bonesFilter);
            for (int i = 0; i < frames.Count; i++)
            {
                // FRAME
                // Include some or all of the bones to create 
                // either an AnimationClip or AnimationPart file
                if (IsClip || BoneTest.IsBoneWeWant(frames[i].Bone))
                {
                    data.Add(String.Format("{0} {1}{2}{3}",
                        ParseData.IntToString(frames[i].Bone),
                        ParseData.TimeToString(frames[i].Time),
                        ParseData.div,
                        ParseData.MatrixToString(frames[i].Transform)));
                }
            }
            return data;
        }

        // Extracts the file names from the paths
        private bool ValidateModelFile(string modelRelativeFile, string takeFullFile)
        {
            string pathToTakeFolder = Path.GetDirectoryName(takeFullFile);
            fileFullPathToModel = Path.Combine(pathToTakeFolder, modelRelativeFile);
            // For saving the animations as individual takes in my format
            pathToSaveFolder = Path.GetDirectoryName(fileFullPathToModel);
            // Check the model file exists
            if (File.Exists(fileFullPathToModel))
            {
                return true;
            }
            return false;
        }

    }
}
