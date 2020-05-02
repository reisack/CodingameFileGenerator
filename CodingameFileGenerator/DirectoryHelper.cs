using System;
using System.IO;
using Serilog;

namespace CodingameFileGenerator
{
    public static class DirectoryHelper
    {
        private static bool _gettingCurrentDirectoryThrowsError = false;
        private static string _currentDirectory = null;

        public static string GetCurrentDirectory()
        {
            if (_gettingCurrentDirectoryThrowsError || !string.IsNullOrWhiteSpace(_currentDirectory))
            {
                return _currentDirectory;
            }

            try
            {
                _currentDirectory = IO.This.Directory.GetCurrentDirectory();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error when accessing current directory");
                _gettingCurrentDirectoryThrowsError = true;
            }

            return _currentDirectory;
        }

        public static string[] GetSourceFilePaths(string path, string extension, SearchOption searchOption)
        {
            string[] filesPath = null;

            try
            {
                filesPath = IO.This.Directory.GetFiles(path, $"*.{ extension }", searchOption);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error when finding source files");
            }

            return filesPath;
        }
    }
}
