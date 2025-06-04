using System.Diagnostics;
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