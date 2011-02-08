#region File Description
//-----------------------------------------------------------------------------
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace Extractor
{
    public static class GlobalSettings
    {
        // Parse FBX files (lowercase)
        public const string fbxStartTakes = "takes:";
        public const string fbxCurrentTake = "current:";
        public const string fbxStartTake = "take:";
        public const string fbxNotStartTake = "multitake:";
        public const string fbxStartSection = "{";
        public const string fbxEndSection = "}";

        // Paths and files
        public const string pathContentFolder = "../../../Content/"; // The relative path to the content folder
        public const string fileFloor = "grid.fbx"; // The local path to the content
        public const string timeFormat = "yyyymmddhhmmss";  // Names that need to be unique
        public const string pathSaveGameFolder = "SavedGames";   // Same as the XNA default
        public const string pathSaveDataFolder = "ExtractTakes";  // used to load and save the results
        public const string fileBoneMap = "BoneMap.txt";    // appended to the model name to save a bonemap
        public const string fileBindPose = "BindPose.txt";    // appended to the model name to save a bind pose

        // Save keyframes
        // All lowercase
        public const string itemRigType = "rigtype";
        public const string itemHeadBones = "headbones";
        public const string itemArmsBones = "armbones";
        public const string itemClipTake = "clip";
        public const string itemHeadTake = "head";
        public const string itemArmsTake = "arms";

        // Names
        public const string listRestPoseName = "* Rest or bind pose";
    }
}
