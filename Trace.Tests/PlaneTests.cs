namespace Trace.Tests;

using Xunit;
using Xunit.Abstractions;

public class PlaneTests
{
    private readonly ITestOutputHelper output;
    
    public PlaneTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    
    // Test intersect method
    [Fact]
    public void TestIntersect()
    {
        var unitPlane = new Plane();

        var ray1 = new Ray(new Point(0.0f, 1.0f, 2.0f), -Vec.ZAxis);
        var exp1 = new HitRecord(new Point(0.0f, 1.0f, 0.0f), new Normal(0.0f, 0.0f, 1.0f), new Vec2D(0.0f, 0.0f), 2.0f,
            ray1);
        var hit1 = unitPlane.Intersect(ray1);
        output.WriteLine("Intersection =\n" + hit1.ToString());
        Assert.True(hit1.HasValue);
        Assert.True(HitRecord.CloseEnough(hit1.Value, exp1));
        
        var ray2 = new Ray(new Point(1.0f, -2.0f, 2.0f), new Vec(-1.0f, 2.0f, -2.0f));
        var exp2 = new HitRecord(new Point(0.0f, 0.0f, 0.0f), new Normal(0.0f, 0.0f, 1.0f), new Vec2D(0.0f, 0.0f), 1.0f,
            ray2);
        var hit2 = unitPlane.Intersect(ray2);
        output.WriteLine("Intersection =\n" + hit2.ToString());
        Assert.True(hit2.HasValue);
        Assert.True(HitRecord.CloseEnough(hit2.Value, exp2));
        
        var ray3 = new Ray(new Point(-1.1f, 2.3f, -1.0f), Vec.ZAxis);
        var exp3 = new HitRecord(new Point(-1.1f, 2.3f, 0.0f), new Normal(0.0f, 0.0f, -1.0f), new Vec2D(0.9f, 0.3f), 1.0f,
            ray3);
        var hit3 = unitPlane.Intersect(ray3);
        output.WriteLine("Intersection =\n" + hit3.ToString());
        Assert.True(hit3.HasValue);
        Assert.True(HitRecord.CloseEnough(hit3.Value, exp3));
        
        var ray4 = new Ray(new Point(-1.1f, 2.3f, -1.0f), Vec.XAxis);
        var hit4 = unitPlane.Intersect(ray4);
        Assert.True(hit4 == null);
    }
    
    // Test transformation of plane
    [Fact]
    public void TestTransformations()
    {
        var transPlane1 = new Plane(Transformation.Translation(new Vec(0.0f, 0.0f, 10.0f)));
        var ray1 = new Ray(new Point(0.0f, 1.0f, 2.0f), -Vec.ZAxis);
        var hit1 = transPlane1.Intersect(ray1);
        Assert.True(hit1 == null);
        
        var transPlane2 = new Plane(Transformation.Translation(new Vec(0.0f, 0.0f, -10.0f)));
        var ray2 = new Ray(new Point(0.0f, 1.0f, 2.0f), -Vec.ZAxis);
        var exp2 = new HitRecord(new Point(0.0f, 1.0f, -10.0f), new Normal(0.0f, 0.0f, 1.0f), new Vec2D(0.0f, 0.0f), 12.0f,
            ray2);
        var hit2 = transPlane2.Intersect(ray2);
        output.WriteLine("Intersection =\n" + hit2.ToString());
        Assert.True(hit2.HasValue);
        Assert.True(HitRecord.CloseEnough(hit2.Value, exp2));
        
        var transPlane3 = new Plane(Transformation.RotationX(90));
        var ray3 = new Ray(new Point(0.0f, 2.0f, 5.0f), -Vec.YAxis);
        var exp3 = new HitRecord(new Point(0.0f, 0.0f, 5.0f), new Normal(0.0f, 1.0f, 0.0f), new Vec2D(0.0f, 0.0f), 2.0f,
            ray3);
        var hit3 = transPlane3.Intersect(ray3);
        output.WriteLine("Intersection =\n" + hit3.ToString());
        Assert.True(hit3.HasValue);
        Assert.True(HitRecord.CloseEnough(hit3.Value, exp3));
    }
}