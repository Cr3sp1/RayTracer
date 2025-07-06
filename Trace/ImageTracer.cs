namespace Trace;

/// <summary>
/// Class representing the connection between <c>HdrImage</c> and <c>Camera</c>.
/// </summary>
public class ImageTracer
{
    public HdrImage Image;
    public ICamera Camera;
    public Renderer Renderer;
    public int RaysPerSide;
    public Pcg Pcg;

    // Constructor
    public ImageTracer(HdrImage image, ICamera camera, Renderer renderer, int raysPerSide, Pcg pcg) =>
        (Image, Camera, Renderer, RaysPerSide, Pcg) = (image, camera, renderer, raysPerSide, pcg);

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
        float uPixel;
        float vPixel;

        for (int row = 0; row < Image.Height; row++)
        {
            for (int col = 0; col < Image.Width; col++)
            {
                if (RaysPerSide > 0)
                {
                    Color cumColor = new Color(0.0f, 0.0f, 0.0f);
                    for (int pixelRow = 0; pixelRow < RaysPerSide; pixelRow++)
                    {
                        for (int pixelCol = 0; pixelCol < RaysPerSide; pixelCol++)
                        {
                            uPixel = (pixelCol + Pcg.RandomFloat()) / RaysPerSide;
                            vPixel = (pixelRow + Pcg.RandomFloat()) / RaysPerSide;
                            var ray = FireRay(col, row, uPixel, vPixel);
                            cumColor += Renderer.Render(ray);
                        }
                    }
                    cumColor = 1.0f / (RaysPerSide * RaysPerSide) * cumColor;
                    Image.SetPixel(col, row, cumColor);
                }
                else
                {
                    var ray = FireRay(col, row);
                    Color color = Renderer.Render(ray);
                    Image.SetPixel(col, row, color);
                }

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