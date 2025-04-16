namespace Trace;

/// <summary>
/// Class representing the connection between <c>HdrImage</c> and <c>Camera</c>.
/// </summary>
public class ImageTracer
{
    public HdrImage Image;
    public ICamera Camera;
    
    // Constructor
    public ImageTracer(HdrImage image, ICamera camera) => (Image, Camera) = (image, camera);
    
    /// <summary>
    /// Fire a ray towards pixel (col, row) of HdrImage;
    /// inside the pixel the ray is centered at position (u_pixel, v_pixel).
    /// </summary>
    public Ray FireRay(int col, int row, float u_pixel = 0.5f, float v_pixel = 0.5f)
    {
        float u = (col + u_pixel) / (Image.Width - 1.0f);
        float v = (row + v_pixel) / (Image.Height - 1.0f);
        return Camera.FireRay(u, v);
    }
    
    /// <summary>
    /// Fire rays towards all pixels of HdrImage.
    /// </summary>
    /// <param name="func">Function solving the rendering equation, taking as input a <c>Ray</c>
    /// and as output a <c>Color</c>.</param>
    public void FireAllRays(Func<Ray, Color> func)
    {
        for (int row = 0; row < Image.Height; row++)
        {
            for (int col = 0; col < Image.Width; col++)
            {
                var ray = Camera.FireRay(col, row);
                Color color = func(ray);
                Image.SetPixel(col, row, color);
            }
        }        
    }
}