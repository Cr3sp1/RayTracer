using System.Diagnostics;
using System.Globalization;
using Exceptions;

namespace Trace;

public class InputStream
{
    private readonly StreamReader Reader;
    public SourceLocation Location;
    public char? SavedChar = null;
    public SourceLocation LastLocation;
    public int Tab;

    public static string Whitespace = " \t\n\r";
    public static string Symbols = "()<>[],*";

    public InputStream(Stream stream, string fileName = "", int tab = 8)
    {
        Reader = new StreamReader(stream, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024, leaveOpen: true);

        Location = new SourceLocation(fileName, 1, 1);
        LastLocation = Location;
        Tab = tab;
    }

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
            int intChar = Reader.Read();
            if (intChar == -1) return null;
            newChar = (char)intChar;
        }

        LastLocation = Location;
        UpdatePosition(newChar);
        return newChar;
    }

    public void UnreadChar(char newChar)
    {
        if (SavedChar != null)
        {
            throw new RuntimeException("Tried to unread two characters in a row!");
        }

        SavedChar = newChar;
        Location = LastLocation;
    }

    public void SkipWhitespaceAndComments()
    {
        char? newChar = ReadChar();
        // Keep going until a non-skippable char
        while (newChar.HasValue && (Whitespace.Contains(newChar.Value) || newChar == '#'))
        {
            // Skip comment line
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

    public Token ReadToken()
    {
        SkipWhitespaceAndComments();

        // Return StopToken if end of file is reached
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

    private LiteralNumberToken _ParseLiteralNumberToken(string firstChar, SourceLocation tokenLocation)
    {
        string token = firstChar;

        char? newChar = ReadChar();
        while (newChar.HasValue && (char.IsDigit(newChar.Value) || newChar == '.' || newChar == 'e' || newChar == 'E'))
        {
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

        // Try to interpret it as a keyword, if it fails interpret it as an identifier
        if (KeywordMap.TryGetValue(token, out var keyword))
        {
            return new KeywordToken(tokenLocation, keyword);
        }

        return new IdentifierToken(tokenLocation, token);
    }

    public static readonly Dictionary<string, Keyword> KeywordMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "new", Keyword.New },
        { "material", Keyword.Material },
        { "plane", Keyword.Plane },
        { "sphere", Keyword.Sphere },
        { "diffuse", Keyword.Diffuse },
        { "specular", Keyword.Specular },
        { "uniform", Keyword.Uniform },
        { "checkered", Keyword.Checkered },
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
        { "float", Keyword.Float }
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

public class GrammarException : Exception
{
    public SourceLocation Location;

    public GrammarException(SourceLocation? location = null) : base("placeholder")
    {
        Location = location ?? new SourceLocation();
    }

    public GrammarException(string message, SourceLocation? location = null) : base(message)
    {
        Location = location ?? new SourceLocation();
    }

    public GrammarException(string message, Exception innerException, SourceLocation? location = null) : base(message,
        innerException)
    {
        Location = location ?? new SourceLocation();
    }
}