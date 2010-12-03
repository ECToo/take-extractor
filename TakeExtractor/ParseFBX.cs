#region File Description
//-----------------------------------------------------------------------------
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
//-----------------------------------------------------------------------------
// Extract the text of an FBX file
//
// Splits the file in to sections.  Header, multiple takes and a footer.
//
// LIMITATIONS
// - The keyword 'Take:' must not be used for the name of any bones or other parts
// - The name of the take must be on the same line as the keyword 'Take:'
//      e.g. 'Take: "Name" {'
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
            Current,
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

            form.AddMessageLine("Extracting data from: " + fbxFullFile);

            source.Clear();
            source.AddRange(data);

            ExtractFileNames(fbxFullFile);

            ExtractComponents();
        }

        // This is used to set up the variables used to create the take file names
        public void ExtractFileNames(string pathFullFile)
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
                if ((source[i].ToLowerInvariant().Contains(GlobalSettings.fbxStartTake) &&
                    !source[i].ToLowerInvariant().Contains(GlobalSettings.fbxNotStartTake)) || 
                    countBrackets < 0 ||
                    source[i].ToLowerInvariant().Contains(GlobalSettings.fbxCurrentTake))
                {
                    // End previous section
                    part.Count = i - part.Start;
                    component.Add(part);
                    // Start next section
                    part = new section();
                    part.Start = i;
                    if (source[i].ToLowerInvariant().Contains(GlobalSettings.fbxCurrentTake))
                    {
                        // Is the Current take line
                        part.Position = element.Current;
                        part.Name = "";
                    }
                    else if (countBrackets >= 0)
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

        // == Save FBX

        public void SaveIndividualFBXtakes()
        {
            if (component.Count < 4 || source.Count < 1)
            {
                form.AddMessageLine("No takes found or unsupportted file format!");
                return;
            }

            List<string> header = new List<string>();
            List<string> current = new List<string>();
            List<string> footer = new List<string>();
            List<int> takeNumbers = new List<int>();
            for (int c = 0; c < component.Count; c++)
            {
                if (component[c].Position == element.Header)
                {
                    header.AddRange(GetLines(component[c].Start, component[c].Count));
                }
                else if (component[c].Position == element.Current)
                {
                    current.AddRange(GetLines(component[c].Start, component[c].Count));
                }
                else if (component[c].Position == element.Footer)
                {
                    footer.AddRange(GetLines(component[c].Start, component[c].Count));
                }
                else if (component[c].Position == element.Take)
                {
                    takeNumbers.Add(c);
                }
            }

            List<string> result = new List<string>();
            List<string> theTake = new List<string>();
            string fileName = "";
            // Loop through each take
            for (int t = 0; t < takeNumbers.Count; t++)
            {
                result.Clear();
                theTake.Clear();
                // Create the new FBX contents
                result.AddRange(header);
                result.AddRange(GetCurrentForThisTake(current, component[takeNumbers[t]].Name));
                result.AddRange(GetLines(component[takeNumbers[t]].Start, component[takeNumbers[t]].Count));
                result.AddRange(footer);
                // Work out the file name
                fileName = GetTakeFileName(component[takeNumbers[t]].Name);
                // Output
                SaveTheFile(fileName, result);
            }
        }

        private List<string> GetCurrentForThisTake(List<string> current, string takeName)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < current.Count; i++)
            {
                if (current[i].ToLowerInvariant().Contains(GlobalSettings.fbxCurrentTake))
                {
                    // This makes the assumption that the line is in the following format
                    //      Current: "takeName"
                    // with nothing else on the line
                    // The following tries to retain the indented formatting
                    // Find the first quote and replace the rest of string
                    string line = current[i].Substring(0, current[i].IndexOf("\""));
                    if (line.Length > 0)
                    {
                        result.Add(line + "\"" + takeName + "\"");
                    }
                }
                else
                {
                    result.Add(current[i]);
                }
            }
            return result;
        }

        public string GetTakeFileName(string takeName)
        {
            string result = Path.Combine(pathToSaveFolder, fileNameWithoutExtension);
            result += "-" + takeName.ToLowerInvariant() + fileExtension;
            return result;
        }

        public string GetFullPath(string shortfilename)
        {
            return Path.Combine(pathToSaveFolder, shortfilename);
        }

        public string GetKeyframeFileName(string rigType, string partName, string partType)
        {
            string result = Path.Combine(pathToSaveFolder, rigType);
            result += "-" + partName + "." + partType;
            return result;
        }

        private void SaveTheFile(string fullFilePath, List<string> data)
        {
            if (data.Count < 1 || string.IsNullOrEmpty(fullFilePath))
            {
                return;
            }

            form.AddMessageLine("Saving: " + fullFilePath);
            File.WriteAllLines(fullFilePath, data);

        }

        private List<string> GetLines(int from, int count)
        {
            if (from + count > source.Count)
            {
                return null;
            }
            return source.GetRange(from, count);
        }



    }
}
