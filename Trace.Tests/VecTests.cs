namespace Trace.Tests;

public class VecTests
{
    // Test Vec constructor and CloseEnough
    [Fact]
    public void TestCloseEnough()
    {
        var v1 = new Vec(1, 2, 3);
        var v2 = new Vec(4, 6, 8);

        Assert.True(Vec.CloseEnough(v1, v1));
        Assert.False(Vec.CloseEnough(v1, v2));
    }

    // Test Vec operations
    [Fact]
    public void TestOperations()
    {
        var v1 = new Vec(1.0f, 2.0f, 3.0f);
        var v2 = new Vec(4.0f, 6.0f, 8.0f);

        Assert.True(Vec.CloseEnough(-v1, new Vec(-1.0f, -2.0f, -3.0f)));
        Assert.True(Vec.CloseEnough(v1 + v2, new Vec(5.0f, 8.0f, 11.0f)));
        Assert.True(Vec.CloseEnough(v2 - v1, new Vec(3.0f, 4.0f, 5.0f)));
        Assert.True(Vec.CloseEnough(2 * v1, new Vec(2.0f, 4.0f, 6.0f)));

        Assert.True(Utils.CloseEnough(v1.Dot(v2), 40.0f));
        Assert.True(Vec.CloseEnough(v1.Cross(v2), new Vec(-2.0f, 4.0f, -2.0f)));
        Assert.True(Vec.CloseEnough(v2.Cross(v1), new Vec(2.0f, -4.0f, 2.0f)));

        Assert.True(Utils.CloseEnough(v1.SquaredNorm(), 14.0f));
        Assert.True(Utils.CloseEnough(v1.Norm(), MathF.Sqrt(14.0f)));
    }
}