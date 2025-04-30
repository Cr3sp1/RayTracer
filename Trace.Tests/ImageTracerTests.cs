namespace Trace.Tests;

using Xunit;
using Xunit.Abstractions;

public class ImageTracerTests
{
    private readonly HdrImage image;
    private readonly PerspectiveCamera camera;
    private readonly ImageTracer tracer;
    private readonly ITestOutputHelper output;

    // Setup
    public ImageTracerTests(ITestOutputHelper output)
    {
        image = new HdrImage(width: 4, height: 2);
        camera = new PerspectiveCamera(aspectRatio: 2);
        tracer = new ImageTracer(image: image, camera: camera);
        this.output = output;
    }

    // Test (u,v) sub-mapping
    [Fact]
    void TestSubMapping()
    {
        var ray1 = tracer.FireRay(0, 0, 2.5f, 1.5f);
        var ray2 = tracer.FireRay(2, 1, 0.5f, 0.5f);
        Assert.True(Ray.CloseEnough(ray1, ray2));
    }

    // Test image coverage
    [Fact]
    void TestImageCoverage()
    {
        tracer.FireAllRays(ray => new Color(1.0f, 2.0f, 3.0f));

        for (int row = 0; row < image.Height; row++)
        {
            for (int col = 0; col < image.Width; col++)
            {
                Assert.Equal(new Color(1.0f, 2.0f, 3.0f), image.GetPixel(col, row));
            }
        }
    }

    // Test orientation
    [Fact]
    void TestOrientation()
    {
        // Fire ray against top-left corner of the screen
        var topLeftRay = tracer.FireRay(0, 0, 0.0f, 0.0f);
        output.WriteLine("Top Left Ray =\n" + topLeftRay.At(1.0f));
        Assert.True(Point.CloseEnough(new Point(0.0f, 2.0f, 1.0f), topLeftRay.At(1.0f)));

        // Fire ray against bottom-right corner of the screen
        var bottomRightRay = tracer.FireRay(3, 1, 1.0f, 1.0f);
        output.WriteLine("Bottom Right Ray =\n" + bottomRightRay.At(1.0f));
        Assert.True(Point.CloseEnough(new Point(0.0f, -2.0f, -1.0f), bottomRightRay.At(1.0f)));
    }
}