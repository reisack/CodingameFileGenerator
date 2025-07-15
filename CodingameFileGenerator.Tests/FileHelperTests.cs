using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Sinks.TestCorrelator;

namespace CodingameFileGenerator.Tests
{
    [TestClass]
    public class FileHelperTests
    {
        private MockFileSystem _mockFileSystem;
        private ILogger _logger;
        private IDisposable _logContext;

        [TestInitialize]
        public void SetUp()
        {
            // Set up mock file system
            _mockFileSystem = new MockFileSystem();
            IO.This = _mockFileSystem;

            // Set up test logger
            _logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();
            Log.Logger = _logger;

            _logContext = TestCorrelator.CreateContext();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _logContext.Dispose();
            Log.CloseAndFlush();
        }

        [TestMethod]
        public void Delete_FileExists_ReturnsTrueAndDeletesFile()
        {
            // Arrange
            string path = @"c:\temp\file.txt";
            _mockFileSystem.AddFile(path, new MockFileData("test"));

            // Act
            var result = FileHelper.Delete(path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(_mockFileSystem.FileExists(path));
        }

        [TestMethod]
        public void Delete_FileDoesNotExist_ReturnsFalseAndLogsError()
        {
            // Passing null to throw a NullReferenceException with IO.This.File.Delete()
            var result = FileHelper.Delete(null);

            // Assert
            Assert.IsFalse(result);

            // Assert Serilog error log
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext();
            Assert.IsTrue(
                logEvents.Any(le => le.Level == Serilog.Events.LogEventLevel.Error &&
                                    le.MessageTemplate.Text.Contains("Error when trying to delete file")));
        }

        [TestMethod]
        public void WriteAllLines_ValidPath_ReturnsTrueAndWritesFile()
        {
            // Arrange
            string path = @"c:\temp\output.txt";
            var lines = new List<string> { "hello", "world" };

            // Act
            var result = FileHelper.WriteAllLines(path, lines);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_mockFileSystem.FileExists(path));
            CollectionAssert.AreEqual(lines, _mockFileSystem.File.ReadAllLines(path));
        }

        [TestMethod]
        public void WriteAllLines_ExceptionThrown_ReturnsFalseAndLogsError()
        {
            // Arrange
            // Create a directory with the same name, so WriteAllLines will throw
            string path = @"c:\temp\conflict.txt";
            _mockFileSystem.AddDirectory(path);

            // Act
            var result = FileHelper.WriteAllLines(path, new[] { "foo" });

            // Assert
            Assert.IsFalse(result);

            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext();
            Assert.IsTrue(
                logEvents.Any(le => le.Level == Serilog.Events.LogEventLevel.Error &&
                                    le.MessageTemplate.Text.Contains("Error when generating file")));
        }
    }
}
