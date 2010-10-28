#region File Description
//-----------------------------------------------------------------------------
// ReplaceBones.cs
//
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
//-----------------------------------------------------------------------------
// Used for animations
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
#endregion

namespace AssetData
{
    // Have a guess what this is
    public class ReplaceBones
    {
        // The contents of the dictionary list contain the transforms for just the bones we want to move.
        // Bone, Transform for that bone
        [ContentSerializer]
        public IDictionary<int, Matrix> transform;

        public ReplaceBones()
        {
            transform = new Dictionary<int, Matrix>();
        }

    }
}
