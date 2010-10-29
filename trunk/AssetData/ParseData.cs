#region File Description
//-----------------------------------------------------------------------------
// ParseData.cs
//
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
//-----------------------------------------------------------------------------
// Helper to split up and convert strings
// Where they are converted they are done so using CultureInfo.InvariantCulture
// That ensures they have the same result whatever language is set.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
#endregion

namespace AssetData
{

    /// <summary>
    /// Parse string data to extract components
    /// </summary>
    public class ParseData
    {
        // == For all
        // Dividing character or separator
        public const string div = "|";
        public const string sep = "?";
        // Placeholder for when a list is empty
        public const string hold = "-";

        // True and False as short strings used to save to data files
        public const string shortTrue = "T";
        public const string shortFalse = "F";

        // Decimal place accuracy enough for map positions
        public const int mapDecimalAccuracy = 2;

        // Space separated floats "xx.x yy.y zz.z"
        public static string VectorToString(Vector3 change)
        {
            string sReturn = "";
            sReturn += FloatToString(change.X) + " ";
            sReturn += FloatToString(change.Y) + " ";
            sReturn += FloatToString(change.Z);
            return sReturn;
        }

        // Space separated floats "xx.x yy.y zz.z"
        public static Vector3 StringToVector(string change)
        {
            string[] numbers = SplitNumbersAtSpaces(change);
            Vector3 vReturn = Vector3.Zero;
            if (numbers.Length > 2)
            {
                vReturn.X = FloatFromString(numbers[0]);
                vReturn.Y = FloatFromString(numbers[1]);
                vReturn.Z = FloatFromString(numbers[2]);
            }
            return vReturn;
        }

        public static string MatrixToString(Matrix change)
        {
            string sReturn = "";
            sReturn += FloatToString(change.M11, -1) + " ";
            sReturn += FloatToString(change.M12, -1) + " ";
            sReturn += FloatToString(change.M13, -1) + " ";
            sReturn += FloatToString(change.M14, -1) + " ";
            sReturn += FloatToString(change.M21, -1) + " ";
            sReturn += FloatToString(change.M22, -1) + " ";
            sReturn += FloatToString(change.M23, -1) + " ";
            sReturn += FloatToString(change.M24, -1) + " ";
            sReturn += FloatToString(change.M31, -1) + " ";
            sReturn += FloatToString(change.M32, -1) + " ";
            sReturn += FloatToString(change.M33, -1) + " ";
            sReturn += FloatToString(change.M34, -1) + " ";
            sReturn += FloatToString(change.M41, -1) + " ";
            sReturn += FloatToString(change.M42, -1) + " ";
            sReturn += FloatToString(change.M43, -1) + " ";
            sReturn += FloatToString(change.M44, -1);
            return sReturn;
        }

        public static Matrix StringToMatrix(string change)
        {
            string[] numbers = SplitNumbersAtSpaces(change);
            Matrix mReturn = Matrix.Identity;
            if (numbers.Length > 15)
            {
                mReturn.M11 = FloatFromString(numbers[0]);
                mReturn.M12 = FloatFromString(numbers[1]);
                mReturn.M13 = FloatFromString(numbers[2]);
                mReturn.M14 = FloatFromString(numbers[3]);
                mReturn.M21 = FloatFromString(numbers[4]);
                mReturn.M22 = FloatFromString(numbers[5]);
                mReturn.M23 = FloatFromString(numbers[6]);
                mReturn.M24 = FloatFromString(numbers[7]);
                mReturn.M31 = FloatFromString(numbers[8]);
                mReturn.M32 = FloatFromString(numbers[9]);
                mReturn.M33 = FloatFromString(numbers[10]);
                mReturn.M34 = FloatFromString(numbers[11]);
                mReturn.M41 = FloatFromString(numbers[12]);
                mReturn.M42 = FloatFromString(numbers[13]);
                mReturn.M43 = FloatFromString(numbers[14]);
                mReturn.M44 = FloatFromString(numbers[15]);
            }
            return mReturn;
        }

        // Space separated floats "xx.x yy.y zz.z ww.w"
        public static string ColorToString(Vector4 change)
        {
            string sReturn = "";
            sReturn += FloatToString(change.X, -1) + " ";
            sReturn += FloatToString(change.Y, -1) + " ";
            sReturn += FloatToString(change.Z, -1) + " ";
            sReturn += FloatToString(change.W, -1);
            return sReturn;
        }

