using System.Collections.Generic;
using Serilog;

namespace CodingameFileGenerator
{
    public class OutputFileWriter
    {
        private IReadOnlyCollection<string> _outputFileUsingsLines;
        private IReadOnlyCollection<string> _outputFileContentLines;

        public OutputFileWriter(IReadOnlyCollection<string> outputFileUsingsLines, IReadOnlyCollection<string> outputFileContentLines)
        {
            _outputFileUsingsLines = outputFileUsingsLines;
            _outputFileContentLines = outputFileContentLines;
        }

        public void WriteToFile(string outputFilePath)
        {
            if (HasLinesToWrite())
            {
                List<string> outputFileAllLines = new List<string>();
                outputFileAllLines.AddRange(_outputFileUsingsLines);
                outputFileAllLines.Add("");
                outputFileAllLines.AddRange(_outputFileContentLines);

                if (FileHelper.WriteAllLines(outputFilePath, outputFileAllLines))
                {
                    Log.Information($"Output file [{ outputFilePath }] generated with { outputFileAllLines.Count } lines");
                }
            }
            else
            {
                Log.Warning("There is no line generated, output file won't be created");
            }
        }

        private bool HasLinesToWrite()
        {
            return _outputFileUsingsLines.Count + _outputFileContentLines.Count > 0;
        }
    }
}
