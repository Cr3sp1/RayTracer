namespace Trace.Tests;
using Xunit;
using Xunit.Abstractions;

public class HomMatTests
{
    private readonly ITestOutputHelper output;

    public HomMatTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    
    // Test HomMat constructor and CloseEnough
    [Fact]
    public void TestCloseEnough()
    {
        var n1 = new HomMat([1.0f, 2.0f, 3.0f, 4.0f],
            [5.0f, 6.0f, 7.0f, 8.0f],
            [9.0f, 9.0f, 8.0f, 7.0f]);
        var n2 = new HomMat([5.0f, 6.0f, 7.0f, 8.0f],
            [9.0f, 9.0f, 8.0f, 7.0f],
            [1.0f, 2.0f, 3.0f, 4.0f]);
        var idMat = new HomMat([1.0f, 0.0f, 0.0f, 0f],
            [0.0f, 1.0f, 0.0f, 0f],
            [0.0f, 0.0f, 1.0f, 0f]);
        
        Assert.True(HomMat.CloseEnough(n1, n1));
        Assert.False(HomMat.CloseEnough(n1, n2));
        Assert.True(HomMat.CloseEnough(idMat, new HomMat()));
    }
    
    // Test HomMat product
    [Fact]
    public void TestOperations()
    {
        var n = new HomMat([1.0f, 2.0f, 3.0f, 4.0f],
            [5.0f, 6.0f, 7.0f, 8.0f],
            [9.0f, 9.0f, 8.0f, 7.0f]);
        var nInv = new HomMat([-3.75f, 2.75f, -1.0f, 0.0f],
            [5.75f, -4.75f, 2.0f, 1.0f],
            [-2.25f, 2.25f, -1.0f, -2.0f]);
        var idMat = new HomMat();
        
        output.WriteLine("Matrix A =\n" + n);
        output.WriteLine("Matrix B =\n" + nInv);
        output.WriteLine("Matrix A*B =\n" + (n * nInv));
        output.WriteLine("Matrix A*B should be equal to:\n" + idMat);
        Assert.True(HomMat.CloseEnough(n*nInv, idMat));
    }
}

