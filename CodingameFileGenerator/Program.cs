using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using Serilog;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace CodingameFileGenerator
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        static void Main(string[] args)
        {
            InitLog();
            InitIOWrapper();
            Log.Information("Start Program");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Run(args);

            stopwatch.Stop();
            Log.Information("Program executed in {0} ms", stopwatch.ElapsedMilliseconds);
            Log.CloseAndFlush();
        }

        private static void InitLog()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"{ exePath }\\CodingameFileGenerator.log",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    flushToDiskInterval: TimeSpan.FromDays(7))
                .CreateLogger();
        }

        private static void InitIOWrapper()
        {
            IO.This = new FileSystem();
        }

        private static void Run(string[] args)
        {
            ConsoleArgumentsManager consoleArgs = new ConsoleArgumentsManager(args);

            if (consoleArgs.AreConsoleArgumentsValid())
            {
                bool isDeleteOutputFileOk = true;
                if (IO.This.File.Exists(consoleArgs.OutputFilepath))
                {
                    isDeleteOutputFileOk = FileHelper.Delete(consoleArgs.OutputFilepath);
                }

                if (isDeleteOutputFileOk)
                {
                    string[] filesPath = DirectoryHelper.GetSourceFilePaths(consoleArgs.RootFolderPath, "cs", SearchOption.AllDirectories);

                    if (filesPath != null)
                    {
                        // Directory.GetFiles() doesn't allow to search by Regex
                        // The solution found is to find all CSharp files and remove the projectname.AssemblyInfo.cs manually
                        var filteredFilesPath = filesPath.Where(filepath => !filepath.Contains("AssemblyInfo.cs"));

                        OutputGenerator output = new OutputGenerator(filteredFilesPath, consoleArgs.FirstFileName);
                        output.Run(consoleArgs.OutputFilepath);
                    }
                }
            }
        }
    }
}