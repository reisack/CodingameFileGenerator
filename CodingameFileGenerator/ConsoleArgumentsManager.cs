using CommandLine;
using System.Collections.Generic;

namespace CodingameFileGenerator
{
    public class Options
    {
        [Option('o', "output", Required = false, HelpText = "Output file folder, ex : --output C:\\Dev\\my-project (Default value : Same as executable)")]
        public string OutputFolder { get; set; }

        [Option('r', "root-folder", Required = false, HelpText = "Root folder for recursively finding source files, ex : --root-folder C:\\Dev\\my-project\\src (Default value : Same as executable)")]
        public string RootFolderPath { get; set; }

        [Option('f', "first-file", Required = false, HelpText = "Content of the provided file name will be at the top of output file, ex : --first-file botconts (for BotConsts.cs file)")]
        public string FirstFileName { get; set; }
    }

    public class ConsoleArgumentsManager
    {
        public string OutputFilepath { get; private set; }
        public string RootFolderPath { get; private set; }
        public string FirstFileName { get; private set; }

        public ConsoleArgumentsManager(IEnumerable<string> args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions);
        }

        public bool AreConsoleArgumentsValid()
        {
            bool rootFolderPathValid = !string.IsNullOrWhiteSpace(RootFolderPath);
            bool outputFilepathValid = !string.IsNullOrWhiteSpace(OutputFilepath);
            return rootFolderPathValid && outputFilepathValid;
        }

        private void RunOptions(Options opts)
        {
            if (!string.IsNullOrWhiteSpace(opts.RootFolderPath))
            {
                RootFolderPath = opts.RootFolderPath;
            }
            else
            {
                RootFolderPath = DirectoryHelper.GetCurrentDirectory();
            }

            const string OUTPUT_FILE_NAME = "_codingame_output.cs";
            if (!string.IsNullOrWhiteSpace(opts.OutputFolder))
            {
                OutputFilepath = $"{ opts.OutputFolder }\\{ OUTPUT_FILE_NAME }";
            }
            else
            {
                string currentDirectory = DirectoryHelper.GetCurrentDirectory();
                if (!string.IsNullOrWhiteSpace(currentDirectory))
                {
                    OutputFilepath = $"{ currentDirectory }\\{ OUTPUT_FILE_NAME }";
                }
            }

            if (!string.IsNullOrWhiteSpace(opts.FirstFileName))
            {
                FirstFileName = opts.FirstFileName;
            }
            else
            {
                FirstFileName = null;
            }
        }
    }
}
