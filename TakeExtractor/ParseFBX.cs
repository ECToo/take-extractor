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
    public class ParseFBX
    {
        // Main form used to display results
        private MainForm form;

        // Save File paths
        private string pathToSaveFolder = "";
        private string fileNameWithoutExtension = "";
        private string fileExtension = "";

        // The original data read from the file
        private List<string> source = new List<string>();

        // Intermediary
        // The file is spit in to component parts 
        private enum element
        {
            Header,
            Take,
            Footer
        }

        private struct section
        {
            // What type of component this is 
            public element Position;
            // The name where applicable such as the take name
            public string Name;
            // The index in the source file to start from
            public int Start;
            // How many lines 
            public int Count;
        }

        private List<section> component = new List<section>();

        public ParseFBX(MainForm parentForm)
        {
            form = parentForm;
        }

        /// <summary>
        /// Loads a text file into an array
        /// </summary>
        public void LoadAsText(string fileName)
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

        private void ProcessData(string[] data, string fbxFullFile)
        {
            // If there is nothing do not process anything
            if (data.Length < 1)
            {
                return;
            }

            form.AddMessageLine("Processing file: " + fbxFullFile);

            source.Clear();
            source.AddRange(data);

            GetFileNames(fbxFullFile);

            ExtractComponents();
        }

        private void GetFileNames(string pathFullFile)
        {
            // Work out the file strings
            pathToSaveFolder = Path.GetDirectoryName(pathFullFile);
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathFullFile);
            // The entension includes the dot, e.g. '.fbx'
            fileExtension = Path.GetExtension(pathFullFile);
        }

        private void ExtractComponents()
        {
            int countBrackets = 0;
            component.Clear();
            section part = new section();
            part.Position = element.Header;
            part.Start = 0;
            part.Name = "";
            // One line at a time
            for (int i = 0; i < source.Count; i++)
            {
                countBrackets += CountCurlyBrackets(source[i]);
                // Has the section ended
                if (source[i].ToLowerInvariant().Contains(GlobalSettings.fbxStartTake) &&
                    !source[i].ToLowerInvariant().Contains(GlobalSettings.fbxNotStartTake) || 
                    countBrackets < 0)
                {
                    // End previous section
                    part.Count = i - part.Start;
                    component.Add(part);
                    // Start next section
                    part = new section();
                    part.Start = i;
                    if (countBrackets >= 0)
                    {
                        // Must be a take
                        part.Position = element.Take;
                        part.Name = GetTakeName(source[i]);
                    }
                    else
                    {
                        // If we've gone past the end of the section without finding a take
                        // Must be the footer
                        part.Position = element.Footer;
                        part.Name = "";
                    }
                    // Reset the counting remember there could be a bracket in this row
                    countBrackets = Math.Max(CountCurlyBrackets(source[i]), 0);
                }
            }
            // Finish the final section
            part.Count = source.Count - part.Start;
            component.Add(part);
        }

        private string GetTakeName(string line)
        {
            int start = 0;
            int count = -1;

            // Read the line a character at a time
            for (int i = 0; i < line.Length; i++)
            {
                if (count == -1 && line.Substring(i, 1) == "\"" && i < line.Length - 1)
                {
                    start = i + 1;
                    count = 0;
                }
                else if (count == 0 && line.Substring(i, 1) == "\"")
                {
                    count = i - start;
                }
            }

            if (count > 0)
            {
                return line.Substring(start, count);
            }
            return line;
        }

        private int CountCurlyBrackets(string line)
        {
            int result = 0;
            if (line.Contains(GlobalSettings.fbxStartSection))
            {
                // Add the start brackets
                result += CountInString(line, GlobalSettings.fbxStartSection);
            }

            if (line.Contains(GlobalSettings.fbxEndSection))
            {
                // Subtract the end brackets
                result -= CountInString(line, GlobalSettings.fbxEndSection);
            }

            return result;
        }

        private int CountInString(string line, string contains)
        {
            int result = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line.Substring(i, 1) == contains)
                {
                    result++;
                }
            }
            return result;
        }


    }
}
