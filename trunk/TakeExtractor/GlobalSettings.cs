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
        public const string fbxStartTakes = "Takes:";
        public const string fbxStartTake = "take:";
        public const string fbxNotStartTake = "multitake:";
        public const string fbxStartSection = "{";
        public const string fbxEndSection = "}";

        // Paths and files
        public const string pathSaveGameFolder = "SavedGames";   // Same as the XNA default
        public const string pathSaveDataFolder = "ExtractTakes";  // used to load and save the results

    }
}