        public static Vector4 StringToColor(string change)
        {
            string[] numbers = SplitNumbersAtSpaces(change);
            Vector4 vReturn = Vector4.Zero;
            if (numbers.Length > 3)
            {
                vReturn.X = FloatFromString(numbers[0]);
                vReturn.Y = FloatFromString(numbers[1]);
                vReturn.Z = FloatFromString(numbers[2]);
                vReturn.W = FloatFromString(numbers[3]);
            }
            return vReturn;
        }

        public static bool ShortStringToBool(string change)
        {
            if (change == shortTrue)
            {
                return true;
            }
            return false;
        }

        public static string BoolToShortString(bool change)
        {
            // Return a string based on the collide setings used to save the map file
            if (change)
            {
                return shortTrue;
            }
            return shortFalse;
        }

        // Converts to ticks first
        public static string TimeToString(TimeSpan time)
        {
            return LongToString(time.Ticks);
        }

        // Returns a time even if it's zero
        public static TimeSpan TimeFromString(string changeTicks)
        {
            return new TimeSpan(LongFromString(changeTicks));
        }

        // Force rounding to the default level to minimise file space usage
        public static string FloatToString(float change)
        {
            return FloatToString(change, mapDecimalAccuracy);
        }

        // Usually 2 decimal places is enough
        // set decimals to -1 to turn off rounding
        public static string FloatToString(float change, int decimals)
        {
            if (decimals >= 0)
            {
                change = (float)Math.Round(change, decimals);
            }
            return change.ToString(CultureInfo.InvariantCulture);
        }

        public static float FloatFromString(string change)
        {
            return Convert.ToSingle(change, CultureInfo.InvariantCulture);
        }

        public static int IntFromString(string change)
        {
            return Convert.ToInt32(change, CultureInfo.InvariantCulture);
        }

        public static string IntToString(int change)
        {
            return change.ToString(CultureInfo.InvariantCulture);
        }

        public static long LongFromString(string change)
        {
            return Convert.ToInt64(change, CultureInfo.InvariantCulture);
        }

        public static string LongToString(long change)
        {
            return change.ToString(CultureInfo.InvariantCulture);
        }

        // To return an array of ints
        public static int[] StringToIntArray(string data)
        {
            return StringToIntList(data).ToArray();
        }

        // To a List of ints
        public static List<int> StringToIntList(string data)
        {
            string[] sNumbers = SplitNumbersAtSpaces(data);
            List<int> listNumbers = new List<int>();
            for (int i = 0; i < sNumbers.Length; i++)
            {
                listNumbers.Add(IntFromString(sNumbers[i]));
            }
            return listNumbers;
        }

        // Create a list of ints separated by spaces
        public static string IntListToString(List<int> change)
        {
            string sReturn = "";
            foreach (int i in change)
            {
                sReturn += IntToString(i) + " ";
            }
            // Remove leading and trailing blank space
            sReturn = sReturn.Trim();
            return sReturn;
        }

        // Create a list of ints separated by spaces
        public static string IntArrayToString(int[] change)
        {
            string sReturn = "";
            foreach (int i in change)
            {
                sReturn += IntToString(i) + " ";
            }
            // Remove leading and trailing blank space
            sReturn = sReturn.Trim();
            return sReturn;
        }

        /// <summary>
        /// To split the strings read from the file in to separate elements
        /// </summary>
        public static string[] SplitItemByDivision(string data)
        {
            char[] delim = new char[1];
            delim = div.ToCharArray();
            return data.Split(delim);
        }

        /// <summary>
        /// To split the strings read from the file in to separate elements
        /// </summary>
        public static string[] SplitItemBySeparator(string data)
        {
            char[] delim = new char[1];
            delim = sep.ToCharArray();
            return data.Split(delim);
        }

        /// <summary>
        /// To split the string wherever there is a dot (.)
        /// </summary>
        public static string[] SplitItemAtDots(string data)
        {
            char[] delim = new char[] { '.' };
            return data.Split(delim);
        }

        // To split Vector and Matrix strings which have numbers separated by spaces
        public static string[] SplitNumbersAtSpaces(string data)
        {
            char[] delim = new char[]{' '};
            return RemoveBlanks(data.Split(delim));
        }

