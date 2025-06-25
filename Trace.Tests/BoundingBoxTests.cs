namespace Trace.Tests;

public class BoundingBoxTests
{
    [Fact]
    public void TestIntersects()
    {
        var bbox = new BoundingBox(-1f, -2f, -3f, 1f, 2f, 3f);
        
        var ray1 = new Ray(new Point(0f, 1.5f, -2.5f), -Vec.ZAxis);
        Assert.True(bbox.Intersects(ray1));
        
        var ray2 =  new Ray(new Point(0f, 1.5f, 6f), -Vec.ZAxis)
        {
            TMax = 2f
        };
        Assert.False(bbox.Intersects(ray2));
        
        var ray3 = new Ray(new Point(-2.5f, 1.5f, 0f), -Vec.ZAxis);
        Assert.False(bbox.Intersects(ray3));
        
        var ray4 = new Ray(new Point(-2.5f, 3f, 0f), -Vec.ZAxis);
        Assert.False(bbox.Intersects(ray4));
        
        var ray5 = new Ray(new Point(0f, 1.5f, 3.5f), Vec.ZAxis);
        Assert.False(bbox.Intersects(ray5));
        
        var ray6 = new Ray(new Point(-2f, 0f, 0f), new Vec(1f, 1f, 1.1f));
        Assert.True(bbox.Intersects(ray6));
        
        var ray7 = new Ray(new Point(-2f, 0f, 0f), new Vec(1f, 2.1f, 1.1f));
        Assert.False(bbox.Intersects(ray7));
    }
}