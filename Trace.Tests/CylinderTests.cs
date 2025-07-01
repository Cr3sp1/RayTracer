namespace Trace.Tests;

public class CylinderTests
{
    // Test Intersect
    [Fact]
    public void TestIntersect()
    {
        var unitCylinder = new Cylinder();

        var ray1 = new Ray(new Point(0.5f, 0.0f, 2.0f), -Vec.ZAxis);
        var exp1 = new HitRecord(unitCylinder, new Point(0.5f, 0.0f, 1.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.0f, 0.5f), ray1, 1.0f);
        var hit1 = unitCylinder.Intersect(ray1);
        Assert.True(hit1.HasValue);
        Assert.True(HitRecord.CloseEnough(hit1.Value, exp1));

        var ray2 = new Ray(new Point(2.0f, 0.0f, 0.5f), -Vec.XAxis);
        var exp2 = new HitRecord(unitCylinder, new Point(1.0f, 0.0f, 0.5f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.0f, 0.75f), ray2, 1.0f);
        var hit2 = unitCylinder.Intersect(ray2);
        Assert.True(hit2.HasValue);
        Assert.True(HitRecord.CloseEnough(hit2.Value, exp2));
        
        var ray3 = new Ray(new Point(-0.5f, 0.0f, 0.0f), -Vec.ZAxis);
        var exp3 = new HitRecord(unitCylinder, new Point(-0.5f, 0.0f, -1.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.5f, 0.5f), ray3, 1.0f);
        var hit3 = unitCylinder.Intersect(ray3);
        Assert.True(hit3.HasValue);
        Assert.True(HitRecord.CloseEnough(hit3.Value, exp3));
        
        var ray4 = new Ray(new Point(0.0f, 0.0f, 2.0f), -Vec.ZAxis);
        var exp4 = new HitRecord(unitCylinder, new Point(0.0f, 0.0f, 1.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.0f, 0.0f), ray4, 1.0f);
        var hit4 = unitCylinder.Intersect(ray4);
        Assert.True(hit4.HasValue);
        Assert.True(HitRecord.CloseEnough(hit4.Value, exp4));
        
        var ray5 = new Ray(new Point(0.0f, 0.0f, 2.0f), -Vec.ZAxis);
        var exp5 = new HitRecord(unitCylinder, new Point(0.0f, 0.0f, 1.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.0f, 0.0f), ray5, 1.0f);
        var hit5 = unitCylinder.Intersect(ray5);
        Assert.True(hit5.HasValue);
        Assert.True(HitRecord.CloseEnough(hit5.Value, exp4));
    }
    
    // Test Transformations
    [Fact]
    public void TestTransformations()
    {
        var transCylinder = new Cylinder(Transformation.Translation(new Vec(0.0f, 0.5f, 1.0f)));

        var ray1 = new Ray(new Point(0.5f, 0.0f, 3.0f), -Vec.ZAxis);
        var exp1 = new HitRecord(transCylinder, new Point(0.5f, 0.0f, 2.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.875f, 0.707f), ray1, 1.0f);
        var hit1 = transCylinder.Intersect(ray1);
        Assert.True(hit1.HasValue);
        Assert.True(HitRecord.CloseEnough(hit1.Value, exp1, 1e-3f));

        var ray2 = new Ray(new Point(0.5f, -3.0f, 3.0f), -Vec.XAxis);
        var hit2 = transCylinder.Intersect(ray2);
        Assert.True(!hit2.HasValue);
    }
}