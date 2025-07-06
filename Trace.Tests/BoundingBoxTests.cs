namespace Trace.Tests;

public class BoundingBoxTests
{
    [Fact]
    public void TestIntersects()
    {
        var bbox = new BoundingBox(-1f, -2f, -3f, 1f, 2f, 3f);

        var ray1 = new Ray(new Point(0f, 1.5f, -2.5f), -Vec.ZAxis);
        Assert.True(bbox.DoesIntersect(ray1));

        var ray2 = new Ray(new Point(0f, 1.5f, 6f), -Vec.ZAxis)
        {
            TMax = 2f
        };
        Assert.False(bbox.DoesIntersect(ray2));

        var ray3 = new Ray(new Point(-2.5f, 1.5f, 0f), -Vec.ZAxis);
        Assert.False(bbox.DoesIntersect(ray3));

        var ray4 = new Ray(new Point(-2.5f, 3f, 0f), -Vec.ZAxis);
        Assert.False(bbox.DoesIntersect(ray4));

        var ray5 = new Ray(new Point(0f, 1.5f, 3.5f), Vec.ZAxis);
        Assert.False(bbox.DoesIntersect(ray5));

        var ray6 = new Ray(new Point(-2f, 0f, 0f), new Vec(1f, 1f, 1.1f));
        Assert.True(bbox.DoesIntersect(ray6));

        var ray7 = new Ray(new Point(-2f, 0f, 0f), new Vec(1f, 2.1f, 1.1f));
        Assert.False(bbox.DoesIntersect(ray7));
    }

    [Fact]
    public void TestTransformation()
    {
        var bbox = new BoundingBox(-1f, -2f, -3f, 1f, 2f, 3f);

        var trans = Transformation.Translation(new Vec(3f, 2f, 1f));
        var transBox = trans * bbox;
        var transBoxExp = new BoundingBox(2f, 0f, -2f, 4f, 4f, 4f);
        Assert.True(BoundingBox.CloseEnough(transBox, transBoxExp));

        var rot = Transformation.RotationX(90) * Transformation.RotationY(90);
        var rotBox = rot * bbox;
        var rotBoxExp = new BoundingBox(-3f, -1f, -2f, 3f, 1f, 2f);
        Assert.True(BoundingBox.CloseEnough(rotBoxExp, rotBox));
        Assert.False(BoundingBox.CloseEnough(transBox, rotBox));

        var scal = Transformation.Scaling(new Vec(2f, 1f, 0.5f));
        var scalBox = scal * bbox;
        var scalBoxExp = new BoundingBox(-2f, -2f, -1.5f, 2f, 2f, 1.5f);
        Assert.True(BoundingBox.CloseEnough(scalBoxExp, scalBox));
    }

    [Fact]
    public void TestGetBoundingBox()
    {
        var sphereA = new Sphere();
        var sphereB = new Sphere(Transformation.Translation(new Vec(2f, 3f, 4f)));
        var sphereC = new Sphere(Transformation.Translation(new Vec(-1.5f, 0f, 7f)));
        var plane = new Plane();

        var sphereBBox = sphereB.GetBoundingBox();
        Assert.True(sphereBBox.HasValue);
        var sphereBBoxExp = new BoundingBox(1f, 2f, 3f, 3f, 4f, 5f);
        Assert.True(BoundingBox.CloseEnough(sphereBBoxExp, sphereBBox.Value));

        Assert.Null(plane.GetBoundingBox());

        var csg1 = new Csg(sphereA, sphereB, CsgType.Fusion);
        var csg1BBox = csg1.GetBoundingBox();
        Assert.True(csg1BBox.HasValue);
        var csg1BBoxExp = new BoundingBox(-1f, -1f, -1f, 3f, 4f, 5f);
        Assert.True(BoundingBox.CloseEnough(csg1BBoxExp, csg1BBox.Value));

        var csg2 = new Csg(csg1, sphereC, CsgType.Intersection);
        var csg2BBox = csg2.BBox;
        Assert.True(csg2BBox.HasValue);
        var csg2BBoxExp = new BoundingBox(-1f, -1f, 6f, -0.5f, 1f, 6f);
        Assert.True(BoundingBox.CloseEnough(csg2BBoxExp, csg2BBox.Value));
        
        var csg3 = new Csg(sphereB, plane, CsgType.Union);
        Assert.Null(csg3.GetBoundingBox());
        
        var t = Transformation.Translation(new Vec(2f, 3f, 4f)) *
                Transformation.Scaling(new Vec(1.5f, -1.5f, 3f)) * Transformation.RotationZ(4);
        var csg4 = new Csg(sphereB, plane, CsgType.Difference, t);
        var csg4BBox = csg4.BBox;
        Assert.True(csg4BBox.HasValue);
        var csg4BBoxExp = t * sphereBBox.Value;
        Assert.True(BoundingBox.CloseEnough(csg4BBoxExp, csg4BBox.Value));
        
        var csg5 = new Csg(sphereB, plane, CsgType.Intersection, t);
        var csg5BBox = csg5.BBox;
        Assert.True(csg5BBox.HasValue);
        var csg5BBoxExp = t * sphereBBox.Value;
        Assert.True(BoundingBox.CloseEnough(csg5BBoxExp, csg5BBox.Value));
    }
}