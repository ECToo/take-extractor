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
    public class ParseClips
    {
        // Main form used to display results
        private MainForm form;

        public ParseClips(MainForm parentForm)
        {
            form = parentForm;
        }

        /// <summary>
        /// Loads a text file and converts to an animation clip
        /// </summary>
        public AnimationClip Load(string fileName)
        {
            string[] result = new string[0];

            if (File.Exists(fileName))
            {
                result = File.ReadAllLines(fileName);
            }
            else
            {
                form.AddMessageLine("File not found: " + fileName);
                return null;
            }

            if (result == null || result.Length < 1)
            {
                form.AddMessageLine("Empty file: " + fileName);
                return null;
            }

            return ProcessData(result, fileName);
        }

        private AnimationClip ProcessData(string[] input, string fullFile)
        {
            // If there is nothing do not process anything
            if (input.Length < 1)
            {
                return null;
            }

            form.AddMessageLine("Processing file: " + fullFile);

            // First line of the file
            string[] data = ParseData.SplitNumbersAtSpaces(input[0]);
            int count = ParseData.IntFromString(data[0]);
            TimeSpan duration = ParseData.TimeFromString(data[1]);
            List<TimeSpan> steps = new List<TimeSpan>();
            if (data.Length > 2)
            {
                // The rest of the first line are the frames for the sounds of foot steps
                for (int d = 2; d < data.Length; d++)
                {
                    steps.Add(ParseData.TimeFromString(data[d]));
                }
            }
            IList<Keyframe> keyFrames = new List<Keyframe>();
            if (input.Length < 1)
            {
                form.AddMessageLine("There are no key frames in this file!");
                return null;
            }
            // Now add all the frames
            for (int i = 1; i < input.Length; i++)
            {
                string[] item = ParseData.SplitItemByDivision(input[i]);
                data = ParseData.SplitNumbersAtSpaces(item[0]);
                keyFrames.Add(new Keyframe(ParseData.IntFromString(data[0]),
                                                        ParseData.TimeFromString(data[1]),
                                                        ParseData.StringToMatrix(item[1])));
            }
            return new AnimationClip(count, duration, keyFrames, steps);
        }

    }
}
