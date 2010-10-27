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
    public class ParseData
    {
        // Dividing character or separator
        public const string div = "|";

        /// <summary>
        /// To split the strings read from the file in to separate elements
        /// </summary>
        public static string[] SplitItemByDivision(string data)
        {
            char[] delim = new char[1];
            delim = div.ToCharArray();
            return data.Split(delim);
        }

    }
}
