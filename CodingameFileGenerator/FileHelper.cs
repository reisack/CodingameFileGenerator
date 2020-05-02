using System;
using System.Collections.Generic;
using Serilog;

namespace CodingameFileGenerator
{
    public static class FileHelper
    {
        public static bool Delete(string path)
        {
            bool processIsOk = false;

            try
            {
                IO.This.File.Delete(path);
                processIsOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when trying to delete file [{ path }]");
            }

            return processIsOk;
        }

        public static bool WriteAllLines(string path, IEnumerable<string> contents)
        {
            bool processIsOk = false;

            try
            {
                IO.This.File.WriteAllLines(path, contents);
                processIsOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when generating file [{ path }]");
            }

            return processIsOk;
        }
    }
}
