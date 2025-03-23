using System;
using System.IO;
using Exceptions;
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

    // Test ReadPfm method
    [Fact]
    public void TestReadPfm()
    {
        string leFilePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "reference_le.pfm");
        byte[] leReferenceBytes = File.ReadAllBytes(leFilePath);

        string beFilePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "reference_be.pfm");
        byte[] beReferenceBytes = File.ReadAllBytes(beFilePath);

        using var leMs = new MemoryStream(leReferenceBytes);
        var leTestImg = new HdrImage(leMs);

        Assert.True(leTestImg.Width == 3);
        Assert.True(leTestImg.Height == 2);

        Assert.True(Color.CloseEnough(leTestImg.GetPixel(0, 0), new Color((float)1.0e1, (float)2.0e1, (float)3.0e1)));
        Assert.True(Color.CloseEnough(leTestImg.GetPixel(1, 0), new Color((float)4.0e1, (float)5.0e1, (float)6.0e1)));
        Assert.True(Color.CloseEnough(leTestImg.GetPixel(2, 0), new Color((float)7.0e1, (float)8.0e1, (float)9.0e1)));
        Assert.True(Color.CloseEnough(leTestImg.GetPixel(0, 1), new Color((float)1.0e2, (float)2.0e2, (float)3.0e2)));
        Assert.True(Color.CloseEnough(leTestImg.GetPixel(0, 0), new Color((float)1.0e1, (float)2.0e1, (float)3.0e1)));
        Assert.True(Color.CloseEnough(leTestImg.GetPixel(1, 1), new Color((float)4.0e2, (float)5.0e2, (float)6.0e2)));
        Assert.True(Color.CloseEnough(leTestImg.GetPixel(2, 1), new Color((float)7.0e2, (float)8.0e2, (float)9.0e2)));

        using var beMs = new MemoryStream(beReferenceBytes);
        var beTestImg = new HdrImage(beMs);

        Assert.True(leTestImg.Width == 3);
        Assert.True(leTestImg.Height == 2);

        Assert.True(Color.CloseEnough(beTestImg.GetPixel(0, 0), new Color((float)1.0e1, (float)2.0e1, (float)3.0e1)));
        Assert.True(Color.CloseEnough(beTestImg.GetPixel(1, 0), new Color((float)4.0e1, (float)5.0e1, (float)6.0e1)));
        Assert.True(Color.CloseEnough(beTestImg.GetPixel(2, 0), new Color((float)7.0e1, (float)8.0e1, (float)9.0e1)));
        Assert.True(Color.CloseEnough(beTestImg.GetPixel(0, 1), new Color((float)1.0e2, (float)2.0e2, (float)3.0e2)));
        Assert.True(Color.CloseEnough(beTestImg.GetPixel(0, 0), new Color((float)1.0e1, (float)2.0e1, (float)3.0e1)));
        Assert.True(Color.CloseEnough(beTestImg.GetPixel(1, 1), new Color((float)4.0e2, (float)5.0e2, (float)6.0e2)));
        Assert.True(Color.CloseEnough(beTestImg.GetPixel(2, 1), new Color((float)7.0e2, (float)8.0e2, (float)9.0e2)));

        byte[] pfmWrong = System.Text.Encoding.ASCII.GetBytes("PF\n3 2\n-1.0\nstop");
        using var wrongMs = new MemoryStream(pfmWrong);
        try
        {
            var wrongTestImg = new HdrImage(wrongMs);
            Assert.Fail("Expected exception");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }
    }

    // Test WritePfm method
    [Fact]
    public void TestWritePfm()
    {
        var img = new HdrImage(2, 3);
        var pixel = new Color(1, 2, 3.1f);
        img.SetPixel(0, 1, pixel);

        using var leStream = new MemoryStream();
        img.WritePfm(leStream);
        leStream.Seek(0, SeekOrigin.Begin);     // Resets stream position
        var leImage = new HdrImage(leStream);
        Assert.Equal(img, leImage);
        
        using var beStream = new MemoryStream();
        img.WritePfm(beStream);
        beStream.Seek(0, SeekOrigin.Begin);
        var beImage = new HdrImage(beStream);
        Assert.Equal(img, beImage);
    }
}