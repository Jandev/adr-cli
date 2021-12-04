using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace adr
{
    internal class AdrEntry
    {
        private readonly string docFolder;
        
        private readonly string templatePath;

        private readonly TemplateType templateType;

        private string fileName;

        private int fileNumber;

        public AdrEntry(TemplateType templateType)
        {
            this.docFolder = AdrSettings.Current.DocFolder;
            this.templateType = templateType;
            this.templatePath = $"{AdrSettings.Current.TemplateFolder}{Path.DirectorySeparatorChar}{templateType.ToString()}.md";
        }

        public string Title { get; set; } = "Record Architecture Decisions";
        public string SupersededFile { get; set; }

        public AdrEntry Write()
        {
            if (this.templateType == TemplateType.Adr)
            {
                this.WriteAdr();
            }
            else
            {
                AdrFile superSededFile = null;
                if (!string.IsNullOrWhiteSpace(this.SupersededFile))
                {
                    string superFilePath = Path.Combine(this.docFolder, Path.GetFileName(this.SupersededFile));

                    if (!File.Exists(superFilePath))
                    {
                        throw new FileNotFoundException("Superseded file does not exists");
                    }

                    superSededFile = new AdrFile(superFilePath);                    
                }

                this.WriteNew();
                
                if (superSededFile != null)
                    SupersedeAdrFileStatus(superSededFile);

            }

            new AdrIndex(this.docFolder).Generate();

            return this;
        }

        private void WriteNew()
        {
            fileNumber = Directory.Exists(this.docFolder)
                ? GetNextFileNumber(this.docFolder)
                : 1;
            fileName = Path.Combine(
                docFolder,
                $"{fileNumber.ToString().PadLeft(4, '0')}-{SanitizeFileName(this.Title)}.md");

            Directory.CreateDirectory(this.docFolder);

            WriteAdrFile(fileNumber);
        }

        private void WriteAdr()
        {
            fileNumber = Directory.Exists(this.docFolder)
                ? GetNextFileNumber(this.docFolder)
                : 1;
            fileName = Path.Combine(
                this.docFolder,
                $"{fileNumber.ToString().PadLeft(4, '0')}-{SanitizeFileName(this.Title)}.md");

            WriteInitialAdrFile(fileNumber);
        }

        private void WriteInitialAdrFile(int fileNumber)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(this.fileName)));
            using var writer = File.CreateText(this.fileName);
            {
                writer.WriteLine($"# {fileNumber}. {this.Title}");
                writer.WriteLine();
                writer.WriteLine(DateTime.Today.ToString("yyyy-MM-dd"));
                writer.WriteLine();
                writer.WriteLine("## Status");
                writer.WriteLine();
                writer.WriteLine("Accepted");
                writer.WriteLine();
                writer.WriteLine("## Context");
                writer.WriteLine();
                writer.WriteLine("We need to record the architectural decisions made on this project.");
                writer.WriteLine();
                writer.WriteLine("## Decision");
                writer.WriteLine();
                writer.WriteLine(
                    "We will use Architecture Decision Records, as described by Michael Nygard in this article: http://thinkrelevance.com/blog/2011/11/15/documenting-architecture-decisions");
                writer.WriteLine();
                writer.WriteLine("## Consequences");
                writer.WriteLine();
                writer.WriteLine("See Michael Nygard's article, linked above.");
            }
        }

        private void WriteAdrFile(int fileNumber)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(this.fileName)));
            using var writer = File.CreateText(this.fileName);
            {
                writer.WriteLine($"# {fileNumber}. {this.Title}");
                writer.WriteLine();
                writer.WriteLine(DateTime.Today.ToString("yyyy-MM-dd"));
                writer.WriteLine();
                writer.WriteLine("## Status");
                writer.WriteLine();
                writer.WriteLine("Proposed");
                writer.WriteLine();
                writer.WriteLine("## Context");
                writer.WriteLine();
                writer.WriteLine("{context}");
                writer.WriteLine();
                writer.WriteLine("## Decision");
                writer.WriteLine();
                writer.WriteLine("{decision}");
                writer.WriteLine();
                writer.WriteLine("## Consequences");
                writer.WriteLine();
                writer.WriteLine("{consequences}");
            }
        }

        private void SupersedeAdrFileStatus(AdrFile file)
        {
            var filePath = Path.Combine(this.docFolder, file.FileName);
            string[] content = File.Exists(filePath) ?
                File.ReadAllLines(filePath) : null;

            if (content?.Length > 0)
            {
                //reading file to update the status 
                bool lastNonEmptyLineContainsStatus = false;
                for (int i = 0; i < content.Length; i++)
                {

                    if (lastNonEmptyLineContainsStatus && !string.IsNullOrWhiteSpace(content[i])
                        && !Regex.IsMatch(content[i], @"##\s+Context", RegexOptions.IgnoreCase))
                    {
                        content[i] = $"Superseded - [{fileNumber.ToString().PadLeft(4, '0')} - {Title}]" +
                            $"(./{Path.GetFileName(fileName)})";

                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(content[i]))
                    {
                        lastNonEmptyLineContainsStatus = Regex.IsMatch(content[i], @"##\s+Status", RegexOptions.IgnoreCase);
                    }
                }

                //writing to the file with new status to `superseded`
                using var writer = File.CreateText(filePath);
                {
                    foreach(var line in content)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }

        public AdrEntry Launch()
        {
            try
            {
                Process.Start(this.fileName);
            }
            catch
            {
                try
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        var url = this.fileName.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", this.fileName);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", this.fileName);
                    }
                    else
                    {
                        throw;
                    }

                }
                catch
                {
                    // If there is no default application configured for markdown files, don't throw an exception.
                }
            }

            return this;
        }

        private static int GetNextFileNumber(string docFolder)
        {
            int fileNumOut = 0;
            var files =
                from file in new DirectoryInfo(docFolder).GetFiles("*.md", SearchOption.TopDirectoryOnly)
                let fileNum = file.Name.Substring(0, 4)
                where int.TryParse(fileNum, out fileNumOut)
                select fileNumOut;
            var maxFileNum = files.Any() ? files.Max() : 0;
            return maxFileNum + 1;
        }

        public static string SanitizeFileName(string title)
        {
            return title
                .Replace(' ', '-')
                .ToLower();
        }
    }
}