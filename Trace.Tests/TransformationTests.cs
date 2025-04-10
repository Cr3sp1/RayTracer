namespace Trace.Tests;

using Xunit;
using Xunit.Abstractions;

public class TransformationTests
{
    private readonly ITestOutputHelper output;

    public TransformationTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    // Test transformation constructor and CloseEnough
    [Fact]
    public void TestCloseEnough()
    {
        var n = new HomMat([1.0f, 2.0f, 3.0f, 4.0f],
            [5.0f, 6.0f, 7.0f, 8.0f],
            [9.0f, 9.0f, 8.0f, 7.0f]);
        var invn = new HomMat([-3.75f, 2.75f, -1.0f, 0.0f],
            [5.75f, -4.75f, 2.0f, 1.0f],
            [-2.25f, 2.25f, -1.0f, -2.0f]);
        var idMat = new HomMat([1.0f, 0.0f, 0.0f, 0f],
            [0.0f, 1.0f, 0.0f, 0f],
            [0.0f, 0.0f, 1.0f, 0f]);
        var t1 = new Transformation(n, invn);
        var t2 = new Transformation();
        var t3 = new Transformation(idMat, idMat);

        Assert.True(t1.IsConsistent());
        Assert.True(t2.IsConsistent());
        Assert.True(t3.IsConsistent());
        Assert.True(HomMat.CloseEnough(t2.M, t3.M));
        Assert.True(HomMat.CloseEnough(t1.M, n));
        Assert.False(HomMat.CloseEnough(t1.M, t2.M));
        Assert.True(HomMat.CloseEnough(idMat, t2.InvM));
        Assert.True(Transformation.CloseEnough(t2, t3));
        Assert.False(Transformation.CloseEnough(t1, t3));
    }

    // Test operations
    [Fact]
    public void TestOperations()
    {
        var n = new HomMat([1.0f, 2.0f, 3.0f, 4.0f],
            [5.0f, 6.0f, 7.0f, 8.0f],
            [9.0f, 9.0f, 8.0f, 7.0f]);
        var invn = new HomMat([-3.75f, 2.75f, -1.0f, 0.0f],
            [5.75f, -4.75f, 2.0f, 1.0f],
            [-2.25f, 2.25f, -1.0f, -2.0f]);
        var idMat = new HomMat([1.0f, 0.0f, 0.0f, 0f],
            [0.0f, 1.0f, 0.0f, 0f],
            [0.0f, 0.0f, 1.0f, 0f]);

        var t1 = new Transformation(n, invn);
        var t2 = new Transformation(invn, n);

        Assert.True(HomMat.CloseEnough(t1.Inverse().M, invn));

        var p = new Point(0, 1, 0);
        var v = new Vec(0, 1, 0);
        var w = new Normal(0, 1, 0);

        output.WriteLine("Matrix M of t1 =\n" + t1.M);
        output.WriteLine("Matrix M of t2 =\n" + t2.M);
        output.WriteLine("Point t1*p =\n" + t1 * p);
        output.WriteLine("Point t1*p =\n" + t1 * p);
        output.WriteLine("Vec t1*v =\n" + t1 * v);
        output.WriteLine("Normal t1*w =\n" + t1 * w);
        Assert.True(Point.CloseEnough(t1 * p, new Point(6, 14, 16)));
        Assert.True(Vec.CloseEnough(t1 * v, new Vec(2, 6, 9)));
        Assert.True(Normal.CloseEnough(t1 * w, new Normal(5.75f, -4.75f, 2.0f)));

        output.WriteLine("Product n*invn =\n" + n * invn);
        output.WriteLine("Composition t1*t2 =\n" + (t1 * t2).M);
        Assert.True(HomMat.CloseEnough((t1 * t2).M, idMat));
    }

    // Test Translation
    [Fact]
    public void TestTranslation()
    {
        var tr1 = Transformation.Translation(new Vec(1.0f, 2.0f, 3.0f));
        Assert.True(tr1.IsConsistent());

        var tr2 = Transformation.Translation(new Vec(4.0f, 6.0f, 8.0f));
        Assert.True(tr2.IsConsistent());

        var prod = tr1 * tr2;
        Assert.True(prod.IsConsistent());
        var expected = Transformation.Translation(new Vec(5.0f, 8.0f, 11.0f));
        Assert.True(Transformation.CloseEnough(prod, expected));

        var p0 = new Point(1.0f, 2.0f, 3.0f);
        Assert.True(Point.CloseEnough(tr2 * p0, new Point(5.0f, 8.0f, 11.0f)));
    }

    // Test Scaling
    [Fact]
    public void TestScaling()
    {
        var tr1 = Transformation.Scaling(new Vec(2.0f, 5.0f, 10.0f));
        Assert.True(tr1.IsConsistent());

        var tr2 = Transformation.Scaling(new Vec(3.0f, 2.0f, 4.0f));
        Assert.True(tr2.IsConsistent());

        var prod = tr1 * tr2;
        Assert.True(prod.IsConsistent());
        var expected = Transformation.Scaling(new Vec(6.0f, 10.0f, 40.0f));
        Assert.True(Transformation.CloseEnough(prod, expected));

        var v0 = new Vec(2.0f, 5.0f, 10.0f);
        Assert.True(Vec.CloseEnough(tr2 * v0, new Vec(6.0f, 10.0f, 40.0f)));
    }

    // Test RotationX, RotationY, RotationZ
    [Fact]
    public void TestRotations()
    {
        Assert.True(Transformation.RotationX(0.1f).IsConsistent());
        Assert.True(Transformation.RotationY(0.1f).IsConsistent());
        Assert.True(Transformation.RotationZ(0.1f).IsConsistent());

        Assert.True(
            Vec.CloseEnough(Transformation.RotationX(90) * Vec.YAxis, Vec.ZAxis));
        Assert.True(
            Vec.CloseEnough(Transformation.RotationY(90) * Vec.ZAxis, Vec.XAxis));
        Assert.True(
            Vec.CloseEnough(Transformation.RotationZ(90) * Vec.XAxis, Vec.YAxis));
    }
}