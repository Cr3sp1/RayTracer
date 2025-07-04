using System.Globalization;

namespace Trace;

/// <summary>
/// Represent a stream from which to read characters and tokens with support for whitespace skipping,
/// comment handling, and tracking source location.
/// </summary>
public class InputStream
{
    private readonly StreamReader _reader;
    public SourceLocation Location;
    public char? SavedChar;
    public Token? SavedToken;
    public SourceLocation LastLocation;
    public int Tab;

    public static string Whitespace = " \t\n\r";
    public static string Symbols = "()<>[],*";

    /// <summary>
    /// Initializes a new instance of the <see cref="InputStream"/> class.
    /// </summary>
    /// <param name="stream">Input stream to read from.</param>
    /// <param name="fileName">Optional source file name for location tracking.</param>
    /// <param name="tab">Tab size used for column calculation.</param>
    public InputStream(Stream stream, string fileName = "", int tab = 8)
    {
        _reader = new StreamReader(stream, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024, leaveOpen: true);

        Location = new SourceLocation(fileName, 1, 1);
        LastLocation = Location;
        Tab = tab;
    }

    /// <summary>
    /// Updates the current source location based on the newly read character.
    /// </summary>
    /// <param name="newChar">The character just read.</param>
    public void UpdatePosition(char? newChar)
    {
        switch (newChar)
        {
            case null:
                break;
            case '\n':
                Location.Line += 1;
                Location.Column = 1;
                break;
            case '\t':
                Location.Column += Tab;
                break;
            default:
                Location.Column += 1;
                break;
        }
    }

    /// <summary>
    /// Reads the next character from the stream, updating location tracking.
    /// </summary>
    /// <returns>The next character, or null if end of stream is reached.</returns>
    public char? ReadChar()
    {
        char newChar;
        if (SavedChar != null)
        {
            newChar = SavedChar.Value;
            SavedChar = null;
        }
        else
        {
            int intChar = _reader.Read();
            if (intChar == -1) return null;
            newChar = (char)intChar;
        }

        LastLocation = Location;
        UpdatePosition(newChar);
        return newChar;
    }

    /// <summary>
    /// Pushes a character back into the stream to be re-read next time.
    /// </summary>
    /// <param name="newChar">The character to unread.</param>
    /// <exception cref="RuntimeException">Thrown if a character is already saved.</exception>
    public void UnreadChar(char newChar)
    {
        if (SavedChar != null)
        {
            throw new RuntimeException("Tried to unread two characters in a row!");
        }

        SavedChar = newChar;
        Location = LastLocation;
    }

    /// <summary>
    /// Pushes a token back into the stream to be re-read next time.
    /// </summary>
    /// <param name="newToken">The token to unread.</param>
    /// <exception cref="RuntimeException">Thrown if a token is already saved.</exception>
    public void UnreadToken(Token newToken)
    {
        if (SavedToken != null)
        {
            throw new RuntimeException("Tried to unread two tokens in a row!");
        }

        SavedToken = newToken;
    }

    /// <summary>
    /// Skips over whitespace and comments starting with '#'.
    /// </summary>
    public void SkipWhitespaceAndComments()
    {
        char? newChar = ReadChar();
        while (newChar.HasValue && (Whitespace.Contains(newChar.Value) || newChar == '#'))
        {
            if (newChar == '#')
            {
                char? skipChar;
                do
                {
                    skipChar = ReadChar();
                } while (skipChar != '\r' && skipChar != '\n' && skipChar.HasValue);
            }

            newChar = ReadChar();
        }

        if (newChar == null) return;
        UnreadChar(newChar.Value);
    }

    /// <summary>
    /// Reads and returns the next token from the stream.
    /// </summary>
    /// <returns>The next parsed token.</returns>
    /// <exception cref="GrammarException">Thrown if the character cannot be parsed into a valid token.</exception>
    public Token ReadToken()
    {
        if (SavedToken != null)
        {
            var result = SavedToken;
            SavedToken = null;
            return result;
        }

        SkipWhitespaceAndComments();

        char? newChar = ReadChar();
        if (newChar == null) return new StopToken(Location);
        char charValue = newChar.Value;
        var tokenLocation = Location;

        if (Symbols.Contains(charValue)) return new SymbolToken(Location, charValue);
        if (charValue == '"') return _ParseLiteralStringToken(Location);
        if (char.IsDigit(charValue) || charValue == '+' || charValue == '-' || charValue == '.')
            return _ParseLiteralNumberToken(charValue.ToString(), Location);
        if (char.IsLetter(charValue) || charValue == '_')
            return _ParseKeywordOrIdentifierToken(charValue.ToString(), Location);

        throw new GrammarException("Invalid character " + charValue, tokenLocation);
    }

