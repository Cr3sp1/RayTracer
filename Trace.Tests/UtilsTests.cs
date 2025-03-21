using System;
using Exceptions;
using Xunit;
using Trace;

namespace Trace.Tests;

public class UtilsTests
{
    // Test for method CloseEnough between floats
    [Fact]
    public void TestCloseEnough()
    {
        Assert.True(Utils.CloseEnough(1f, 1f + 0.9e-5f));
        Assert.True(Utils.CloseEnough(100, 101, 2));
        Assert.False(Utils.CloseEnough(1f, 1f - 2e-5f));
    }

    // Tests for methods regarding PFM format

    // Method ParseEndianness
    [Fact]
    public void TestParseEndianness()
    {
        Assert.True(Utils.ParseEndianness("1.0") == Utils.Endianness.BigEndian);
        Assert.True(Utils.ParseEndianness("-1.0") == Utils.Endianness.LittleEndian);

        /*try
        {
            var e = new Utils.Endianness();
            e = Utils.ParseEndianness("0.0");
            Assert.Fail("Expected exception");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }*/

        try
        {
            var en = new Utils.Endianness();
            en = Utils.ParseEndianness("pippo");
            Assert.Fail("Expected exception");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }
    }

    // Method ParseImgSize
    [Fact]
    public void TestParseImgSize()
    {
        Assert.True(Utils.ParseImgSize("3 2") == (3, 2));
        Assert.False(Utils.ParseImgSize("1 2") == (3, 2));

        int width, height;
        try
        {
            (width, height) = Utils.ParseImgSize("1 2 3");
            Assert.Fail("Expected exception for invalid length of line");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }

        try
        {
            (width, height) = Utils.ParseImgSize("1");
            Assert.Fail("Expected exception for invalid length of line");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }

        try
        {
            (width, height) = Utils.ParseImgSize("-1 1");
            Assert.Fail("Expected exception for negative width or height");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }

        try
        {
            (width, height) = Utils.ParseImgSize("pippo pippa");
            Assert.Fail("Expected exception for invalid width or height");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }
    }
}