using Serilog;
using System.Diagnostics;
using System.IO;

namespace CodingameFileGenerator.IntegrationTests
{
    public static class Program
    {
        public const string INTEGRATION_TESTS_FOLDER = "D:\\Dev\\dotnet\\CodingameFileGenerator\\CodingameFileGenerator.IntegrationTests";
        public const string OUTPUT_FILEPATH = INTEGRATION_TESTS_FOLDER + "\\_codingame_output.cs";

        private const string LOG_FILENAME = "CodingameFileGenerator_IntegrationTest.log";

        static void Main(string[] args)
        {
            InitLog();

            string testLabel, arguments, expectedContentFilename;

            testLabel = "Test 1 : Bad argument provided";
            arguments = $"-r { INTEGRATION_TESTS_FOLDER }\\test-files -o { INTEGRATION_TESTS_FOLDER } -z error -f program";
            PrepareTest(testLabel, arguments);

            testLabel = "Test 2 : All arguments provided with shortname";
            arguments = $"-r { INTEGRATION_TESTS_FOLDER }\\test-files -o { INTEGRATION_TESTS_FOLDER } -f program";
            expectedContentFilename = "Main_Assert.txt";
            PrepareTest(testLabel, arguments, expectedContentFilename);

            testLabel = "Test 3 : All arguments provided with fullname";
            arguments = $"--root-folder { INTEGRATION_TESTS_FOLDER }\\test-files --output { INTEGRATION_TESTS_FOLDER } --first-file program";
            expectedContentFilename = "Main_Assert.txt";
            PrepareTest(testLabel, arguments, expectedContentFilename);

            testLabel = "Test 4 : --first-file argument is not provided";
            arguments = $"-r { INTEGRATION_TESTS_FOLDER }\\test-files -o { INTEGRATION_TESTS_FOLDER }";
            expectedContentFilename = "Main_Assert.txt";
            PrepareTest(testLabel, arguments, expectedContentFilename);

            testLabel = "Test 5 : All arguments provided with Player class first";
            arguments = $"-r { INTEGRATION_TESTS_FOLDER }\\test-files -o { INTEGRATION_TESTS_FOLDER } -f player";
            expectedContentFilename = "Player_Class_First_Assert.txt";
            PrepareTest(testLabel, arguments, expectedContentFilename);

            Log.Information("===============================================");
            Log.Information("Total : {0}, Succeeded : {1}, Failed : {2}", IntegrationTest.NbTests, IntegrationTest.NbTestsSucceeded, IntegrationTest.NbTestsFailed);
            
            Log.CloseAndFlush();
            CleanOutputFile();

            // Open file with notepad
            Process.Start("notepad", LOG_FILENAME);
        }

        private static void PrepareTest(string testLabel, string arguments, string expectedContentFilename = null)
        {
            Log.Information("-----------------------------------------------");
            Log.Information(testLabel);
            Log.Information("Arguments : {0}", arguments);

            IntegrationTest test;
            if (expectedContentFilename == null)
            {
                test = new IntegrationTest(arguments);
            }
            else
            {
                test = new IntegrationTest(arguments, expectedContentFilename);
            }

            test.Execute();
        }

        private static void CleanOutputFile()
        {
            if (File.Exists(OUTPUT_FILEPATH))
            {
                File.Delete(OUTPUT_FILEPATH);
            }
        }

        private static void InitLog()
        {
            if (File.Exists(LOG_FILENAME))
            {
                File.Delete(LOG_FILENAME);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(LOG_FILENAME,
                    rollingInterval: RollingInterval.Infinite,
                    rollOnFileSizeLimit: false)
                .CreateLogger();
        }
    }
}