    /// <summary>
    /// Parses a quoted string literal from the stream.
    /// </summary>
    /// <param name="tokenLocation">The starting location of the token.</param>
    /// <returns>A literal string token.</returns>
    /// <exception cref="GrammarException">Thrown if the string is unterminated.</exception>
    private LiteralStringToken _ParseLiteralStringToken(SourceLocation tokenLocation)
    {
        string token = "";

        char? newChar = ReadChar();
        while (newChar != '"')
        {
            if (newChar == null) throw new GrammarException("Unterminated literal string!", tokenLocation);
            token += newChar;
            newChar = ReadChar();
        }

        return new LiteralStringToken(tokenLocation, token);
    }

    /// <summary>
    /// Parses a numeric literal from the stream.
    /// </summary>
    /// <param name="firstChar">The first character of the number.</param>
    /// <param name="tokenLocation">The starting location of the token.</param>
    /// <returns>A literal number token.</returns>
    /// <exception cref="GrammarException">Thrown if the numeric value is invalid.</exception>
    private LiteralNumberToken _ParseLiteralNumberToken(string firstChar, SourceLocation tokenLocation)
    {
        string token = firstChar;

        char? newChar = ReadChar();
        bool lastCharIsE = false;
        while (newChar.HasValue && (char.IsDigit(newChar.Value) || newChar is '.' or 'e' or 'E' ||
                                    (lastCharIsE && newChar is '+' or '-')))
        {
            lastCharIsE = newChar is 'e' or 'E';
            token += newChar;
            newChar = ReadChar();
        }

        if (newChar.HasValue) UnreadChar(newChar.Value);

        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out float tokenVal))
        {
            throw new GrammarException($"\"{token}\" is an invalid floating-point number", tokenLocation);
        }

        return new LiteralNumberToken(tokenLocation, tokenVal);
    }

    /// <summary>
    /// Parses either a keyword or identifier token from the stream.
    /// </summary>
    /// <param name="firstChar">The first character of the token.</param>
    /// <param name="tokenLocation">The starting location of the token.</param>
    /// <returns>A keyword token or identifier token.</returns>
    private Token _ParseKeywordOrIdentifierToken(string firstChar, SourceLocation tokenLocation)
    {
        string token = firstChar;

        char? newChar = ReadChar();
        while (newChar.HasValue && (char.IsLetterOrDigit(newChar.Value) || newChar == '_'))
        {
            token += newChar;
            newChar = ReadChar();
        }

        if (newChar.HasValue) UnreadChar(newChar.Value);

        if (KeywordMap.TryGetValue(token, out var keyword))
        {
            return new KeywordToken(tokenLocation, keyword);
        }

        return new IdentifierToken(tokenLocation, token);
    }

    /// <summary>
    /// A case-insensitive dictionary mapping keywords to their enum values.
    /// </summary>
    public static readonly Dictionary<string, Keyword> KeywordMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "new", Keyword.New },
        { "material", Keyword.Material },
        { "plane", Keyword.Plane },
        { "sphere", Keyword.Sphere },
        { "cube", Keyword.Cube },
        { "cylinder", Keyword.Cylinder },
        { "csg", Keyword.Csg },
        { "union", Keyword.Union },
        { "difference", Keyword.Difference },
        { "intersection", Keyword.Intersection },
        { "diffuse", Keyword.Diffuse },
        { "specular", Keyword.Specular },
        { "uniform", Keyword.Uniform },
        { "checkered", Keyword.Checkered },
        { "striped", Keyword.Striped },
        { "vertical", Keyword.Vertical },
        { "horizontal", Keyword.Horizontal },
        { "image", Keyword.Image },
        { "identity", Keyword.Identity },
        { "translation", Keyword.Translation },
        { "rotation_x", Keyword.RotationX },
        { "rotation_y", Keyword.RotationY },
        { "rotation_z", Keyword.RotationZ },
        { "scaling", Keyword.Scaling },
        { "camera", Keyword.Camera },
        { "orthogonal", Keyword.Orthogonal },
        { "perspective", Keyword.Perspective },
        { "float", Keyword.Float },
        { "shape", Keyword.Shape }
    };
}

/// <summary>
/// Represents a location in a source file, including file name, line number, and column number.
/// </summary>
public struct SourceLocation
{
    public string FileName;
    public int Line;
    public int Column;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceLocation"/> struct.
    /// </summary>
    /// <param name="fileName">The name of the source file.</param>
    /// <param name="line">The line number in the source file.</param>
    /// <param name="column">The column number in the source file.</param>
    public SourceLocation(string fileName = "", int line = 0, int column = 0)
    {
        FileName = fileName;
        Line = line;
        Column = column;
    }
}