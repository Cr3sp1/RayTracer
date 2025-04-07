namespace Trace.Tests;

public class NormalTests
{
    // Test Normal constructor and CloseEnough
    [Fact]
    public void TestCloseEnough()
    {
        var n1 = new Normal(1, 2, 3);
        var n2 = new Normal(4, 6, 8);

        Assert.True(Normal.CloseEnough(n1, n1));
        Assert.False(Normal.CloseEnough(n1, n2));
    }

    // Test Vec operations
    [Fact]
    public void TestOperations()
    {
        var n1 = new Normal(1, 2, 3);
        var n2 = new Normal(4, 6, 8);
        var v1 = new Vec(1.0f, 2.0f, 3.0f);
        var v2 = new Vec(4.0f, 6.0f, 8.0f);

        Assert.True(Normal.CloseEnough(-n1, new Normal(-1.0f, -2.0f, -3.0f)));
        Assert.True(Normal.CloseEnough(2 * n1, new Normal(2.0f, 4.0f, 6.0f)));
        
        Assert.True(Utils.CloseEnough(n1.Dot(n2), 40.0f));
        Assert.True(Normal.CloseEnough(n1.Cross(n2), new Normal(-2.0f, 4.0f, -2.0f)));
        Assert.True(Utils.CloseEnough(n1.Dot(v2), 40.0f));
        Assert.True(Vec.CloseEnough(n2.Cross(v1), new Vec(2.0f, -4.0f, 2.0f)));
        
        Assert.True(Utils.CloseEnough(n1.SquaredNorm(), 14.0f));
        Assert.True(Utils.CloseEnough(n1.Norm(), MathF.Sqrt(14.0f)));
    }
}