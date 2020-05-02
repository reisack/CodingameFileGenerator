using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;

namespace CodingameFileGenerator
{
    public class OutputFileGenerator
    {
        private readonly string[] _commonTypeSystem =
        {
            "class", "struct", "enum", "interface"
        };

        private IList<string> _filesPath;

        private IList<string> _outputFileUsingsLines;
        public IReadOnlyCollection<string> OutputFileUsingsLines
        {
            get { return new ReadOnlyCollection<string>(_outputFileUsingsLines); }
        }

        private IList<string> _outputFileContentLines;
        public IReadOnlyCollection<string> OutputFileContentLines
        {
            get { return new ReadOnlyCollection<string>(_outputFileContentLines); }
        }

        public OutputFileGenerator(IEnumerable<string> filesPath, string firstFileName = null)
        {
            _filesPath = new List<string>(filesPath);
            _outputFileUsingsLines = new List<string>();
            _outputFileContentLines = new List<string>();

            if (!string.IsNullOrWhiteSpace(firstFileName))
            {
                PutFirstFileNameFirstInFilesList(firstFileName);
            }
        }

        public OutputFileWriter Run()
        {
            OutputFileWriter writer;
            if (SeparateUsingsFromOtherContent())
            {
                writer = new OutputFileWriter(OutputFileUsingsLines, OutputFileContentLines);
            }
            else
            {
                // Generation failed
                string[] noLines = Array.Empty<string>();
                writer = new OutputFileWriter(noLines, noLines);
            }

            return writer;
        }

        private void PutFirstFileNameFirstInFilesList(string firstFileName)
        {
            string firstFileFilePath = _filesPath.FirstOrDefault(filepath =>
            {
                return filepath.EndsWith($"{ firstFileName }.cs", StringComparison.OrdinalIgnoreCase);
            });

            if (!string.IsNullOrWhiteSpace(firstFileFilePath))
            {
                _filesPath.Remove(firstFileFilePath);
                _filesPath.Insert(0, firstFileFilePath);
            }
            else
            {
                Log.Warning($"Cannot put file content of { firstFileName }.cs at top of output file, this is probably a filename typo");
            }
        }

        private bool SeparateUsingsFromOtherContent()
        {
            bool processIsOk = false;
            bool usingDirectivesFinished = false;

            try
            {
                foreach (string filePath in _filesPath)
                {
                    usingDirectivesFinished = false;
                    using (StreamReader stream = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = stream.ReadLine()) != null)
                        {
                            HandleSourceCodeLine(line, usingDirectivesFinished);
                        }
                    }
                }
                processIsOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error when reading source files");
            }

            return processIsOk;
        }

        private void HandleSourceCodeLine(string line, bool usingDirectivesFinished)
        {
            if (!usingDirectivesFinished && line.Contains("using"))
            {
                // If line is a using directive and contains multiline comment, throws error
                if (line.Contains("/*") || line.Contains("*/"))
                {
                    string errorMessage = "Multiline comments in using directives must be removed for generating output file";
                    throw new MultilineCommentOnUsingException(errorMessage);
                }

                // Remove comments, multiple spaces and tabs on using directives
                string unwantedChars = @"//.*|\s+";
                line = Regex.Replace(line, unwantedChars, " ").Trim();

                if (!string.IsNullOrWhiteSpace(line) && !_outputFileUsingsLines.Contains(line))
                {
                    _outputFileUsingsLines.Add(line);
                }
            }
            else
            {
                if (!usingDirectivesFinished && _commonTypeSystem.Any(x => line.Contains(x))
                    && (line.IndexOf('/') < 0 || line.IndexOf('/') > line.IndexOfAny(_commonTypeSystem)))
                {
                    usingDirectivesFinished = true;
                }

                _outputFileContentLines.Add(line);
            }
        }
    }
}
