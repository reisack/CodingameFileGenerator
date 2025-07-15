using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace CodingameFileGenerator.Tests
{
    [TestClass]
    public class OutputGeneratorTests
    {
        private const string OUTPUT_FILEPATH = "output_path";
        private const string PATH_1 = "D:\\Dev\\Project\\File1.cs";
        private const string PATH_2 = "D:\\Dev\\Project\\File2.cs";
        private const string PATH_3 = "D:\\Dev\\Project\\File3.cs";

        [TestMethod]
        public void Run__With_No_File()
        {
            string[] filePaths = Array.Empty<string>();
            IO.This = new MockFileSystem();
            OutputGenerator output = new OutputGenerator(filePaths);

            output.Run(OUTPUT_FILEPATH);

            bool fileHasBeenGenerated = IO.This.File.Exists(OUTPUT_FILEPATH);
            Assert.IsFalse(fileHasBeenGenerated);
        }

        [TestMethod]
        public void Run__With_Two_Files_Without_Using()
        {
            string[] filePaths = { PATH_1, PATH_2 };
            IO.This = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { PATH_1, new MockFileData(FileDataProvider.ClassWithoutUsing) },
                { PATH_2, new MockFileData(FileDataProvider.EnumWithoutUsing) }
            });
            OutputGenerator output = new OutputGenerator(filePaths);

            output.Run(OUTPUT_FILEPATH);

            string outputContent = IO.This.File.ReadAllText(OUTPUT_FILEPATH);
            Assert.AreEqual(OutputGeneratorAsserts.TwoFilesWithoutUsingAssert, outputContent);
        }

        [TestMethod]
        public void Run__With_Three_Files_With_Using()
        {
            string[] filePaths = { PATH_1, PATH_2, PATH_3 };
            IO.This = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { PATH_1, new MockFileData(FileDataProvider.ClassWithUsing) },
                { PATH_2, new MockFileData(FileDataProvider.EnumWithUsing) },
                { PATH_3, new MockFileData(FileDataProvider.StaticClassWithUsing) }
            });
            OutputGenerator output = new OutputGenerator(filePaths);

            output.Run(OUTPUT_FILEPATH);

            string outputContent = IO.This.File.ReadAllText(OUTPUT_FILEPATH);
            Assert.AreEqual(OutputGeneratorAsserts.ThreeFilesWithUsingAssert, outputContent);
        }

        [TestMethod]
        public void Run__With_Three_Files_With_Using_And_First_File_Provided()
        {
            string[] filePaths = { PATH_1, PATH_2, PATH_3 };
            IO.This = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { PATH_1, new MockFileData(FileDataProvider.ClassWithUsing) },
                { PATH_2, new MockFileData(FileDataProvider.EnumWithUsing) },
                { PATH_3, new MockFileData(FileDataProvider.StaticClassWithUsing) }
            });
            OutputGenerator output = new OutputGenerator(filePaths, "file3");

            output.Run(OUTPUT_FILEPATH);

            string outputContent = IO.This.File.ReadAllText(OUTPUT_FILEPATH);
            Assert.AreEqual(OutputGeneratorAsserts.ThreeFilesWithUsingAssertAndFirstFileProvided, outputContent);
        }

        [TestMethod]
        public void Run__Must_Not_Generate_Output_File_Because_Of_Multiline_Comment_On_Using()
        {
            string[] filePaths = { PATH_1, PATH_2, PATH_3 };
            IO.This = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { PATH_1, new MockFileData(FileDataProvider.ClassWithUsing) },
                { PATH_2, new MockFileData(FileDataProvider.EnumWithMultilineCommentOnUsing) },
                { PATH_3, new MockFileData(FileDataProvider.StaticClassWithUsing) }
            });
            OutputGenerator output = new OutputGenerator(filePaths);

            output.Run(OUTPUT_FILEPATH);

            bool fileHasBeenGenerated = IO.This.File.Exists(OUTPUT_FILEPATH);
            Assert.IsFalse(fileHasBeenGenerated);
        }
    }
}
