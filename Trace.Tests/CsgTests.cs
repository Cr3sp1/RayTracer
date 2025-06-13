// ReSharper disable InconsistentNaming

namespace Trace.Tests;

public class CsgTests
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once InconsistentNaming
    private readonly Sphere sphereA, sphereB;

    public CsgTests()
    {
        sphereA = new Sphere();
        sphereB = new Sphere(Transformation.Translation(new Vec(0.5f, 0f, 0f)));
    }

    [Fact]
    public void TestUnion()
    {
        var union = new Csg(sphereA, sphereB, CsgType.Union);
        var ray = new Ray(new Point(5.0f, 0.0f, 0.0f), -Vec.XAxis);
        var exp0 = new HitRecord(sphereB, new Point(1.5f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.0f, 0.5f), ray, 3.5f);
        var exp1 = new HitRecord(sphereA, new Point(-1f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.5f, 0.5f), ray, 6f);
        var allHits = union.AllIntersects(ray);
        Assert.Equal(2, allHits.Count);
        var closestHit = union.Intersect(ray);
        Assert.True(closestHit.Equals(allHits[0]));
        Assert.True(HitRecord.CloseEnough(allHits[0], exp0));
        Assert.True(HitRecord.CloseEnough(allHits[1], exp1));
    }

    [Fact]
    public void TestDifference()
    {
        var difference = new Csg(sphereA, sphereB, CsgType.Difference);
        var ray = new Ray(new Point(5.0f, 0.0f, 0.0f), -Vec.XAxis);
        var exp0 = new HitRecord(sphereB, new Point(-0.5f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.5f, 0.5f), ray, 5.5f);
        var exp1 = new HitRecord(sphereA, new Point(-1f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.5f, 0.5f), ray, 6f);
        var allHits = difference.AllIntersects(ray);
        Assert.Equal(2, allHits.Count);
        var closestHit = difference.Intersect(ray);
        Assert.True(closestHit.Equals(allHits[0]));
        Assert.True(HitRecord.CloseEnough(allHits[0], exp0));
        Assert.True(HitRecord.CloseEnough(allHits[1], exp1));
    }

    [Fact]
    public void TestIntersection()
    {
        var intersection = new Csg(sphereA, sphereB, CsgType.Intersection);
        var ray = new Ray(new Point(5.0f, 0.0f, 0.0f), -Vec.XAxis);
        var exp0 = new HitRecord(sphereA, new Point(1f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0f, 0.5f), ray, 4f);
        var exp1 = new HitRecord(sphereB, new Point(-0.5f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.5f, 0.5f), ray, 5.5f);
        var allHits = intersection.AllIntersects(ray);
        Assert.Equal(2, allHits.Count);
        var closestHit = intersection.Intersect(ray);
        Assert.True(closestHit.Equals(allHits[0]));
        Assert.True(HitRecord.CloseEnough(allHits[0], exp0));
        Assert.True(HitRecord.CloseEnough(allHits[1], exp1));
    }
}