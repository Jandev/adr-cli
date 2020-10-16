using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace adr
{
    internal class AdrIndex
    {
        private const string IndexFileName = "Index";
        private const int fileNumber = 0;
        private string _indexFileName;
        private string _indexFile;
        private string _indexDocFolder;


        public AdrIndex(string docFolder)
        {
            Directory.CreateDirectory(docFolder);

            _indexDocFolder = docFolder;
            _indexFileName = $"{fileNumber.ToString().PadLeft(4, '0')}-{AdrEntry.SanitizeFileName(IndexFileName)}.md";
            _indexFile = Path.Combine(docFolder, _indexFileName);
        }

        public AdrIndex Generate()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(_indexDocFolder, "*.md", SearchOption.TopDirectoryOnly);

            using var writer = File.CreateText(_indexFile);
            {
                writer.WriteLine($"# {IndexFileName}");
                writer.WriteLine();
                writer.WriteLine("| Number | Title | Superseded by |");
                writer.WriteLine("| ------ | ----- | ------------- |");

                if (files?.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        if (fileName == _indexFileName)
                            continue;

                        AdrFile adrFile = new AdrFile(file);
                        if (string.IsNullOrWhiteSpace(adrFile.SupersededFile))
                            writer.WriteLine($"| {adrFile.Number} | [{adrFile.Title}](./{adrFile.FileName})| |");
                        else
                        {
                            AdrFile superSededFile = new AdrFile(adrFile.SupersededFile);
                            writer.WriteLine($"| {adrFile.Number} | ~~[{adrFile.Title}](./{fileName})~~| [{superSededFile.Title}](./{superSededFile.FileName}) |");
                        }
                    }
                }
            }
            return this;
        }

    }
}
