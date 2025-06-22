namespace Trace;

/// <summary>
/// Class representing the connection between <c>HdrImage</c> and <c>Camera</c>.
/// </summary>
public class ImageTracer
{
    public HdrImage Image;
    public ICamera Camera;
    public Renderer Renderer;

    // Constructor
    public ImageTracer(HdrImage image, ICamera camera, Renderer renderer) =>
        (Image, Camera, Renderer) = (image, camera, renderer);

    /// <summary>
    /// Fire a ray towards a pixel (col, row) of HdrImage;
    /// inside the pixel the ray is centered at position (uPixel, vPixel).
    /// </summary>
    public Ray FireRay(int col, int row, float uPixel = 0.5f, float vPixel = 0.5f)
    {
        float u = (col + uPixel) / Image.Width;
        float v = 1.0f - (row + vPixel) / Image.Height;
        return Camera.FireRay(u, v);
    }

    /// <summary>
    /// Fire rays towards all pixels of HdrImage.
    /// </summary>
    public void FireAllRays()
    {
        Console.Out.WriteLine("Starting rendering...");
        int totalPixels = int.Max(Image.Width * Image.Height, 1);
        int processedPixels = 0;
        int oldPercentage = -1;

        for (int row = 0; row < Image.Height; row++)
        {
            for (int col = 0; col < Image.Width; col++)
            {
                var ray = FireRay(col, row);
                Color color = Renderer.Render(ray);
                Image.SetPixel(col, row, color);

                processedPixels++;
                int processedPercentage = 100 * processedPixels / totalPixels;
                if (processedPercentage > oldPercentage)
                {
                    Console.Write($"\rProgress: {processedPercentage}%");
                    oldPercentage = processedPercentage;
                }
            }
        }

        Console.WriteLine("\nRendering complete.");
    }
}