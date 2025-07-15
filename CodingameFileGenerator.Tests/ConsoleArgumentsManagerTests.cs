using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO.Abstractions.TestingHelpers;

namespace CodingameFileGenerator.Tests
{
    [TestClass]
    public class ConsoleArgumentsManagerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            IO.This = new MockFileSystem();
        }

        [TestMethod]
        public void AreConsoleArgumentsValid__No_Args_At_All_In_Command_Line()
        {
            string[] arguments = Array.Empty<string>();
            ConsoleArgumentsManager consoleArgs = new ConsoleArgumentsManager(arguments);

            // Default values if no argument specified in command line
            bool argsAreValids = consoleArgs.AreConsoleArgumentsValid();

            Assert.IsTrue(argsAreValids);
        }

        [TestMethod]
        public void AreConsoleArgumentsValid__Only_One_Arg_Is_Specified()
        {
            string[] arguments =
            {
                "-r", "folder"
            };
            ConsoleArgumentsManager consoleArgs = new ConsoleArgumentsManager(arguments);

            bool argsAreValids = consoleArgs.AreConsoleArgumentsValid();

            Assert.IsTrue(argsAreValids);
        }

        [TestMethod]
        public void AreConsoleArgumentsValid__All_Args_Are_Specified_With_Shortname()
        {
            string[] arguments =
            {
                "-o", "path",
                "-r", "folder",
                "-f", "first"
            };
            ConsoleArgumentsManager consoleArgs = new ConsoleArgumentsManager(arguments);

            bool argsAreValids = consoleArgs.AreConsoleArgumentsValid();

            Assert.IsTrue(argsAreValids);
        }

        [TestMethod]
        public void AreConsoleArgumentsValid__All_Args_Are_Specified_With_Fullname()
        {
            string[] arguments =
            {
                "--output", "path",
                "--root-folder", "folder",
                "--first-file", "first"
            };
            ConsoleArgumentsManager consoleArgs = new ConsoleArgumentsManager(arguments);

            bool argsAreValids = consoleArgs.AreConsoleArgumentsValid();

            Assert.IsTrue(argsAreValids);
        }

        [TestMethod]
        public void AreConsoleArgumentsValid__One_Arg_Does_Not_Exist()
        {
            string[] arguments =
            {
                "-o", "path",
                "-r", "folder",
                "-f", "first",
                "-z", "error"
            };
            ConsoleArgumentsManager consoleArgs = new ConsoleArgumentsManager(arguments);

            bool argsAreValids = consoleArgs.AreConsoleArgumentsValid();

            Assert.IsFalse(argsAreValids);
        }
    }
}
