namespace Trace.Tests;

using Xunit;
using Xunit.Abstractions;

public class PigmentTests
{
    private readonly ITestOutputHelper output;

    public PigmentTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    // Test Uniform Pigment
    [Fact]
    public void TestUniformPigment()
    {
        var color = new Color(1.0f, 2.0f, 3.0f);
        var pigment = new UniformPigment(color);

        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(0.0f, 0.0f)), color));
        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(1.0f, 0.0f)), color));
        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(0.0f, 1.0f)), color));
        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(1.0f, 1.0f)), color));
    }

    // Test Checkered Pigment
    [Fact]
    public void TestCheckeredPigment()
    {
        var color1 = new Color(1.0f, 2.0f, 3.0f);
        var color2 = new Color(10.0f, 20.0f, 30.0f);
        var pigment = new CheckeredPigment(color1, color2, 2);

        output.WriteLine("Color in position (0.25,0.25) =\n" + pigment.GetColor(new Vec2D(0.25f, 0.25f)).ToString());
        output.WriteLine("Color in position (0.75,0.25) =\n" + pigment.GetColor(new Vec2D(0.75f, 0.25f)).ToString());
        output.WriteLine("Color in position (0.25,0.75) =\n" + pigment.GetColor(new Vec2D(0.25f, 0.75f)).ToString());
        output.WriteLine("Color in position (0.75,0.75) =\n" + pigment.GetColor(new Vec2D(0.75f, 0.75f)).ToString());

        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(0.25f, 0.25f)), color1));
        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(0.75f, 0.25f)), color2));
        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(0.25f, 0.75f)), color2));
        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(0.75f, 0.75f)), color1));
    }

    // Test Image Pigment
    [Fact]
    public void TestImagePigment()
    {
        var image = new HdrImage(2, 2);
        var color1 = new Color(1.0f, 2.0f, 3.0f);
        var color2 = new Color(2.0f, 3.0f, 1.0f);
        var color3 = new Color(2.0f, 1.0f, 3.0f);
        var color4 = new Color(3.0f, 2.0f, 1.0f);

        image.SetPixel(0, 0, color1);
        image.SetPixel(1, 0, color2);
        image.SetPixel(0, 1, color3);
        image.SetPixel(1, 1, color4);

        var pigment = new ImagePigment(image);

        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(0.0f, 0.0f)), color1));
        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(1.0f, 0.0f)), color2));
        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(0.0f, 1.0f)), color3));
        Assert.True(Color.CloseEnough(pigment.GetColor(new Vec2D(1.0f, 1.0f)), color4));
    }
}