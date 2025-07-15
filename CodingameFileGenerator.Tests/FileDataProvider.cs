namespace CodingameFileGenerator.Tests
{
    public static class FileDataProvider
    {
        public const string EnumWithoutUsing =
@"public enum Direction //struct
{
    NoDirection,
    Left,
    Right,
    Up,
    Down
}";

        public const string EnumWithUsing =
@"// using toto
// class ItsATrap
using         System;
      using System.Text;

public enum Direction //struct
{
    NoDirection,
    Left,
    Right,
    Up,
    Down
}";

        public const string EnumWithMultilineCommentOnUsing =
@"// using toto
// class ItsATrap
using System; /* 
multiline comments */

public enum Direction //struct
{
    NoDirection,
    Left,
    Right,
    Up,
    Down
}";

        public const string StaticClassWithUsing =
@"  using System;
using System.IO;

public static class MapInfo
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }
    public static char[,] GridInfo { get; private set; }
    public static bool[,] PositionsDone { get; private set; }

    public static void Init(int width, int height)
    {
        Width = width;
        Height = height;
        GridInfo = new char[height, width];
        PositionsDone = new bool[height, width];
    }

    public static void ClearPositionsDone()
    {
        PositionsDone = new bool[Height, Width];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                PositionsDone[y, x] = false;
            }
        }
    }
}";

        public const string ClassWithoutUsing =
@"public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public int GetSector()
    {
        const int SECTOR_SIZE = 5;
        return ((X / SECTOR_SIZE) + 1) + ((Y / SECTOR_SIZE) * 3);
    }
}";

        public const string ClassWithUsing =
@"

using System; // class
  using System.Collection;
public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public int GetSector()
    {
        const int SECTOR_SIZE = 5;
        return ((X / SECTOR_SIZE) + 1) + ((Y / SECTOR_SIZE) * 3);
    }
}";
    }
}
