namespace Trace.Tests;

using Xunit;

public class PcgTests
{
    // Test random number generator
    [Fact]
    public void TestRandom()
    {
        var rng = new Pcg();

        Assert.Equal((ulong)1753877967969059832, rng.State);
        Assert.Equal((ulong)109, rng.Inc);

        var expected = new List<uint>
        {
            2707161783, 2068313097, 3122475824,
            2211639955, 3215226955, 3421331566
        };

        foreach (uint exp in expected)
        {
            uint res = rng.Random();
            Assert.Equal(exp, res);
        }
    }
}