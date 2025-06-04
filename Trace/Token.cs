namespace Trace;

// Class representing a lexical token
public class Token
{
    public SourceLocation Location;
    
    // Constructor
    public Token(SourceLocation location) => Location = location;
}

/// <summary>
/// Token containing a keyword recognized by the lexer.
/// </summary>
public class KeywordToken : Token
{
    public Keyword Keyword;

    // Constructor
    public KeywordToken(Keyword keyword, SourceLocation location) : base(location) => Keyword = keyword;
}

/// <summary>
/// Token containing an identifier.
/// </summary>
public class IdentifierToken : Token
{
    public string Identifier;
    
    // Constructor
    public IdentifierToken(string identifier, SourceLocation location) : base(location) => Identifier = identifier;
}

/// <summary>
/// Token containing a literal string.
/// </summary>
public class LiteralStringToken : Token
{
    public string LiteralString;
    
    // Constructor
    public LiteralStringToken(string literalString, SourceLocation location) : base(location) => LiteralString = literalString;
}

/// <summary>
/// Token containing a literal number.
/// </summary>
public class LiteralNumberToken : Token
{
    public float LiteralNumber;
    
    // Constructor
    public LiteralNumberToken(float literalNumber, SourceLocation location) : base(location) => LiteralNumber = literalNumber;
}

/// <summary>
/// Token containing a symbol.
/// </summary>
public class SymbolToken : Token
{
    public float Symbol;
    
    // Constructor
    public SymbolToken(float symbol, SourceLocation location) : base(location) => Symbol = symbol;
}

/// <summary>
/// Token signalling the end of a file.
/// </summary>
public class StopToken : Token
{
    // Constructor
    public StopToken(SourceLocation location) : base(location){}
}

/// <summary>
/// Enumeration of all the keywords recognized by the lexer.
/// </summary>
public enum Keyword
{
    Material,
    Sphere,
    Plane,
    Diffuse,
    Specular,
    Uniform,
    Checkered,
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
    OnOffRenderer,
    FlatRenderer,
    PathTracer
}