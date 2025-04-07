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
        output.WriteLine("Point t1*p =\n" + t1*p);
        output.WriteLine("Point t1*p =\n" + t1*p);
        output.WriteLine("Vec t1*v =\n" + t1*v);
        output.WriteLine("Normal t1*w =\n" + t1*w);
        Assert.True(Point.CloseEnough(t1*p, new Point(6, 14, 16)));
        Assert.True(Vec.CloseEnough(t1*v, new Vec(2, 6, 9)));
        Assert.True(Normal.CloseEnough(t1*w, new Normal(5.75f, -4.75f, 2.0f)));
        
        output.WriteLine("Product n*invn =\n" + n*invn);
        output.WriteLine("Composition t1*t2 =\n" + (t1*t2).M);
        Assert.True(HomMat.CloseEnough((t1*t2).M, idMat));

    }
}