using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace adr
{
    internal class AdrFile
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string FileName { get; set; }

        /// <summary>
        /// Path to the superseded file
        /// </summary>
        public string SupersededFile { get; set; }

        public AdrFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            FileName = fileName;

            string num = fileName.Split("-")[0];
            
            if(int.TryParse(num, out int number))
                Number = number;

            string[] content = File.Exists(filePath) ?
                File.ReadAllLines(filePath) : null;

            if (content?.Length > 0)
            {
                foreach(string line in content)
                {

                    var headerMatch = Regex.Match(line, @"(?<![^\s])#(\s*)[0-9]+[.][\s]*.*");
                    if(headerMatch.Success)
                    {
                        string header = headerMatch.Value;

                        Regex titleRegex = new Regex(@"(?<![^\s])#(\s*)[0-9]+[.][\s]*");
                        Title = titleRegex.Replace(header, string.Empty, 1);
                    }

                    var superMatch = Regex.Match(line, @"(?<=Superseded - ).*(?=\s*)");
                    if (superMatch.Success)
                    {
                        string superSededLine = superMatch.Value;

                        Regex superFileRegex = new Regex(@"(?<=\(\.\/)[^\s]+(?=\))");

                        var superFileMatches = superFileRegex.Matches(superSededLine);
                        if (superFileMatches.Count > 0)
                        {
                            var superFileName = superFileMatches[superFileMatches.Count - 1].Value;
                            SupersededFile = Path.Combine(AdrSettings.Current.DocFolder, superFileName);
                        }

                        break;
                    }
                }
            }
        }

        
    }
}
