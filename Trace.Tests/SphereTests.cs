namespace Trace.Tests;

public class SphereTests
{
    // Test Intersect
    [Fact]
    public void TestIntersect()
    {
        var unitSphere = new Sphere();

        var ray1 = new Ray(new Point(0.0f, 0.0f, 2.0f), -Vec.ZAxis);
        var exp1 = new HitRecord(unitSphere, new Point(0.0f, 0.0f, 1.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.0f, 0.0f), ray1, 1.0f);
        var hit1 = unitSphere.Intersect(ray1);
        Assert.True(hit1.HasValue);
        Assert.True(HitRecord.CloseEnough(hit1.Value, exp1));

        var ray2 = new Ray(new Point(3.0f, 0.0f, 0.0f), -Vec.XAxis);
        var exp2 = new HitRecord(unitSphere, new Point(1.0f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.0f, 0.5f), ray2, 2.0f);
        var hit2 = unitSphere.Intersect(ray2);
        Assert.True(hit2.HasValue);
        Assert.True(HitRecord.CloseEnough(hit2.Value, exp2));


        var ray3 = new Ray(new Point(0.0f, 0.0f, 0.0f), Vec.XAxis);
        var exp3 = new HitRecord(unitSphere, new Point(1.0f, 0.0f, 0.0f), new Normal(-1.0f, 0.0f, 0.0f),
            new Vec2D(0.0f, 0.5f), ray3, 1.0f);
        var hit3 = unitSphere.Intersect(ray3);
        Assert.True(hit3.HasValue);
        Assert.True(HitRecord.CloseEnough(hit3.Value, exp3));

        Assert.False(HitRecord.CloseEnough(hit2.Value, exp1));
        Assert.False(unitSphere.Intersect(new Ray(new Point(0.0f, 0.0f, 1.0f), Vec.ZAxis)).HasValue);
    }

    // Test Transformations
    [Fact]
    public void TestTransformations()
    {
        var transSphere = new Sphere(Transformation.Translation(new Vec(10.0f, 0.0f, 0.0f)));

        var ray1 = new Ray(new Point(10.0f, 0.0f, 2.0f), -Vec.ZAxis);
        var exp1 = new HitRecord(transSphere, new Point(10.0f, 0.0f, 1.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.0f, 0.0f), ray1, 1.0f);
        var hit1 = transSphere.Intersect(ray1);
        Assert.True(hit1.HasValue);
        Assert.True(HitRecord.CloseEnough(hit1.Value, exp1));

        var ray2 = new Ray(new Point(13.0f, 0.0f, 0.0f), -Vec.XAxis);
        var exp2 = new HitRecord(transSphere, new Point(11.0f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.0f, 0.5f), ray2, 2.0f);
        var hit2 = transSphere.Intersect(ray2);
        Assert.True(hit2.HasValue);
        Assert.True(HitRecord.CloseEnough(hit2.Value, exp2));

        Assert.False(HitRecord.CloseEnough(hit2.Value, exp1));
        Assert.False(transSphere.Intersect(new Ray(new Point(0.0f, 0.0f, 1.0f), -Vec.ZAxis)).HasValue);
    }
    
    [Fact]
    public void TestAllIntersects()
    {
        var unitSphere = new Sphere();

        var ray1 = new Ray(new Point(0.0f, 0.0f, 2.0f), -Vec.ZAxis);
        var exp1F = new HitRecord(unitSphere, new Point(0.0f, 0.0f, 1.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.0f, 0.0f), ray1, 1.0f);
        var exp1S = new HitRecord(unitSphere, new Point(0.0f, 0.0f, -1.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.0f, 1.0f), ray1, 3.0f);
        var allHits1 = unitSphere.AllIntersects(ray1);
        Assert.Equal(2, allHits1.Count);
        Assert.True(HitRecord.CloseEnough(allHits1[0], exp1F));
        Assert.True(HitRecord.CloseEnough(allHits1[1], exp1S));


        var ray2 = new Ray(new Point(0.0f, 0.0f, 0.0f), Vec.XAxis);
        var exp2 = new HitRecord(unitSphere, new Point(1.0f, 0.0f, 0.0f), new Normal(-1.0f, 0.0f, 0.0f),
            new Vec2D(0.0f, 0.5f), ray2, 1.0f);
        var allHits2 = unitSphere.AllIntersects(ray2);
        Assert.Single(allHits2);
        Assert.True(HitRecord.CloseEnough(allHits2[0], exp2));
    }
}