#region File Description
//-----------------------------------------------------------------------------
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
// Modified from the samples provided by
// Microsoft XNA Community Game Platform
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
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

        private string rotateX = "0";
        private string rotateY = "0";
        private string rotateZ = "0";

        private Dictionary<string, AnimationClip> loadedClips = new Dictionary<string,AnimationClip>();
        private List<string> clipNames = new List<string>();
        private string currentClipName = "";

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

        // == File

        private void OpenRigidModelMenuClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = defaultFileFolder;

            fileDialog.Title = "Load Rigid Model";

            fileDialog.Filter = "Model Files (*.fbx;*.x)|*.fbx;*.x|" +
                                "FBX Files (*.fbx)|*.fbx|" +
                                "X Files (*.x)|*.x|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ClearMessages();
                lastLoadedFile = fileDialog.FileName;
                LoadModel(fileDialog.FileName);
            }
            AddMessageLine("== Finished ==");
            HasModelLoaded();
        }

        /// <summary>
        /// Event handler for the Open menu option.
        /// </summary>
        private void OpenAnimatedModelMenuClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = defaultFileFolder;

            fileDialog.Title = "Load Animated Model";

            fileDialog.Filter = "Model Files (*.fbx;*.x)|*.fbx;*.x|" +
                                "FBX Files (*.fbx)|*.fbx|" +
                                "X Files (*.x)|*.x|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ClearMessages();
                lastLoadedFile = fileDialog.FileName;
                LoadAnimatedModel(true, fileDialog.FileName, rotateX, rotateY, rotateZ);
                //LoadAnimatedModel(true, fileDialog.FileName, "90", "0", "180");
            }
            AddMessageLine("== Finished ==");
            HasModelLoaded();
        }

        private void loadIndividualClipClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = defaultFileFolder;

            fileDialog.Title = "Load an individual animation clip";

            fileDialog.Filter = "Animation Files (*.clip)|*.clip|" +
                                //"Part Files (*.head;*.arms)|*.head;*.arms|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ClearMessages();
                LoadClip(fileDialog.FileName);
            }
            AddMessageLine("== Finished ==");
        }

        private void loadBlenderActionClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = defaultFileFolder;

            fileDialog.Title = "Load a Blender Action";

            fileDialog.Filter = "Animation Files (*.action)|*.action|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ClearMessages();
                LoadActions(fileDialog.FileName);
            }
            AddMessageLine("== Finished ==");
        }

        private void SplitFBXMenuClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = defaultFileFolder;

            fileDialog.Title = "Split FBX Model files to have only one take per file";

            fileDialog.Filter = "FBX Files (*.fbx)|*.fbx|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ClearMessages();
                lastLoadedFile = fileDialog.FileName;
                SplitFBX(fileDialog.FileName);
            }
            AddMessageLine("== Finished ==");
            HasModelLoaded();
        }

        private void OpenTakesMenuClicked(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = defaultFileFolder;

            fileDialog.Title = "Load a list of animation takes and save them in a keyframe format";

            fileDialog.Filter = "Takes Files (*.takes)|*.takes|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ClearMessages();
                lastLoadedFile = fileDialog.FileName;
                LoadTakes(fileDialog.FileName);
            }
            AddMessageLine("== Finished ==");
            HasModelLoaded();
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
                SaveBoneMapMenu.Enabled = false;
                SaveBindPoseMenuItem.Enabled = false;
                return;
            }
            BoneMapSaveDialogue(GetBoneMapList(skinData));
            AddMessageLine("== Finished ==");
        }

        private void SaveBindPoseMenuClicked(object sender, EventArgs e)
        {
            if (modelViewerControl.Model == null)
            {
                // Can only save one if we have a model
                return;
            }
            BindPoseSaveDialogue(GetBindPoseList());
            AddMessageLine("== Finished ==");
        }

        /// <summary>
        /// Event handler for the Exit menu option.
        /// </summary>
        private void ExitMenuClicked(object sender, EventArgs e)
        {
            Close();
        }

        // == View

        private void yUpClicked(object sender, EventArgs e)
        {
            yUpMenuItem.Checked = true;
            zUpMenuItem.Checked = false;
            zDownMenuItem.Checked = false;
            modelViewerControl.ViewUp = 1;
        }

        private void zUpClicked(object sender, EventArgs e)
        {
            yUpMenuItem.Checked = false;
            zUpMenuItem.Checked = true;
            zDownMenuItem.Checked = false;
            modelViewerControl.ViewUp = 2;
        }

        private void zDownClicked(object sender, EventArgs e)
        {
            yUpMenuItem.Checked = false;
            zUpMenuItem.Checked = false;
            zDownMenuItem.Checked = true;
            modelViewerControl.ViewUp = 3;
        }

        /// <summary>
        /// Call this to enable the various menu items that require an already loaded animated model
        /// </summary>
        private void HasModelLoaded()
        {
            if (modelViewerControl.Model == null || !modelViewerControl.IsAnimated)
            {
                PoseHeading.Visible = false;
                ClipNamesComboBox.Visible = false;
                ClipNamesComboBox.Enabled = false;
                SaveBoneMapMenu.Enabled = false;
                SaveBindPoseMenuItem.Enabled = false;
                LoadIndividualClipMenu.Enabled = false;
                loadBlenderActionMenuItem.Enabled = false;
                return;
            }
            PoseHeading.Visible = true;
            ClipNamesComboBox.Visible = true;
            ClipNamesComboBox.Text = currentClipName;
            ClipNamesComboBox.Enabled = true;
            SaveBoneMapMenu.Enabled = true;
            SaveBindPoseMenuItem.Enabled = true;
            LoadIndividualClipMenu.Enabled = true;
            loadBlenderActionMenuItem.Enabled = true;
        }

        private void HaveClipsLoaded()
        {
            ClipNamesComboBox.Items.Clear();
            ClipNamesComboBox.Items.Add(GlobalSettings.listRestPoseName);
            if (loadedClips != null && loadedClips.Count < 1)
            {
                return;
            }
            ClipNamesComboBox.Items.AddRange(clipNames.ToArray());
            ClipNamesComboBox.Text = currentClipName;
        }

        /// <summary>
        /// Loads a new 3D model file into the ModelViewerControl.
        /// </summary>
        public void LoadModel(string fileName)
        {
            LoadAnimatedModel(false, fileName, rotateX, rotateY, rotateZ);
        }

        /// <summary>
        /// Loads a new 3D model file into the ModelViewerControl.
        /// </summary>
        public void LoadAnimatedModel(bool isAnimated, string fileName, string rotateXdeg, string rotateYdeg, string rotateZdeg)
        {
            Cursor = Cursors.WaitCursor;

            // Unload any existing model.
            modelViewerControl.UnloadModel();
            contentManager.Unload();

            // Tell the ContentBuilder what to build.
            contentBuilder.Clear();
            ClearClips();
            if (isAnimated)
            {
                AddMessageLine("Loading animated model: " + fileName);
                contentBuilder.AddAnimated(fileName, "Model", rotateXdeg, rotateYdeg, rotateZdeg);
            }
            else
            {
                AddMessageLine("Loading model: " + fileName);
                contentBuilder.Add(fileName, "Model", null, "ModelProcessor");
            }
            AddMessageLine("Rotating model: X " + rotateXdeg + ", Y " + rotateYdeg + ", Z " + rotateZdeg);

            // Build this new model data.
            string buildError = contentBuilder.Build();
            string buildWarnings = contentBuilder.Warnings();
            if (!string.IsNullOrEmpty(buildWarnings))
            {
                AddMessageLine(buildWarnings);
            }

            if (string.IsNullOrEmpty(buildError))
            {
                // If the build succeeded, use the ContentManager to
                // load the temporary .xnb file that we just created.
                string result = modelViewerControl.SetModel(isAnimated, contentManager.Load<Model>("Model"));
                if (!string.IsNullOrEmpty(result))
                {
                    AddMessageLine(result);
                }
                currentClipName = GlobalSettings.listRestPoseName;
            }
            else
            {
                // If the build failed, display an error message and log it
                AddMessageLine(buildError);
                MessageBox.Show(buildError, "Error");
            }

            Cursor = Cursors.Arrow;
        }

        public SkinningData GetModelSkinData()
        {
            if (modelViewerControl.Model == null)
            {
                return null;
            }
            return (SkinningData)modelViewerControl.Model.Tag;
        }

        private void SplitFBX(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            if (!File.Exists(fileName))
            {
                AddMessageLine("File not found: " + fileName);
                Cursor = Cursors.Arrow;
                return;
            }

            ParseFBX fbx = new ParseFBX(this);
            fbx.LoadAsText(fileName);

            LoadAnimatedModel(true, fileName, rotateX, rotateY, rotateZ);

            fbx.SaveIndividualFBXtakes();

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

        /// <summary>
        /// Loads a text file and converts to an Animation Clip
        /// </summary>
        private void LoadClip(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            ClearMessages();
            ParseClips clips = new ParseClips(this);
            AnimationClip clip = null;
            // Wrap in a try catch just in case the file format is wrong
            try
            {
                clip = clips.Load(fileName);
            }
            catch (Exception e)
            {
                AddMessageLine(e.ToString());
            }

            if (clip != null)
            {
                string name = Path.GetFileNameWithoutExtension(fileName);
                if (loadedClips.ContainsKey(name))
                {
                    // rename to avoid duplicates
                    name += DateTime.Now.ToString(GlobalSettings.timeFormat);
                }
                clipNames.Add(name);
                loadedClips.Add(name, clip);
                currentClipName = name;
                string error = modelViewerControl.SetExternalClip(clip);
                if (!string.IsNullOrEmpty(error))
                {
                    AddMessageLine(error);
                    DisplayTheBindPose();
                }
            }
            else
            {
                AddMessageLine("The clip did not load!");
            }
            HaveClipsLoaded();

            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Loads a text file and converts to an Animation Clip
        /// </summary>
        private void LoadActions(string fileName)
        {
            Cursor = Cursors.WaitCursor;

            ClearMessages();
            ParseBlenderAction clips = new ParseBlenderAction(this);
            clipNames.Clear();
            loadedClips.Clear();
            // Wrap in a try catch just in case the file format is wrong
            try
            {
                loadedClips = clips.Load(fileName, modelViewerControl.Model,
                                                    rotateX, rotateY, rotateZ);
            }
            catch (Exception e)
            {
                AddMessageLine(e.ToString());
            }

            if (loadedClips != null && loadedClips.Count > 0)
            {
                clipNames.AddRange(loadedClips.Keys);
                AddMessageLine(string.Format("{0} actions loaded.", loadedClips.Count));
            }
            else
            {
                AddMessageLine("No actions loaded!");
            }
            HaveClipsLoaded();

            Cursor = Cursors.Arrow;
        }

        public void ClearMessages()
        {
            messageBox.Clear();
        }

        // Ready for a new model to load
        private void ClearClips()
        {
            clipNames.Clear();
            currentClipName = "";
            loadedClips.Clear();
            HaveClipsLoaded();
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

            fileDialog.Title = "Save a list of bone names with their numeric index";

            fileDialog.FileName = fileName;

            fileDialog.Filter = "Text Files (*.txt)|*.txt|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveTextFile(fileDialog.FileName, data);
            }

        }

        private void BindPoseSaveDialogue(List<string> data)
        {
            if (data == null || data.Count < 1)
            {
                AddMessageLine("No bind pose!");
                return;
            }
            // Path to default location
            string pathToSaveFolder = defaultFileFolder;
            string assetName = "";
            string fileName = GlobalSettings.fileBindPose;
            // If we have loaded a file use that for the path and the name
            if (lastLoadedFile != "")
            {
                pathToSaveFolder = Path.GetDirectoryName(lastLoadedFile);
                assetName = Path.GetFileNameWithoutExtension(lastLoadedFile) + "-";
            }
            // Append the name to the end of the filename
            fileName = assetName + fileName;

            SaveFileDialog fileDialog = new SaveFileDialog();

            fileDialog.InitialDirectory = pathToSaveFolder;

            fileDialog.Title = "Save the bind pose matrices of the model";

            fileDialog.FileName = fileName;

            fileDialog.Filter = "Text Files (*.txt)|*.txt|" +
                                "All Files (*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveTextFile(fileDialog.FileName, data);
            }

        }

        private void SaveTextFile(string fileName, List<string> data)
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

        public List<string> GetBoneMapList(SkinningData skinData)
        {
            IDictionary<string, int> boneMap = skinData.BoneMap;

            // Convert to an array so we can loop through
            string[] keys = new string[boneMap.Keys.Count];
            boneMap.Keys.CopyTo(keys, 0);
            int[] values = new int[boneMap.Values.Count];
            boneMap.Values.CopyTo(values, 0);

            // Create the list to store the results
            List<string> results = new List<string>();
            // Add the headings
            results.Add("Bone = #  [ Parent = # ]");
            results.Add("========================");
            // Add the value pairs to the list
            for (int i = 0; i < boneMap.Count; i++)
            {
                int parent = skinData.SkeletonHierarchy[values[i]];
                if (parent >= 0 && parent < keys.Length)
                {
                    results.Add(String.Format("{0} = {1}  [ {2} = {3} ]", keys[i], values[i], keys[parent], parent));
                }
                else
                {
                    results.Add(String.Format("{0} = {1}  [ Root ]", keys[i], values[i]));
                }
            }
            return results;
        }

        public List<string> GetBindPoseList()
        {
            SkinningData skinData = (SkinningData)modelViewerControl.Model.Tag;
            if (skinData == null)
            {
                AddMessageLine("Not an animated model!");
                SaveBoneMapMenu.Enabled = false;
                SaveBindPoseMenuItem.Enabled = false;
                return null;
            }

            Matrix[] bonePose = new Matrix[skinData.BindPose.Count];
            skinData.BindPose.CopyTo(bonePose, 0);

            // Get the bone map so we can see the bone names
            IDictionary<string, int> boneMap = skinData.BoneMap;
            // Reverse the lookup
            Dictionary<int, string> reverseBoneMap = new Dictionary<int, string>();

            // Convert to an array so we can loop through
            string[] keys = new string[boneMap.Keys.Count];
            boneMap.Keys.CopyTo(keys, 0);
            int[] values = new int[boneMap.Values.Count];
            boneMap.Values.CopyTo(values, 0);

            // Fill reverse bonemap
            for (int b = 0; b < keys.Length; b++)
            {
                reverseBoneMap.Add(values[b], keys[b]);
            }

            // Create the list to store the results
            List<string> results = new List<string>();
            // Add the headings
            results.Add("A= Pose Bone     | Bind Pose Transform Matrix");
            results.Add("B= Armature Bone | Transform Matrix");
            results.Add("================================================");
            // Add the value to the list
            for (int i = 0; i < bonePose.Length; i++)
            {
                // Bind pose matrices
                results.Add(String.Format("A= {0} | {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16}", 
                    reverseBoneMap[i], 
                    bonePose[i].M11, bonePose[i].M12, bonePose[i].M13, bonePose[i].M14, 
                    bonePose[i].M21, bonePose[i].M22, bonePose[i].M23, bonePose[i].M24, 
                    bonePose[i].M31, bonePose[i].M32, bonePose[i].M33, bonePose[i].M34, 
                    bonePose[i].M41, bonePose[i].M42, bonePose[i].M43, bonePose[i].M44));
                // Skeleton matrices
                Matrix armature = modelViewerControl.Model.Bones[i].Transform;
                results.Add(String.Format("B= {0} | {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} {16}",
                    modelViewerControl.Model.Bones[i].Name,
                    armature.M11, armature.M12, armature.M13, armature.M14,
                    armature.M21, armature.M22, armature.M23, armature.M24,
                    armature.M31, armature.M32, armature.M33, armature.M34,
                    armature.M41, armature.M42, armature.M43, armature.M44));
            }
            return results;
        }

        private void XComboBoxChanged(object sender, EventArgs e)
        {
            rotateX = XComboBox.Text;
            rotateX = rotateX.Substring(2);
        }

        private void YComboBoxChanged(object sender, EventArgs e)
        {
            rotateY = YComboBox.Text;
            rotateY = rotateY.Substring(2);
        }

        private void ZComboBoxChanged(object sender, EventArgs e)
        {
            rotateZ = ZComboBox.Text;
            rotateZ = rotateZ.Substring(2);
        }

        private void ClipNamesComboBoxChanged(object sender, EventArgs e)
        {
            string nextClipName = ClipNamesComboBox.Text;
            // Always re-apply the bind pose when selected
            // otherwise only change if a different pose has been selected
            if (nextClipName != currentClipName || nextClipName == GlobalSettings.listRestPoseName)
            {
                if (loadedClips.ContainsKey(nextClipName))
                {
                    currentClipName = nextClipName;
                    string error = modelViewerControl.SetExternalClip(loadedClips[currentClipName]);
                    if (!string.IsNullOrEmpty(error))
                    {
                        AddMessageLine(error);
                        // Display Bind Pose if there are any errors
                        DisplayTheBindPose();
                    }
                }
                else
                {
                    // Anything else displays the Bind pose
                    DisplayTheBindPose();
                }
            }
        }

        private void DisplayTheBindPose()
        {
            if (modelViewerControl.Model != null)
            {
                // Set the clip to null to show the bind pose
                string error = modelViewerControl.SetExternalClip(null);
                currentClipName = GlobalSettings.listRestPoseName;
                // To avoid an endless loop do not set the text unless it has changed
                if (ClipNamesComboBox.Text != currentClipName)
                {
                    ClipNamesComboBox.Text = currentClipName;
                }
                if (!string.IsNullOrEmpty(error))
                {
                    AddMessageLine(error);
                }
            }

        }

    }
}
