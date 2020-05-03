using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CodingameFileGenerator.IntegrationTests
{
    public class IntegrationTest
    {
        private const string EXE_FILENAME = "C:\\Dev\\CodingameFileGenerator\\CodingameFileGenerator\\bin\\Release\\netcoreapp3.1\\CodingameFileGenerator.exe";
        
        private string _arguments;
        private string _expectedContentFilename;
        private bool _outputFileShouldBeCreated;

        public static int NbTestsSucceeded { get; private set; } = 0;
        public static int NbTestsFailed { get; private set; } = 0;

        public static int NbTests
        {
            get { return NbTestsSucceeded + NbTestsFailed; }
        }

        /// <summary>
        /// Assert output file is not created
        /// </summary>
        /// <param name="badArguments">Command line arguments</param>
        public IntegrationTest(string badArguments)
        {
            _arguments = badArguments;
            _expectedContentFilename = null;
            _outputFileShouldBeCreated = false;
        }

        /// <summary>
        /// Assert output file is created and same as expected content
        /// </summary>
        /// <param name="arguments">Command line arguments</param>
        /// <param name="expectedContentFilename">Filename of expected content</param>
        public IntegrationTest(string arguments, string expectedContentFilename)
        {
            _arguments = arguments;
            _expectedContentFilename = expectedContentFilename;
            _outputFileShouldBeCreated = true;
        }

        public void Execute()
        {
            ProcessStartInfo startInfos = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = EXE_FILENAME,
                Arguments = _arguments
            };

            bool testSucceeded = ProcessTest(startInfos);

            if (testSucceeded)
            {
                NbTestsSucceeded++;
            }
            else
            {
                NbTestsFailed++;
            }
        }

        private bool ProcessTest(ProcessStartInfo startInfos)
        {
            bool testSucceeded = false;
            using (Process process = Process.Start(startInfos))
            {
                if (process.WaitForExit(2000))
                {
                    // Check Exit code
                    if (process.ExitCode != 0)
                    {
                        Log.Error($"Unexpected exit code { process.ExitCode }");
                    }
                    else
                    {
                        if (_outputFileShouldBeCreated)
                        {
                            testSucceeded = OutputFileSameAsExpected();
                        }
                        else
                        {
                            testSucceeded = !File.Exists(Program.OUTPUT_FILEPATH);
                            if (testSucceeded)
                            {
                                Log.Information("Output file has not been created - test OK");
                            }
                            else
                            {
                                Log.Error("Output file has been created - test KO");
                            }
                        }
                    }
                }
                else
                {
                    Log.Error("Timeout Error");
                }
            }

            return testSucceeded;
        }

        private bool OutputFileSameAsExpected()
        {
            bool filesAreIdentical = false;
            FileInfo actual = new FileInfo(Program.OUTPUT_FILEPATH);
            FileInfo expected = new FileInfo($"{Program.INTEGRATION_TESTS_FOLDER}\\{ _expectedContentFilename }");

            bool sameBinary = false;
            bool sameLength = actual.Length == expected.Length;

            if (sameLength)
            {
                sameBinary = File.ReadAllBytes(expected.FullName)
                    .SequenceEqual(File.ReadAllBytes(actual.FullName));
            }

            if (sameLength && sameBinary)
            {
                Log.Information("Files are identical - Test OK");
                filesAreIdentical = true;
            }
            else
            {
                Log.Error("Files are different - Test KO");
            }

            return filesAreIdentical;
        }
    }
}
