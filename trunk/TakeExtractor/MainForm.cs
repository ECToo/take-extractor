#region File Description
//-----------------------------------------------------------------------------
// MainForm.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AssetData;
#endregion

namespace Extractor
{
    /// <summary>
    /// Custom form provides the main user interface for the program.
    /// In this sample we used the designer to fill the entire form with a
    /// ModelViewerControl, except for the menu bar which provides the
    /// "File / Open..." option.
    /// </summary>
    public partial class MainForm : Form
    {
        private ContentBuilder contentBuilder;
        private ContentManager contentManager;

        private string defaultFileFolder = "";
        private string lastLoadedFile = "";

        /// <summary>
        /// Constructs the main form.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            contentBuilder = new ContentBuilder();

            contentManager = new ContentManager(modelViewerControl.Services,
                                                contentBuilder.OutputDirectory);

            // A folder in the users MyDocuments
            defaultFileFolder = GetSavePath();

            /// Automatically bring up the "Load Model" dialog when we are first shown.
            //this.Shown += OpenModelMenuClicked;
        }

        private string GetSavePath()
        {
            string result = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                GlobalSettings.pathSaveGameFolder, GlobalSettings.pathSaveDataFolder);
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
            return result;
        }

        /// <summary>
        /// Event handler for the Open menu option.
        /// </summary>
        private void OpenModelMenuClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();


            fileDialog.InitialDirectory = defaultFileFolder;

            fileDialog.Title = "Load Model";

            fileDialog.Filter = "Model Files (*.fbx;*.x)|*.fbx;*.x|" +
                                "FBX Files (*.fbx)|*.fbx|" +
                                "X Files (*.x)|*.x|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                lastLoadedFile = fileDialog.FileName;
                LoadModel(fileDialog.FileName);
            }
            AddMessageLine("== Finished ==");
        }

        private void OpenTakesMenuClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = defaultFileFolder;

            fileDialog.Title = "Load Takes";

            fileDialog.Filter = "Takes Files (*.takes)|*.takes|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                lastLoadedFile = fileDialog.FileName;
                LoadTakes(fileDialog.FileName);
            }
            AddMessageLine("== Finished ==");
        }

        private void SaveBoneMapMenuClicked(object sender, EventArgs e)
        {
            if (modelViewerControl.Model == null)
            {
                // Can only save one if we have a model
                return;
            }
            SkinningData skinData = (SkinningData)modelViewerControl.Model.Tag;
            if (skinData == null)
            {
                AddMessageLine("Not an animated model!");
                return;
            }
            BoneMapSaveDialogue(GetBoneMapList(skinData));
            AddMessageLine("== Finished ==");
        }

        /// <summary>
        /// Event handler for the Exit menu option.
        /// </summary>
        private void ExitMenuClicked(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Loads a new 3D model file into the ModelViewerControl.
        /// </summary>
        public void LoadModel(string fileName)
        {
            Cursor = Cursors.WaitCursor;
            AddMessageLine("Loading model: " + fileName);

            // Unload any existing model.
            modelViewerControl.Model = null;
            contentManager.Unload();

            // Tell the ContentBuilder what to build.
            contentBuilder.Clear();
            contentBuilder.Add(fileName, "Model", null, "ModelProcessor");

            // Build this new model data.
            string buildError = contentBuilder.Build();

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                modelViewerControl.Model = contentManager.Load<Model>("Model");
            }
            else
            {
                // If the build failed, display an error message.
                AddMessageLine(buildError);
                MessageBox.Show(buildError, "Error");
            }

            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Loads a new 3D model file into the ModelViewerControl.
        /// </summary>
        public void LoadAnimatedModel(string fileName, string rotateXdeg, string rotateYdeg, string rotateZdeg)
        {
            Cursor = Cursors.WaitCursor;
            AddMessageLine("Loading model: " + fileName);

            // Unload any existing model.
            modelViewerControl.Model = null;
            contentManager.Unload();

            // Tell the ContentBuilder what to build.
            contentBuilder.Clear();
            contentBuilder.AddAnimated(fileName, "Model", rotateXdeg, rotateYdeg, rotateZdeg);

            // Build this new model data.
            string buildError = contentBuilder.Build();

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                modelViewerControl.Model = contentManager.Load<Model>("Model");
            }
            else
            {
                // If the build failed, display an error message and log it
                AddMessageLine(buildError);
                MessageBox.Show(buildError, "Error");
            }

            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Loads a text file into an array
        /// </summary>
        private void LoadTakes(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            ClearMessages();
            ParseTakes takes = new ParseTakes(this);
            takes.Load(fileName);

            Cursor = Cursors.Arrow;
        }

        public void ClearMessages()
        {
            messageBox.Clear();
        }

        public void AddMessageLine(string text)
        {
            messageBox.AppendText(text + "\n");
        }

        private void BoneMapSaveDialogue(List<string> data)
        {
            if (data.Count < 1)
            {
                AddMessageLine("No bone names!");
                return;
            }
            // Path to default location
            string pathToSaveFolder = defaultFileFolder;
            string assetName = "";
            string fileName = GlobalSettings.fileBoneMap;
            // If we have loaded a file use that for the path and the name
            if (lastLoadedFile != "")
            {
                pathToSaveFolder = Path.GetDirectoryName(lastLoadedFile);
                assetName = Path.GetFileNameWithoutExtension(lastLoadedFile) + "-";
            }
            // Append the bonemap name to the end of the filename
            fileName = assetName + fileName;

            SaveFileDialog fileDialog = new SaveFileDialog();

            fileDialog.InitialDirectory = pathToSaveFolder;

            fileDialog.Title = "Save BoneMap";

            fileDialog.FileName = fileName;

            fileDialog.Filter = "Text Files (*.txt)|*.txt|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveBoneMap(fileDialog.FileName, data);
            }

        }

        private void SaveBoneMap(string fileName, List<string> data)
        {
            if (data.Count < 1 || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            Cursor = Cursors.WaitCursor;

            AddMessageLine("Saving: " + fileName);
            File.WriteAllLines(fileName, data);

            Cursor = Cursors.Arrow;

        }

        private List<string> GetBoneMapList(SkinningData skinData)
        {
            IDictionary<string, int> boneMap = skinData.BoneMap;

            // Convert to an array so we can loop through
            string[] keys = new string[boneMap.Keys.Count];
            boneMap.Keys.CopyTo(keys, 0);
            int[] values = new int[boneMap.Values.Count];
            boneMap.Values.CopyTo(values, 0);

            // Create the list and add the value pairs to it
            List<string> bones = new List<string>();
            for (int i = 0; i < boneMap.Count; i++)
            {
                bones.Add(String.Format("{0}{1}{2}", keys[i], " = ", values[i]));
            }
            return bones;

        }


    }
}
