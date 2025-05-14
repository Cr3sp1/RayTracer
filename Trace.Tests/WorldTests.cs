namespace Trace.Tests;

using Xunit;
using Xunit.Abstractions;

public class WorldTests
{
    private readonly ITestOutputHelper output;
    
    public WorldTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    
    // Test intersection method
    [Fact]
    public void TestIntersectAll()
    {
        var world = new World();
        
        var unitSphere = new Sphere();
        world.AddShape(unitSphere);
        var transPlane1 = new Plane(Transformation.Translation(new Vec(0.0f, 0.0f, -2.0f)));
        world.AddShape(transPlane1);
        
        var ray1 = new Ray(new Point(0.0f, 0.0f, 2.0f), -Vec.ZAxis);
        var exp1 = new HitRecord(new Point(0.0f, 0.0f, 1.0f), new Normal(0.0f, 0.0f, 1.0f), new Vec2D(0.0f, 0.0f), 1.0f,
            ray1);
        var hit1 = world.IntersectAll(ray1);
        output.WriteLine("Intersection =\n" + hit1.ToString());
        Assert.True(hit1.HasValue);
        Assert.True(HitRecord.CloseEnough(hit1.Value, exp1));
        
        var transPlane2 = new Plane(Transformation.Translation(new Vec(0.0f, 0.0f, 1.5f)));
        world.AddShape(transPlane2);
        
        var exp2 = new HitRecord(new Point(0.0f, 0.0f, 1.5f), new Normal(0.0f, 0.0f, 1.0f), new Vec2D(0.0f, 0.0f), 0.5f,
            ray1);
        var hit2 = world.IntersectAll(ray1);
        output.WriteLine("Intersection =\n" + hit2.ToString());
        Assert.True(hit2.HasValue);
        Assert.True(HitRecord.CloseEnough(hit2.Value, exp2));
    }
}