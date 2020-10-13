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
        public string SupersededFile { get; set; }

        public AdrFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string num = fileName.Split("-")[0];
            
            if(int.TryParse(num, out int number))
                Number = number;

            string[] content = File.Exists(filePath) ?
                File.ReadAllLines(filePath) : null;

            if (content?.Length > 0)
            {
                foreach(string line in content)
                {

                    var match = Regex.Match(line, @"(?<![^\s])#(\s*)[0-9]+[.][\s]*.*");
                    if(match.Success)
                    {
                        string header = match.Value;

                        Regex titleRegex = new Regex(@"(?<![^\s])#(\s*)[0-9]+[.][\s]*");
                        Title = titleRegex.Replace(header, string.Empty, 1);
                        break;
                    }
                }
            }
        }

        
    }
}
