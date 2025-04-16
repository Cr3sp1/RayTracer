namespace Trace.Tests;

public class RayTests
{
    // Test Constructor and CloseEnough
    [Fact]
    public void TestCloseEnough()
    {
        var r1 = new Ray(new Point(1.0f,2.0f,3.0f), new Vec(5.0f, 4.0f, -1.0f));
        var r2 = new Ray(new Point(1.0f,2.0f,3.0f), new Vec(5.0f, 4.0f, -1.0f));
        var r3 = new Ray(new Point(1.0f,5.0f,3.0f), new Vec(5.0f, 4.0f, -1.0f));
        
        Assert.True(Ray.CloseEnough(r1, r2));
        Assert.False(Ray.CloseEnough(r1, r3));
    }
    
    // Test At method
    [Fact]
    public void TestAt()
    {
        var r = new Ray(new Point(1.0f,2.0f,4.0f), new Vec(4.0f, 2.0f, 1.0f));
        
        Assert.True(Point.CloseEnough(r.At(0.0f), r.Origin));
        Assert.True(Point.CloseEnough(r.At(1.0f), new Point(5.0f, 4.0f, 5.0f)));
        Assert.True(Point.CloseEnough(r.At(3.0f), new Point(13.0f, 8.0f, 7.0f)));
    }
}