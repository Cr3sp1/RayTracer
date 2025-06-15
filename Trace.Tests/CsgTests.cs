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
    public void TestFusion()
    {
        var fusion = new Csg(sphereA, sphereB, CsgType.Fusion);
        var ray = new Ray(new Point(5.0f, 0.0f, 0.0f), -Vec.XAxis);
        var exp0 = new HitRecord(sphereB, new Point(1.5f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.0f, 0.5f), ray, 3.5f);
        var exp1 = new HitRecord(sphereA, new Point(-1f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.5f, 0.5f), ray, 6f);
        var allHits = fusion.AllIntersects(ray);
        Assert.Equal(2, allHits.Count);
        var closestHit = fusion.Intersect(ray);
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

    [Fact]
    public void TestUnion()
    {
        var union = new Csg(sphereA, sphereB, CsgType.Union);
        var ray = new Ray(new Point(5.0f, 0.0f, 0.0f), -Vec.XAxis);
        var exp0 = new HitRecord(sphereB, new Point(1.5f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.0f, 0.5f), ray, 3.5f);
        var exp1 = new HitRecord(sphereA, new Point(1f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0f, 0.5f), ray, 4f);
        var exp2 = new HitRecord(sphereB, new Point(-0.5f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.5f, 0.5f), ray, 5.5f);
        var exp3 = new HitRecord(sphereA, new Point(-1f, 0.0f, 0.0f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.5f, 0.5f), ray, 6f);
        var allHits = union.AllIntersects(ray);
        Assert.Equal(4, allHits.Count);
        var closestHit = union.Intersect(ray);
        Assert.True(closestHit.Equals(allHits[0]));
        Assert.True(HitRecord.CloseEnough(allHits[0], exp0));
        Assert.True(HitRecord.CloseEnough(allHits[1], exp1));
        Assert.True(HitRecord.CloseEnough(allHits[2], exp2));
        Assert.True(HitRecord.CloseEnough(allHits[3], exp3));
    }

    [Fact]
    public void TestContactPoint()
    {
        var shapeA = new Sphere(Transformation.Translation(new Vec(0.5f, 0f, 0f)));
        var shapeB = new Sphere(Transformation.Translation(new Vec(-0.5f, 0f, 0f)));
        var union = new Csg(shapeA, shapeB, CsgType.Union);
        var fusion = new Csg(shapeA, shapeB, CsgType.Fusion);
        var difference = new Csg(shapeA, shapeB, CsgType.Difference);
        var intersection = new Csg(shapeA, shapeB, CsgType.Intersection);
        var ray = new Ray(new Point(0f, 0f, 5f), -Vec.ZAxis);

        var hitsUnion = union.AllIntersects(ray);
        var hitsFusion = fusion.AllIntersects(ray);
        var hitsDifference = difference.AllIntersects(ray);
        var hitsIntersection = intersection.AllIntersects(ray);

        Assert.Equal(4, hitsUnion.Count);
        Assert.True(Utils.CloseEnough(4.13397455f, hitsUnion[0].T));
        Assert.True(Utils.CloseEnough(4.13397455f, hitsUnion[1].T));
        Assert.True(Utils.CloseEnough(5.86602545f, hitsUnion[2].T));
        Assert.True(Utils.CloseEnough(5.86602545f, hitsUnion[3].T));
        
        Assert.Equal(2, hitsFusion.Count);
        Assert.True(Utils.CloseEnough(4.13397455f, hitsFusion[0].T));
        Assert.True(Utils.CloseEnough(5.86602545f, hitsFusion[1].T));
        
        Assert.Equal(2, hitsDifference.Count);
        Assert.True(Utils.CloseEnough(4.13397455f, hitsDifference[0].T));
        Assert.True(Utils.CloseEnough(5.86602545f, hitsDifference[1].T));
        
        Assert.Equal(2, hitsIntersection.Count);
        Assert.True(Utils.CloseEnough(4.13397455f, hitsIntersection[0].T));
        Assert.True(Utils.CloseEnough(5.86602545f, hitsIntersection[1].T));
    }
}