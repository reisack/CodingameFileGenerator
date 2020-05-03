using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using Serilog;

namespace CodingameFileGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            InitLog();
            InitIOWrapper();
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
                        OutputGenerator output = new OutputGenerator(filesPath, consoleArgs.FirstFileName);
                        output.Run(consoleArgs.OutputFilepath);
                    }
                }
            }
        }
    }
}