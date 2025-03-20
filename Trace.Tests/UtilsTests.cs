using System;
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
        Assert.True(Utils.ParseEndianness("1.0")==Utils.Endianness.BigEndian);
        Assert.True(Utils.ParseEndianness("-1.0")==Utils.Endianness.LittleEndian);
    }
}