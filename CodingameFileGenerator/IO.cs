using System.IO.Abstractions;

namespace CodingameFileGenerator
{
    public static class IO
    {
        /// <summary>
        /// Mockable singleton IO Object Representing the System.IO namespace
        /// </summary>
        public static IFileSystem This { get; set; }
    }
}
