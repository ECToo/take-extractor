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

            fbx.SaveIndividualFBXtakes();
        }

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
