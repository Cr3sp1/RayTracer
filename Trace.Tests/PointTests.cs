namespace Trace.Tests;

public class PointTests
{
    // Test CloseEnough
    [Fact]
    public void TestCloseEnough()
    {
        Point p1 = new Point(1.0f, 2.0f, 3.0f);
        Point p2 = new Point(2.0f, 3.0f, 4.0f);

        Assert.True(Point.CloseEnough(p1, p1));
        Assert.True(Point.CloseEnough(p2, p2));
        Assert.False(Point.CloseEnough(p1, p2));
    }

    // Test Point operations
    [Fact]
    public void TestOperations()
    {
        Point p1 = new Point(1.0f, 2.0f, 3.0f);
        Point p2 = new Point(2.0f, 3.0f, 4.0f);
        Vec v = new Vec(5.0f, 6.0f, 7.0f);

        Assert.True(Vec.CloseEnough(p1 - p2, new Vec(-1.0f, -1.0f, -1.0f)));
        Assert.False(Vec.CloseEnough(p1 - p2, new Vec(0.0f, -1.0f, -1.0f)));

        Assert.True(Point.CloseEnough(p1 + v, new Point(6.0f, 8.0f, 10.0f)));
        Assert.False(Point.CloseEnough(p2 + v, new Point(6.0f, 8.0f, 10.0f)));

        Assert.True(Point.CloseEnough(p1 - v, new Point(-4.0f, -4.0f, -4.0f)));
        Assert.False(Point.CloseEnough(p2 - v, new Point(6.0f, 8.0f, 10.0f)));

        Assert.True(Point.CloseEnough(3 * p1, new Point(3.0f, 6.0f, 9.0f)));
        Assert.False(Point.CloseEnough(3 * p2, new Point(6.0f, 8.0f, 10.0f)));
    }
}