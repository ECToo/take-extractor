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

            // The second element in the data must be the model file name
            // Extract the path and file name
            if (!ExtractPathNames(source[1], takeFullFile))
            {
                form.AddMessageLine("File not found: " + fileFullPathToModel);
                return;
            }
            // Get the rotation
            // For storing the rotations from the takes file
            string[] items = new string[3] { "", "", "" };
            if (source.Count > 2)
            {
                // Get the rotation
                items = ParseData.SplitItemByDivision(source[2]);
            }

            // Decide what file type we have
            int formatType = ParseData.IntFromString(source[0]);
            switch (formatType)
            {
                case 1:
                    ProcessTypeOne(items[0], items[1], items[2]);
                    break;
                case 2:
                    ProcessTypeTwo(items[0], items[1], items[2]);
                    break;
                default:
                    // Error
                    form.AddMessageLine("Unrecognised file format type: " + formatType);
                    return;
            }
        }

        private void ProcessTypeOne(string rotateXdeg, string rotateYdeg, string rotateZdeg)
        {
            ParseFBX fbx = new ParseFBX(form);
            // Load the model text for splitting to separate files
            fbx.LoadAsText(fileFullPathToModel);
            // Load the model as a model
            form.LoadAnimatedModel(true, fileFullPathToModel, rotateXdeg, rotateYdeg, rotateZdeg);
            // Must save the takes to individual files and
            // the file names must be consistent
            fbx.SaveIndividualFBXtakes();
            // Now we can load each in turn to get the keyframe data
            ExportTakesToKeyframes(1, fbx, rotateXdeg, rotateYdeg, rotateZdeg);
        }

        private void ProcessTypeTwo(string rotateXdeg, string rotateYdeg, string rotateZdeg)
        {
            ParseFBX fbx = new ParseFBX(form);
            // Just get the file names and paths
            fbx.ExtractFileNames(fileFullPathToModel);
            // Load the model as a model
            form.LoadAnimatedModel(true, fileFullPathToModel, rotateXdeg, rotateYdeg, rotateZdeg);
            // Animation files (FBX) must already exist
            // Now we can load each in turn to get the keyframe data
            ExportTakesToKeyframes(2, fbx, rotateXdeg, rotateYdeg, rotateZdeg);
        }


        private struct Parts
        {
            public string partType;
            public string takeName;
            public string partName;
        }

        private void ExportTakesToKeyframes(int formatType, ParseFBX fbx, string rotateXdeg, string rotateYdeg, string rotateZdeg)
        {
            string rigType = "unknown";
            List<string> headFilter = new List<string>();
            List<string> armsFilter = new List<string>();
            List<Parts> clipParts = new List<Parts>();

            // == Extract list of clips

            // For extractng the data
            string[] items;
            // Starting on the fourth line following the file name and the rotation
            for (int s = 3; s < source.Count; s++)
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
                // Get the filename to load each take from
                string fileName = "";
                if (formatType == 1)
                {
                    // In type 1 only the animation (action, take) name is included in the config file
                    fileName = fbx.GetTakeFileName(clipParts[c].takeName);
                }
                else if (formatType == 2)
                {
                    // In type 2 the file including extension that contains the 
                    // animation is included in the config file
                    fileName = fbx.GetFullPath(clipParts[c].takeName);
                }
                else
                {
                    // Error but we should never get this far anyway
                    return;
                }
                // Add each animation in to the form
                form.LoadAnimationTakes(fileName, rotateXdeg, rotateYdeg, rotateZdeg);
                // The animation loaded must have been selected as the current animation for this to work
                List<string> exportData;
                if (clipParts[c].partType == GlobalSettings.itemHeadTake)
                {
                    exportData = GetSaveClipData(form.GetCurrentClip(), form.GetBoneMap(), clipParts[c].takeName, headFilter);
                }
                else if (clipParts[c].partType == GlobalSettings.itemArmsTake)
                {
                    exportData = GetSaveClipData(form.GetCurrentClip(), form.GetBoneMap(), clipParts[c].takeName, armsFilter);
                }
                else
                {
                    exportData = GetSaveClipData(form.GetCurrentClip(), form.GetBoneMap(), clipParts[c].takeName, null);
                }

                if (exportData == null || exportData.Count < 1)
                {
                    // Nothing to save go to the next one
                    continue;
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
        public List<string> GetSaveClipData(AnimationClip clip, IDictionary<string, int> boneMap, string clipName, List<string> bonesFilter)
        {
            if (clip == null || boneMap == null)
            {
                form.AddMessageLine("Animation does not exist in the file: " + clipName);
                return null;
            }

            return ParseClips.GetAnimationClipData(clip, boneMap, bonesFilter);
        }

        // Extracts the file names from the paths and validate that the file exists
        private bool ExtractPathNames(string modelRelativeFile, string takeFullFile)
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
