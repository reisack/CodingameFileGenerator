using System;
using System.Diagnostics;
using System.IO;
using Serilog;

namespace CodingameFileGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            InitLog();
            Log.Information($"Start Program");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Run(args);

            stopwatch.Stop();
            Log.Information($"Program executed in { stopwatch.ElapsedMilliseconds } ms");
            Log.CloseAndFlush();
        }

        private static void InitLog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("CodingameFileGenerator.log",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    flushToDiskInterval: TimeSpan.FromDays(7))
                .CreateLogger();
        }

        private static void Run(string[] args)
        {
            ConsoleArgumentsManager consoleArgs = new ConsoleArgumentsManager(args);

            if (consoleArgs.AreConsoleArgumentsValid())
            {
                bool isDeleteOutputFileOk = true;
                if (File.Exists(consoleArgs.OutputFilepath))
                {
                    isDeleteOutputFileOk = FileHelper.Delete(consoleArgs.OutputFilepath);
                }

                if (isDeleteOutputFileOk)
                {
                    string[] filesPath = DirectoryHelper.GetSourceFilePaths(consoleArgs.RootFolderPath, "cs", SearchOption.AllDirectories);
                    if (filesPath != null)
                    {
                        OutputFileGenerator output = new OutputFileGenerator(filesPath, consoleArgs.FirstFileName);
                        output.Run(consoleArgs.OutputFilepath);
                    }
                }
            }
        }
    }
}