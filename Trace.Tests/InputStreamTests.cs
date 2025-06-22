namespace Trace.Tests;

using System.IO;
using System.Text;

public class InputStreamTests
{
    // Test InputStream char reading
    [Fact]
    public void TestInputReading()
    {
        var input = "abc   \nd\nef";
        var stream = new InputStream(new MemoryStream(Encoding.UTF8.GetBytes(input)));

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

    // Test lexer
    [Fact]
    public void TestLexer()
    {
        var input = """
                    # This is a comment
                    new material sky_material(
                        diffuse(image("my file.pfm")),
                        <5.1, 5.2e+3, 5.3e-2>
                    )
                    """;
        var stream = new InputStream(new MemoryStream(Encoding.UTF8.GetBytes(input)));

        _AssertIsKeyword(stream.ReadToken(), Keyword.New);
        _AssertIsKeyword(stream.ReadToken(), Keyword.Material);
        _AssertIsIdentifier(stream.ReadToken(), "sky_material");
        _AssertIsSymbol(stream.ReadToken(), '(');
        _AssertIsKeyword(stream.ReadToken(), Keyword.Diffuse);
        _AssertIsSymbol(stream.ReadToken(), '(');
        _AssertIsKeyword(stream.ReadToken(), Keyword.Image);
        _AssertIsSymbol(stream.ReadToken(), '(');
        _AssertIsString(stream.ReadToken(), "my file.pfm");
        _AssertIsSymbol(stream.ReadToken(), ')');
        _AssertIsSymbol(stream.ReadToken(), ')');
        _AssertIsSymbol(stream.ReadToken(), ',');
        _AssertIsSymbol(stream.ReadToken(), '<');
        _AssertIsNumber(stream.ReadToken(), 5.1f);
        _AssertIsSymbol(stream.ReadToken(), ',');
        _AssertIsNumber(stream.ReadToken(), 5200f);
        _AssertIsSymbol(stream.ReadToken(), ',');
        _AssertIsNumber(stream.ReadToken(), 0.053f);
        _AssertIsSymbol(stream.ReadToken(), '>');
        _AssertIsSymbol(stream.ReadToken(), ')');
        Assert.IsType<StopToken>(stream.ReadToken());
    }

    // Support functions for TestLexer
    private void _AssertIsKeyword(Token token, Keyword expectedKeyword)
    {
        Assert.IsType<KeywordToken>(token);
        Assert.Equal(expectedKeyword, ((KeywordToken)token).Keyword);
    }

    private void _AssertIsIdentifier(Token token, string expectedIdentifier)
    {
        Assert.IsType<IdentifierToken>(token);
        Assert.Equal(expectedIdentifier, ((IdentifierToken)token).Identifier);
    }

    private void _AssertIsSymbol(Token token, char expectedSymbol)
    {
        Assert.IsType<SymbolToken>(token);
        Assert.Equal(expectedSymbol, ((SymbolToken)token).Symbol);
    }

    private void _AssertIsNumber(Token token, float expectedValue)
    {
        Assert.IsType<LiteralNumberToken>(token);
        Assert.Equal(expectedValue, ((LiteralNumberToken)token).Value);
    }

    private void _AssertIsString(Token token, string expectedString)
    {
        Assert.IsType<LiteralStringToken>(token);
        Assert.Equal(expectedString, ((LiteralStringToken)token).String);
    }
}