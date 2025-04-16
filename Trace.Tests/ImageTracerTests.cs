namespace Trace.Tests;

public class ImageTracerTests
{
    // Test FireRay and FireAllRays
    [Fact]
    void TestImageTracer()
    {
        var image = new HdrImage(width: 4, height: 2);
        var camera = new PerspectiveCamera(aspectRatio: 2);
        var tracer = new ImageTracer(image: image, camera: camera);

        var ray1 = tracer.FireRay(0, 0, 2.5f, 1.5f);
        var ray2 = tracer.FireRay(2, 1, 0.5f, 0.5f);
        Assert.True(Ray.CloseEnough(ray1, ray2));

        tracer.FireAllRays(ray => new Color(1.0f, 2.0f, 3.0f));

        for (int row = 0; row < image.Height; row++)
        {
            for (int col = 0; col < image.Width; col++)
            {
                Assert.Equal(new Color(1.0f, 2.0f, 3.0f), image.GetPixel(col, row));
            }
        }
    }
}