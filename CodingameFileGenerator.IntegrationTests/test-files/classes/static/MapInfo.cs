using System;

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