namespace Trace;

public class HdrImage
{
    // HdrImage fields
    public int width, height;
    public Color[] pixels;

    // Constructor
    public HdrImage(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.pixels = new Color[width * height];
    }

    // Get a pixel
    public Color GetPixel(int x, int y) => pixels[y * width + x];
    
    // Set a pixel
    public Color SetPixel(int x, int y, Color c) => pixels[y * width + x] = c;
    
}
