namespace Trace;

public class HdrImage
{
    // HdrImage fields
    public readonly int Width, Height;
    public Color[] Pixels;

    // Constructor
    public HdrImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new Color[width * height];
    }

    // Get a pixel
    public Color GetPixel(int x, int y)
    {
        if (!ValidCoords(x, y))
            throw new ArgumentOutOfRangeException(nameof(x), "Invalid coordinates");

        return Pixels[_PixelOffset(x, y)];
    }

    // Set a pixel
    public void SetPixel(int x, int y, Color c)
    {
        if (!ValidCoords(x, y))
            throw new ArgumentOutOfRangeException(nameof(x), "Invalid coordinates");

        Pixels[_PixelOffset(x, y)] = c;
    }
    
    // Check validity of coordinates
    public bool ValidCoords(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    // Get array index corresponding to coordinates
    public int _PixelOffset(int x, int y) => y * Width + x;
}