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