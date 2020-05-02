using System;
using System.Collections.Generic;
using System.Text;

namespace CodingameFileGenerator.Tests
{
    public class OutputGeneratorAsserts
    {
        public static string TwoFilesWithoutUsingAssert =
@"
public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public int GetSector()
    {
        const int SECTOR_SIZE = 5;
        return ((X / SECTOR_SIZE) + 1) + ((Y / SECTOR_SIZE) * 3);
    }
}
public enum Direction //struct
{
    NoDirection,
    Left,
    Right,
    Up,
    Down
}
";

        public static string ThreeFilesWithUsingAssert =
            @"using System;
using System.Collection;
using System.Text;
using System.IO;



public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public int GetSector()
    {
        const int SECTOR_SIZE = 5;
        return ((X / SECTOR_SIZE) + 1) + ((Y / SECTOR_SIZE) * 3);
    }
}
// class ItsATrap

public enum Direction //struct
{
    NoDirection,
    Left,
    Right,
    Up,
    Down
}

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
}
";

        public static string ThreeFilesWithUsingAssertAndFirstFileProvided =
@"using System;
using System.IO;
using System.Collection;
using System.Text;


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
}


public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public int GetSector()
    {
        const int SECTOR_SIZE = 5;
        return ((X / SECTOR_SIZE) + 1) + ((Y / SECTOR_SIZE) * 3);
    }
}
// class ItsATrap

public enum Direction //struct
{
    NoDirection,
    Left,
    Right,
    Up,
    Down
}
";
    }
}
