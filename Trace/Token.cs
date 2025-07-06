namespace Trace;

// Class representing a lexical token
public class Token
{
    public SourceLocation Location;

    // Constructor
    public Token(SourceLocation location) => Location = location;

    public override string ToString() => $"base Token at line {Location.Line}, column {Location.Column}";
}

/// <summary>
/// Token containing a keyword recognized by the lexer.
/// </summary>
public class KeywordToken : Token
{
    public Keyword Keyword;

    // Constructor
    public KeywordToken(SourceLocation location, Keyword keyword) : base(location) => Keyword = keyword;

    public override string ToString() => $"keyword \'{Keyword}\' at line {Location.Line}, column {Location.Column}";
}

/// <summary>
/// Token containing an identifier.
/// </summary>
public class IdentifierToken : Token
{
    public string Identifier;

    // Constructor
    public IdentifierToken(SourceLocation location, string identifier) : base(location) => Identifier = identifier;

    public override string ToString() =>
        $"identifier \'{Identifier}\' at line {Location.Line}, column {Location.Column}";
}

/// <summary>
/// Token containing a literal string.
/// </summary>
public class LiteralStringToken : Token
{
    public string String;

    // Constructor
    public LiteralStringToken(SourceLocation location, string @string) : base(location) =>
        String = @string;

    public override string ToString() => $"string \'{String}\' at line {Location.Line}, column {Location.Column}";
}

/// <summary>
/// Token containing the value of a literal number.
/// </summary>
public class LiteralNumberToken : Token
{
    public float Value;

    // Constructor
    public LiteralNumberToken(SourceLocation location, float value) : base(location) =>
        Value = value;

    public override string ToString() => $"number \'{Value}\' at line {Location.Line}, column {Location.Column}";
}

/// <summary>
/// Token containing a symbol.
/// </summary>
public class SymbolToken : Token
{
    public char Symbol;

    // Constructor
    public SymbolToken(SourceLocation location, char symbol) : base(location) => Symbol = symbol;

    public override string ToString() => $"symbol \'{Symbol}\' at line {Location.Line}, column {Location.Column}";
}

/// <summary>
/// Token signaling the end of a file.
/// </summary>
public class StopToken : Token
{
    // Constructor
    public StopToken(SourceLocation location) : base(location)
    {
    }

    public override string ToString() => $"end of file at line {Location.Line}, column {Location.Column}";
}

/// <summary>
/// Enumeration of all the keywords recognized by the lexer.
/// </summary>
public enum Keyword
{
    New,
    Material,
    Plane,
    Sphere,
    Cube,
    Cylinder,
    Csg,
    Fusion,
    Difference,
    Intersection,
    Diffuse,
    Specular,
    Uniform,
    Checkered,
    Striped,
    Vertical,
    Horizontal,
    Image,
    Identity,
    Translation,
    RotationX,
    RotationY,
    RotationZ,
    Scaling,
    Camera,
    Orthogonal,
    Perspective,
    Float,
    Shape
}