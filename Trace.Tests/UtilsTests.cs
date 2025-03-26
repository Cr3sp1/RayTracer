using System;
using System.Text;
using Exceptions;
using Xunit;
using Trace;

namespace Trace.Tests;

using static Utils;
public class UtilsTests
{
    // Test for method CloseEnough between floats
    [Fact]
    public void TestCloseEnough()
    {
        Assert.True(CloseEnough(1f, 1f + 0.9e-5f));
        Assert.True(CloseEnough(100, 101, 2));
        Assert.False(CloseEnough(1f, 1f - 2e-5f));
    }
    
    
    // Tests for methods regarding PFM format
    
    // Test for method ReadLine
    [Fact]
    public void TestReadLine()
    {
        using var stream = new MemoryStream(Encoding.ASCII.GetBytes("hello\nworld"));
        Assert.Equal("hello", ReadLine(stream));
        Assert.Equal("world", ReadLine(stream));
        Assert.Equal("", ReadLine(stream));
    }
    
    // Method ParseEndianness
    [Fact]
    public void TestParseEndianness()
    {
        Assert.True(ParseEndianness("1.0") == Endianness.BigEndian);
        Assert.True(ParseEndianness("-1.0") == Endianness.LittleEndian);
        
        try
        {
            ParseEndianness("0.0");
            Assert.Fail("Expected exception");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }

        try
        {
            ParseEndianness("pippo");
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
        Assert.True(ParseImgSize("3 2") == (3, 2));
        Assert.False(ParseImgSize("1 2") == (3, 2));

        try
        {
            ParseImgSize("1 2 3");
            Assert.Fail("Expected exception for invalid length of line");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }

        try
        {
            ParseImgSize("1");
            Assert.Fail("Expected exception for invalid length of line");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }

        try
        {
            ParseImgSize("-1 1");
            Assert.Fail("Expected exception for negative width or height");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }

        try
        {
            ParseImgSize("pippo pippa");
            Assert.Fail("Expected exception for invalid width or height");
        }
        catch (InvalidPfmFileFormatException)
        {
            Assert.True(true);
        }
    }
}