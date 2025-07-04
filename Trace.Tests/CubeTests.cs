namespace Trace.Tests;

public class CubeTests
{
    // Test Intersect
    [Fact]
    public void TestIntersect()
    {
        var unitCube = new Cube();

        var ray1 = new Ray(new Point(0.0f, 0.0f, 2.0f), -Vec.ZAxis);
        var exp1 = new HitRecord(unitCube, new Point(0.0f, 0.0f, 1.0f), new Normal(0.0f, 0.0f, 1.0f),
            new Vec2D(0.5f, 0.5f), ray1, 1.0f, 3);
        var hit1 = unitCube.Intersect(ray1);
        Assert.True(hit1.HasValue);
        Assert.True(HitRecord.CloseEnough(hit1.Value, exp1));

        var ray2 = new Ray(new Point(3.0f, 0.5f, 0.5f), -Vec.XAxis);
        var exp2 = new HitRecord(unitCube, new Point(1.0f, 0.5f, 0.5f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.75f, 0.75f), ray2, 2.0f, 1);
        var hit2 = unitCube.Intersect(ray2);
        Assert.True(hit2.HasValue);
        Assert.True(HitRecord.CloseEnough(hit2.Value, exp2));


        var ray3 = new Ray(new Point(0.0f, 0.0f, 0.0f), Vec.XAxis);
        var exp3 = new HitRecord(unitCube, new Point(1.0f, 0.0f, 0.0f), new Normal(-1.0f, 0.0f, 0.0f),
            new Vec2D(0.5f, 0.5f), ray3, 1.0f, 1);
        var hit3 = unitCube.Intersect(ray3);
        Assert.True(hit3.HasValue);
        Assert.True(HitRecord.CloseEnough(hit3.Value, exp3));

        Assert.False(HitRecord.CloseEnough(hit2.Value, exp1));
        Assert.False(unitCube.Intersect(new Ray(new Point(0.0f, 0.0f, 1.0f), Vec.ZAxis)).HasValue);
    }

    [Fact]
    public void TestAllIntersects()
    {
        var unitCube = new Cube();

        var ray1 = new Ray(new Point(3.0f, 0.5f, 0.5f), -Vec.XAxis);
        var exp1F = new HitRecord(unitCube, new Point(1.0f, 0.5f, 0.5f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.75f, 0.75f), ray1, 2.0f, 1);
        var exp1S = new HitRecord(unitCube, new Point(-1.0f, 0.5f, 0.5f), new Normal(1.0f, 0.0f, 0.0f),
            new Vec2D(0.25f, 0.75f), ray1, 4.0f, 6);
        var allHits1 = unitCube.AllIntersects(ray1);
        Assert.Equal(2, allHits1.Count);
        Assert.True(HitRecord.CloseEnough(allHits1[0], exp1F));
        Assert.True(HitRecord.CloseEnough(allHits1[1], exp1S));


        var ray2 = new Ray(new Point(0.0f, 0.0f, 0.0f), Vec.XAxis);
        var exp2 = new HitRecord(unitCube, new Point(1.0f, 0.0f, 0.0f), new Normal(-1.0f, 0.0f, 0.0f),
            new Vec2D(0.5f, 0.5f), ray2, 1.0f,1);
        var allHits2 = unitCube.AllIntersects(ray2);
        Assert.Single(allHits2);
        Assert.True(HitRecord.CloseEnough(allHits2[0], exp2));
    }
}