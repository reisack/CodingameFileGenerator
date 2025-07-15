using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodingameFileGenerator.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void IndexOfAny__All_Words_Are_In_The_Line()
        {
            string line = "stan kenny kyle cartman";
            string[] words = { "stan", "kenny", "kyle", "cartman" };

            int index = line.IndexOfAny(words);

            Assert.AreEqual(0, index);
        }

        [TestMethod]
        public void IndexOfAny__All_Words_Are_In_The_Line_But_With_Different_Case()
        {
            string line = "Stan kennY kyle cartman";
            string[] words = { "stan", "kenny", "kyle", "cartman" };

            int index = line.IndexOfAny(words);

            Assert.AreEqual(11, index);
        }

        [TestMethod]
        public void IndexOfAny__All_Words_Are_In_The_Line_But_Not_In_The_Same_Order_In_Array()
        {
            string line = "stan kenny kyle cartman";
            string[] words = { "kyle", "cartman", "stan", "kenny" };

            int index = line.IndexOfAny(words);

            Assert.AreEqual(0, index);
        }

        [TestMethod]
        public void IndexOfAny__One_Half_Of_A_Word_Is_In_The_Line()
        {
            string line = "stan kenny kyle cartman";
            string[] words = { "homer", "artm", "bart", "lisa" };

            int index = line.IndexOfAny(words);

            Assert.AreEqual(17, index);
        }

        [TestMethod]
        public void IndexOfAny__None_Of_Words_Are_In_The_Line()
        {
            string line = "stan kenny kyle cartman";
            string[] words = { "homer", "marge", "bart", "lisa" };

            int index = line.IndexOfAny(words);

            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void IndexOfAny__No_Words_At_All()
        {
            string line = "stan kenny kyle cartman";
            string[] words = Array.Empty<string>();

            int index = line.IndexOfAny(words);

            Assert.AreEqual(-1, index);
        }

        [TestMethod]
        public void IndexOfAny__Words_Array_Is_Null()
        {
            string line = "stan kenny kyle cartman";
            string[] words = null;

            Assert.ThrowsExactly<ArgumentNullException>(() => line.IndexOfAny(words));
        }
    }
}
