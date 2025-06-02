namespace Trace.Tests;

using Xunit;
using Xunit.Abstractions;

public class RendererTest
{
    private readonly Color sphereColor;
    private readonly HdrImage image;
    private readonly OrthogonalCamera camera;
    private readonly World scene;
    private readonly ITestOutputHelper output;

    // Setup
    public RendererTest(ITestOutputHelper output)
    {
        sphereColor = new Color(1.0f, 2.0f, 3.0f);
        image = new HdrImage(width: 3, height: 3);
        camera = new OrthogonalCamera();
        var sphere = new Sphere(transform: Transformation.Translation(new Vec(2.0f, 0.0f, 0.0f)) *
                                           Transformation.Scaling(new Vec(0.2f, 0.2f, 0.2f)),
            material: new Material(new Brdf(new UniformPigment(sphereColor))));
        scene = new World();
        scene.AddShape(sphere);
        this.output = output;
    }

    // Test OnOffRenderer
    [Fact]
    public void TestOnOffRenderer()
    {
        var renderer = new OnOffRenderer(scene);
        var tracer = new ImageTracer(image: image, camera: camera, renderer: renderer);
        tracer.FireAllRays();

        Assert.True(Color.CloseEnough(image.GetPixel(0, 0), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(1, 0), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(2, 0), Color.Black));

        Assert.True(Color.CloseEnough(image.GetPixel(0, 1), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(1, 1), Color.White));
        Assert.True(Color.CloseEnough(image.GetPixel(2, 1), Color.Black));

        Assert.True(Color.CloseEnough(image.GetPixel(0, 2), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(1, 2), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(2, 2), Color.Black));
    }

    // Test OnOffRenderer
    [Fact]
    public void TestFlatRenderer()
    {
        var renderer = new FlatRenderer(scene);
        var tracer = new ImageTracer(image: image, camera: camera, renderer: renderer);
        tracer.FireAllRays();

        Assert.True(Color.CloseEnough(image.GetPixel(0, 0), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(1, 0), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(2, 0), Color.Black));

        Assert.True(Color.CloseEnough(image.GetPixel(0, 1), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(1, 1), sphereColor));
        Assert.True(Color.CloseEnough(image.GetPixel(2, 1), Color.Black));

        Assert.True(Color.CloseEnough(image.GetPixel(0, 2), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(1, 2), Color.Black));
        Assert.True(Color.CloseEnough(image.GetPixel(2, 2), Color.Black));
    }

    // Test Path Tracer: Furnace test
    [Fact]
    public void TestPathTracer()
    {
        var pcg = new Pcg();

        // Furnace test for random values of L_e and Rho_d
        for (int i = 0; i < 5; i++)
        {
            var emittedRadiance = pcg.RandomFloat();
            output.WriteLine("Emitted radiance = " + emittedRadiance);
            var reflectance = pcg.RandomFloat() * 0.9f; // Avoid numbers too close to 1
            output.WriteLine("Reflectance = " + reflectance);

            var world = new World();
            var enclosedMaterial = new Material(new DiffuseBrdf(new UniformPigment(reflectance * Color.White)),
                new UniformPigment(emittedRadiance * Color.White));

            world.AddShape(new Sphere(material: enclosedMaterial));

            var pathTracer = new PathTracer(world, 1, 100, 101, pcg);

            var ray = new Ray(new Point(0.0f, 0.0f, 0.0f), new Vec(1.0f, 0.0f, 0.0f));
            var renderedColor = pathTracer.Render(ray);
            Assert.True(Utils.CloseEnough(renderedColor.R, renderedColor.G) &&
                        Utils.CloseEnough(renderedColor.R, renderedColor.G));
            output.WriteLine("Rendered radiance = " + renderedColor.R.ToString("F23"));

            float expected = emittedRadiance / (1.0f - reflectance);
            output.WriteLine("Expected radiance = " + expected.ToString("F23") + "\n");

            Assert.True(Utils.CloseEnough(expected, renderedColor.R, expected * 1e-5f));
        }
    }
}