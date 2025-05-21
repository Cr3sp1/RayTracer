namespace Trace.Tests;

public class RendererTest
{
    private readonly Color sphereColor;
    private readonly HdrImage image;
    private readonly OrthogonalCamera camera;
    private readonly World scene;

    // Setup
    public RendererTest()
    {
        sphereColor = new Color(1.0f, 2.0f, 3.0f);
        image = new HdrImage(width: 3, height: 3);
        camera = new OrthogonalCamera();
        var sphere = new Sphere(transform: Transformation.Translation(new Vec(2.0f, 0.0f, 0.0f)) *
                                           Transformation.Scaling(new Vec(0.2f, 0.2f, 0.2f)),
            material: new Material(new Brdf(new UniformPigment(sphereColor))));
        scene = new World();
        scene.AddShape(sphere);
        // renderer = new Renderer(scene);
        // tracer = new ImageTracer(image: image, camera: camera, renderer: renderer);
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
}