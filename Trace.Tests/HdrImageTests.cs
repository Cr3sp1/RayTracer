using System;
using Xunit;
using Trace;

namespace Trace.Tests;

public class HdrImageTests
{
    // Test constructor
    [Fact]
    public void TestImageCreation()
    {
        var img = new HdrImage(5, 6);
        Assert.Equal(5, img.Width);
        Assert.Equal(6, img.Height);
    }

    // Test method ValidCoords
    [Fact]
    public void TestValidCoords()
    {
        var img = new HdrImage(5, 6);
        Assert.True(img.ValidCoords(0, 0));
        Assert.True(img.ValidCoords(2, 1));
        Assert.True(img.ValidCoords(4, 5));
        Assert.False(img.ValidCoords(5, 6));
        Assert.False(img.ValidCoords(5, 0));
        Assert.False(img.ValidCoords(0, 6));
        Assert.False(img.ValidCoords(-1, 0));
        Assert.False(img.ValidCoords(0, -1));
    }

    // Test methods SetPixel and GetPixel
    [Fact]
    public void TestGetSetPixel()
    {
        var img = new HdrImage(5, 6);
        var pixel = new Color(1, 2, 3.1f);
        img.SetPixel(0, 1, pixel);
        Assert.Equal(3.1f, img.GetPixel(0, 1).B);
        Assert.Equal(pixel, img.GetPixel(0, 1));
    }
}