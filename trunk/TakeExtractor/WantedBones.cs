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
    public class WantedBones
    {
        // SkinninginData.BoneMap
        IDictionary<string, int> boneMap;
        // List of the bone names that we want to keep
        List<string> bonesFilter;

        public WantedBones(IDictionary<string, int> skinBoneMap, List<string> filterBones)
        {
            boneMap = skinBoneMap;  //skinData.BoneMap
            bonesFilter = filterBones;  // List of bone names
        }

        // Use to create part files
        // Arms = Shoulders, arms, hands and collar bones (AimRifle etc.) 
        // Head = Head and Neck (Look and Aim)
        public bool IsBoneWeWant(int bone)
        {
            for (int i = 0; i < bonesFilter.Count; i++)
            {
                if (bone == boneMap[bonesFilter[i]])
                {
                    return true;
                }
            }
            return false;
        }


    }
}