        // Return a string array with no blank items
        // This is because the Xbox String functions do not have this option built in
        private static string[] RemoveBlanks(string[] data)
        {
            List<string> nonBlanks = new List<string>();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != "")
                {
                    nonBlanks.Add(data[i]);
                }
            }
            return nonBlanks.ToArray();
        }

        public static string ToLower(string change)
        {
            return change.ToLower(CultureInfo.InvariantCulture);
        }

        #region Validation

        /// <summary>
        /// Make sure the characters used are valid for an XNA asset name including the 
        /// folder path and return a safe string.
        /// </summary>
        /// <remarks>This is very similar to a filename but excludes the full stop .</remarks>
        public static string ValidateAssetname(string strWords)
        {
            // Remove leading and trailing blank space
            strWords = strWords.Trim();
            // Replace the escape character \ with the folder character /
            strWords = strWords.Replace("\\", "/");

            // Restrict file names to Unicode character codes in hex (decimal):
            // space = 0020 (32)
            // () = 0028 - 0029
            // + = 002B
            // - = 002D
            // / = 002F
            // 0-9 = 0030 - 0039
            // A-Z = 0041 - 005A
            // _ = 005F
            // a-z = 0061 - 007A
            // ~ = 007E (126)

            // For Regex see:
            // http://msdn.microsoft.com/en-us/library/system.text.regularexpressions.regex_members.aspx
            // http://www.codeproject.com/KB/dotnet/regextutorial.aspx
            // Remove all the characters we don't want by replacing with a blank string (note the -- as - is a special character)
            // [^a]= not "a" e.g. [^a-zA-Z0-9 ()+--/_~] is not any alphanumerc and those other characters
            strWords = Regex.Replace(strWords, "[^a-zA-Z0-9 ()+--/_~]", "");

            // For titles and descriptions (but not filenames)
            // Allow most characters from space 0020 (32) to ~ 007E (126) but
            // exclude " (quote) | (pipe) \ (escape)
            // If the font file is extended allow any other normal 
            // letter or number from other languages.  

            return strWords;
        }

        /// <summary>
        /// This is a filename with no file extension and no folder.
        /// </summary>
        public static string ValidateFilePart(string strWords)
        {
            // Make sure it does not contain illegal characters
            string sReturn = ValidateAssetname(strWords);
            // Remove folder / as this is not valid for a filename
            sReturn = sReturn.Replace("/", "_");
            return sReturn;
        }

        /// <summary>
        /// This reference is used as a unique identifies for triggers.
        /// </summary>
        public static string ValidateReference(string strWords)
        {
            // Make sure it does not contain illegal characters and convert to uppercase
            string result = ValidateFilePart(strWords).ToUpper();
            // get rid of spaces
            return result.Replace(" ", "_");
        }

        // http://www.regular-expressions.info/examples.html

        /// <summary>
        /// Restrict the characters that can be typed in a description field and 
        /// return a safe string.
        /// </summary>
        public static string ValidateDescription(string strWords)
        {
            // Remove leading and trailing blank space
            strWords = strWords.Trim();
            // Replace the escape character \ with the folder character /
            strWords = strWords.Replace("\\", "/");
            // Restrict to the characters we consider safe to use.  
            // Any characters not listed after ^ are not allowed0
            strWords = Regex.Replace(strWords, "[^a-zA-Z0-9 !.,'?()+--/_~]", "");

            // Non-English (Latin) character sets:
            // If the font file is extended we could allow any other normal 
            // letter or number from other languages.  

            return strWords;
        }

        /// <summary>
        /// Return just the file name without an extension from a full path
        /// which includes the file extension.  Returns just the name without any path.
        /// This is similar to ExtractAssetNameFromAsset(...) but the source is a filename
        /// </summary>
        public static string ExtractShortAssetNameFromFullFile(string fullpath)
        {
            string sReturn = "";
            // Remove leading and trailing blank space
            fullpath = fullpath.Trim();
            // Replace the escape character \ with the folder character /
            fullpath = fullpath.Replace("\\", "/");
            // split the path in to separate folders and files (Xbox 360 only supports char[] not string)
            char[] delim = new char[] { '/', '.' };
            string[] words = fullpath.Split(delim);
            // The second from last entry must be the filename
            if (words != null && words.Length > 1)
            {
                sReturn = words[words.Length - 2];
            }
            return sReturn;
        }

        /// <summary>
        /// Return just the file name without an extension from a full path
        /// which includes the file extension.  Returns just the name without any path.
        /// This is similar to ExtractAssetNameFromAsset(...) but the source is a filename
        /// </summary>
        public static string ExtractShortAssetNameFromManifest(string fullpath)
        {
            string sReturn = "";
            // Remove leading and trailing blank space
            fullpath = fullpath.Trim();
            // Replace the escape character \ with the folder character /
            fullpath = fullpath.Replace("\\", "/");
            // split the path in to separate folders and files (Xbox 360 only supports char[] not string)
            // Similar to files but there is no extension in the manifest entry
            char[] delim = new char[] { '/' };
            string[] words = fullpath.Split(delim);
            // The last entry must be the asset name
            if (words != null && words.Length > 0)
            {
                sReturn = words[words.Length - 1];
            }
            return sReturn;
        }

        /*
        /// <summary>
        /// UNTESTED: Return just the path one level up from a full path including the filename
        /// </summary>
        public static string ExtractOneLevelUp(string fullpath)
        {
            string sReturn = "";
            // Remove leading and trailing blank space
            fullpath = fullpath.Trim();
            // Replace the escape character \ with the folder character /
            fullpath = fullpath.Replace("\\", "/");
            // split the path in to separate folders and files (Xbox 360 only supports char[] not string)
            char[] delim = new char[] { '/' };
            string[] words = fullpath.Split(delim);
            // Recreate a path
            if (words.Length > 2)
            {
                // The last words will be the file and also remove one folder level up
                for (int i = 0; i < words.Length - 2; i++)
                {
                    sReturn = Path.Combine(sReturn, words[i]);
                }
            }

            return sReturn;
        }
         * */

        /// <summary>
        /// Return just the asset name from a full path of a content asset which does 
        /// not include an extension.
        /// This is similar to ExtractAssetNameFromFile(...) but the source is a content asset name
        /// </summary>
        public static string ExtractShortAssetFromLongAsset(string fullpath)
        {
            string sReturn = "";
            // Remove leading and trailing blank space
            fullpath = fullpath.Trim();
            // Replace the escape character \ with the folder character /
            fullpath = fullpath.Replace("\\", "/");
            // split the path in to separate folders and files (Xbox 360 only supports char[] not string)
            char[] delim = new char[] { '/' };
            string[] words = fullpath.Split(delim);
            // The last entry must be the asset name
            if (words != null && words.Length > 0)
            {
                sReturn = words[words.Length - 1];
            }
            return sReturn;
        }

        // Remove the path from an asset and make sure all characters are valid
        public static string ValidateShortAssetFromLongAsset(string fullpath)
        {
            return ExtractShortAssetFromLongAsset(ValidateAssetname(fullpath));
        }

        public static float ValidateFloatFromString(string strWords)
        {
            if (strWords == null)
            {
                return 0.0f;
            }
            // Remove leading and trailing blank space
            strWords = strWords.Trim();
            // Restrict to the characters valid as numbers
            strWords = Regex.Replace(strWords, "[^0-9.+--]", "");
            // Must be a valid float
            // ^ Must start at the start of the string and $ end at the end of the string
            // This means that it only finds one occurence and not any match within a longer string
            // Can optionally start with a plus or minus max one character ? (0 or 1)
            // Any number * of numeric characters followed by an optional decimal point ? (0 or 1)
            // Followed by some or no other + numeric characters
            if (Regex.IsMatch(strWords, "^[-+]?[0-9]*[.]?[0-9]+$"))
            {
                // Return the number
                return FloatFromString(strWords);
            }
            return 0.0f;
        }

        public static int ValidateIntFromString(string strWords)
        {
            if (strWords == null)
            {
                return 0;
            }
            // Remove leading and trailing blank space
            strWords = strWords.Trim();
            // Restrict to the characters valid as numbers
            strWords = Regex.Replace(strWords, "[^0-9+--]", "");
            // Must be a valid int
            if (Regex.IsMatch(strWords, "^[-+]?[0-9]+$"))
            {
                // Return the number
                return IntFromString(strWords);
            }
            return 0;
        }

        public static string ValidateStringIsInt(string strWords)
        {
            int i = ValidateIntFromString(strWords);
            return IntToString(i);
        }

        #endregion

    }
}
