namespace Trace.Tests;

public class CamerasTests
{
    // Test Orthogonal Camera
    [Fact]
    public void OrthogonalCameraTest()
    {
        var camera = new OrthogonalCamera(aspectRatio: 2.0f);
        var r1 = camera.FireRay(0.0f, 0.0f);
        var r2 = camera.FireRay(0.0f, 1.0f);
        var r3 = camera.FireRay(1.0f, 0.0f);
        var r4 = camera.FireRay(1.0f, 1.0f);

        Assert.True(Utils.CloseEnough(0.0f, r1.Dir.Cross(r2.Dir).SquaredNorm()));
        Assert.True(Utils.CloseEnough(0.0f, r1.Dir.Cross(r3.Dir).SquaredNorm()));
        Assert.True(Utils.CloseEnough(0.0f, r1.Dir.Cross(r4.Dir).SquaredNorm()));
        Assert.True(Utils.CloseEnough(0.0f, r3.Dir.Cross(r2.Dir).SquaredNorm()));
        Assert.True(Utils.CloseEnough(0.0f, r3.Dir.Cross(r4.Dir).SquaredNorm()));
        Assert.True(Utils.CloseEnough(0.0f, r4.Dir.Cross(r2.Dir).SquaredNorm()));

        Assert.True(Point.CloseEnough(r1.At(1.0f), new Point(0.0f, 2.0f, -1.0f)));
        Assert.True(Point.CloseEnough(r2.At(1.0f), new Point(0.0f, 2.0f, 1.0f)));
        Assert.True(Point.CloseEnough(r3.At(1.0f), new Point(0.0f, -2.0f, -1.0f)));
        Assert.True(Point.CloseEnough(r4.At(1.0f), new Point(0.0f, -2.0f, 1.0f)));
    }

    // Test Perspective Camera
    [Fact]
    public void PerspectiveCameraTest()
    {
        var camera = new PerspectiveCamera(aspectRatio: 2.0f);
        var r1 = camera.FireRay(0.0f, 0.0f);
        var r2 = camera.FireRay(0.0f, 1.0f);
        var r3 = camera.FireRay(1.0f, 0.0f);
        var r4 = camera.FireRay(1.0f, 1.0f);

        Assert.True(Point.CloseEnough(r1.Origin, r2.Origin));
        Assert.True(Point.CloseEnough(r1.Origin, r3.Origin));
        Assert.True(Point.CloseEnough(r1.Origin, r4.Origin));

        Assert.True(Point.CloseEnough(r1.At(1.0f), new Point(0.0f, 2.0f, -1.0f)));
        Assert.True(Point.CloseEnough(r2.At(1.0f), new Point(0.0f, 2.0f, 1.0f)));
        Assert.True(Point.CloseEnough(r3.At(1.0f), new Point(0.0f, -2.0f, -1.0f)));
        Assert.True(Point.CloseEnough(r4.At(1.0f), new Point(0.0f, -2.0f, 1.0f)));
    }

    // Test Orthogonal Observer
    [Fact]
    public void OrthoObserver()
    {
        var camera1 = new OrthogonalCamera(Transformation.Translation(-2.0f * Vec.YAxis), 2.0f);
        var r1 = camera1.FireRay(0.5f, 0.5f);
        var camera2 = new OrthogonalCamera(Transformation.Translation(-2.0f * Vec.YAxis) * Transformation.RotationZ(90),
            2.0f);
        var r2 = camera2.FireRay(0.5f, 0.5f);

        Assert.True(Point.CloseEnough(r1.At(1.0f), new Point(0.0f, -2.0f, 0.0f)));
        Assert.True(Point.CloseEnough(r2.At(1.0f), new Point(0.0f, -2.0f, 0.0f)));
    }

    // Test Perspective Observer
    [Fact]
    public void PerspObserver()
    {
        var camera1 = new PerspectiveCamera(Transformation.Translation(-2.0f * Vec.YAxis), 2.0f);
        var r1 = camera1.FireRay(0.5f, 0.5f);
        var camera2 =
            new PerspectiveCamera(Transformation.Translation(-2.0f * Vec.YAxis) * Transformation.RotationZ(90), 2.0f);
        var r2 = camera2.FireRay(0.5f, 0.5f);

        Assert.True(Point.CloseEnough(r1.At(1.0f), new Point(0.0f, -2.0f, 0.0f)));
        Assert.True(Point.CloseEnough(r2.At(1.0f), new Point(0.0f, -2.0f, 0.0f)));
    }
}