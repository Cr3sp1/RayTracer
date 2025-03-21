using System;
using Xunit;
using Trace;

namespace Trace.Tests;

public class ColorTests
{
    // Test equality between two Colors
    [Fact]
    public void TestCloseEnough()
    {
        Color col1 = new Color(1.0f, 2.0f, 3.0f);

        Assert.True(Color.CloseEnough(new Color(1.0f, 2.0f, 3.0f), col1));
    }

    // Test sum between two Colors
    [Fact]
    public void TestAdd()
    {
        Color col1 = new Color(1.0f, 2.0f, 3.0f);
        Color col2 = new Color(5.0f, 6.0f, 7.0f);

        Assert.True(Color.CloseEnough(new Color(6.0f, 8.0f, 10.0f), col1 + col2));
    }

    // Test product between scalar and Color or Color and Color
    [Fact]
    public void TestProd()
    {
        Color col1 = new Color(1.0f, 2.0f, 3.0f);
        Color col2 = new Color(5.0f, 6.0f, 7.0f);
        float m = 4.0f;

        Assert.True(Color.CloseEnough(new Color(4.0f, 8.0f, 12.0f), m * col1));
        Assert.True(Color.CloseEnough(new Color(5.0f, 12.0f, 21.0f), col1 * col2));
    }
    
    // Test method ToString
    [Fact]
    public void TestToString()
    {
        var col = new Color(1.1f, 2.0f, 3);
        Assert.True("<R:1.1, G:2, B:3>" == col.ToString(),  "<R:1.1, G:2.0, B:3> != " + col.ToString());
    }
}