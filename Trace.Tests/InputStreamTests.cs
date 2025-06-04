namespace Trace.Tests;

using System.IO;
using System.Text;

public class InputStreamTests
{
    // Test InputStream methods
    [Fact]
    public void TestInputStream()
    {
        var memStream = new MemoryStream();
        var writer = new StreamWriter(memStream, Encoding.UTF8);
        writer.Write("abc   \nd\nef");
        writer.Flush(); // Ensure all data is written to the stream
        memStream.Position = 0; // Reset position for reading

        var stream = new InputStream(memStream);
        Assert.Equal(1, stream.Location.Line);
        Assert.Equal(1, stream.Location.Column);

        Assert.Equal(stream.ReadChar(), 'a');
        Assert.Equal(1, stream.Location.Line);
        Assert.Equal(2, stream.Location.Column);

        stream.UnreadChar('A');
        Assert.Equal(1, stream.Location.Line);
        Assert.Equal(1, stream.Location.Column);

        Assert.Equal(stream.ReadChar(), 'A');
        Assert.Equal(1, stream.Location.Line);
        Assert.Equal(2, stream.Location.Column);

        Assert.Equal(stream.ReadChar(), 'b');
        Assert.Equal(1, stream.Location.Line);
        Assert.Equal(3, stream.Location.Column);

        Assert.Equal(stream.ReadChar(), 'c');
        Assert.Equal(1, stream.Location.Line);
        Assert.Equal(4, stream.Location.Column);

        stream.SkipWhitespaceAndComments();

        Assert.Equal(stream.ReadChar(), 'd');
        Assert.Equal(2, stream.Location.Line);
        Assert.Equal(2, stream.Location.Column);

        Assert.Equal(stream.ReadChar(), '\n');
        Assert.Equal(3, stream.Location.Line);
        Assert.Equal(1, stream.Location.Column);

        Assert.Equal(stream.ReadChar(), 'e');
        Assert.Equal(3, stream.Location.Line);
        Assert.Equal(2, stream.Location.Column);

        Assert.Equal(stream.ReadChar(), 'f');
        Assert.Equal(3, stream.Location.Line);
        Assert.Equal(3, stream.Location.Column);


        Assert.True(stream.ReadChar() == null);
    }
}