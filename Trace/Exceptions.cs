namespace Trace;

/// <summary>
/// Exception thrown when a Pfm file is improperly formatted.
/// </summary>
public class InvalidPfmFileFormatException : FormatException
{
    /// <summary>
    /// Initializes a new instance with a default error message.
    /// </summary>
    public InvalidPfmFileFormatException() : base("Invalid Pfm file format.")
    {
    }

    /// <summary>
    /// Initializes a new instance with a custom error message.
    /// </summary>
    public InvalidPfmFileFormatException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a custom error message and inner exception.
    /// </summary>
    public InvalidPfmFileFormatException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// General-purpose exception used for runtime errors in the application.
/// </summary>
public class RuntimeException : Exception
{
    /// <summary>
    /// Initializes a new instance with a default runtime error message.
    /// </summary>
    public RuntimeException() : base("Runtime Error.")
    {
    }

    /// <summary>
    /// Initializes a new instance with a custom error message.
    /// </summary>
    public RuntimeException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a custom error message and inner exception.
    /// </summary>
    public RuntimeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown during parsing when encountering incorrect grammar.
/// </summary>
public class GrammarException : Exception
{
    /// <summary>
    /// The location in the source where the error occurred.
    /// </summary>
    public SourceLocation Location;

    /// <summary>
    /// Initializes a new instance with a default error message and optional source location.
    /// </summary>
    public GrammarException(SourceLocation? location = null) : base("placeholder")
    {
        Location = location ?? new SourceLocation();
    }

    /// <summary>
    /// Initializes a new instance with a custom error message and optional source location.
    /// </summary>
    public GrammarException(string message, SourceLocation? location = null) : base(message)
    {
        Location = location ?? new SourceLocation();
    }

    /// <summary>
    /// Initializes a new instance with a custom message, inner exception, and optional source location.
    /// </summary>
    public GrammarException(string message, Exception innerException, SourceLocation? location = null) : base(message,
        innerException)
    {
        Location = location ?? new SourceLocation();
    }
}