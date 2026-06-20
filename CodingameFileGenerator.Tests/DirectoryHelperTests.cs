using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Sinks.TestCorrelator;

namespace CodingameFileGenerator.Tests
{
    [TestClass]
    public class DirectoryHelperTests
    {
        private static readonly string CurrentDirectoryPath =
            Path.DirectorySeparatorChar == '\\' ? @"c:\workspace" : "/workspace";

        private MockFileSystem _mockFileSystem;
        private ILogger _logger;
        private IDisposable _logContext;

        [TestInitialize]
        public void SetUp()
        {
            ResetDirectoryHelperState();

            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>(), CurrentDirectoryPath);
            IO.This = _mockFileSystem;

            _logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();
            Log.Logger = _logger;

            _logContext = TestCorrelator.CreateContext();
        }

        [TestCleanup]
        public void Cleanup()
        {
            ResetDirectoryHelperState();
            _logContext?.Dispose();
            Log.CloseAndFlush();
        }

        [TestMethod]
        public void GetCurrentDirectory_WhenAvailable_ReturnsCurrentDirectory()
        {
            var currentDirectory = DirectoryHelper.GetCurrentDirectory();

            Assert.AreEqual(CurrentDirectoryPath, currentDirectory);
        }

        [TestMethod]
        public void GetCurrentDirectory_WhenCachedValueExists_DoesNotReadFileSystemAgain()
        {
            var firstDirectory = DirectoryHelper.GetCurrentDirectory();
            IO.This = null;

            var secondDirectory = DirectoryHelper.GetCurrentDirectory();

            Assert.AreEqual(firstDirectory, secondDirectory);
            Assert.AreEqual(CurrentDirectoryPath, secondDirectory);
        }

        [TestMethod]
        public void GetCurrentDirectory_WhenAccessThrows_ReturnsNullAndLogsError()
        {
            IO.This = null;

            var currentDirectory = DirectoryHelper.GetCurrentDirectory();

            Assert.IsNull(currentDirectory);

            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext();
            Assert.Contains(
                le => le.Level == Serilog.Events.LogEventLevel.Error &&
                      le.MessageTemplate.Text.Contains("Error when accessing current directory"),
                logEvents);
        }

        [TestMethod]
        public void GetCurrentDirectory_WhenInitialAccessThrows_DoesNotRetryOnLaterCalls()
        {
            IO.This = null;
            var firstDirectory = DirectoryHelper.GetCurrentDirectory();

            IO.This = _mockFileSystem;
            var secondDirectory = DirectoryHelper.GetCurrentDirectory();

            Assert.IsNull(firstDirectory);
            Assert.IsNull(secondDirectory);
        }

        [TestMethod]
        public void GetSourceFilePaths_WhenFilesExist_ReturnsMatchingPaths()
        {
            var firstCsFilePath = Path.Combine(CurrentDirectoryPath, "a.cs");
            var txtFilePath = Path.Combine(CurrentDirectoryPath, "b.txt");
            var nestedCsFilePath = Path.Combine(CurrentDirectoryPath, "sub", "c.cs");

            _mockFileSystem.AddFile(firstCsFilePath, new MockFileData("class A {}"));
            _mockFileSystem.AddFile(txtFilePath, new MockFileData("text"));
            _mockFileSystem.AddFile(nestedCsFilePath, new MockFileData("class C {}"));

            var paths = DirectoryHelper.GetSourceFilePaths(CurrentDirectoryPath, "cs", SearchOption.AllDirectories);

            CollectionAssert.AreEquivalent(
                new[]
                {
                    firstCsFilePath,
                    nestedCsFilePath
                },
                paths);
        }

        [TestMethod]
        public void GetSourceFilePaths_WhenAccessThrows_ReturnsNullAndLogsError()
        {
            var paths = DirectoryHelper.GetSourceFilePaths(null, "cs", SearchOption.TopDirectoryOnly);

            Assert.IsNull(paths);

            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext();
            Assert.Contains(
                le => le.Level == Serilog.Events.LogEventLevel.Error &&
                      le.MessageTemplate.Text.Contains("Error when finding source files"),
                logEvents);
        }

        private static void ResetDirectoryHelperState()
        {
            // DirectoryHelper caches state in private static fields and exposes no reset hook.
            // Reflection keeps production code unchanged while ensuring each test starts isolated.
            typeof(DirectoryHelper)
                .GetField("_gettingCurrentDirectoryThrowsError", BindingFlags.NonPublic | BindingFlags.Static)
                ?.SetValue(null, false);

            typeof(DirectoryHelper)
                .GetField("_currentDirectory", BindingFlags.NonPublic | BindingFlags.Static)
                ?.SetValue(null, null);
        }
    }
}
