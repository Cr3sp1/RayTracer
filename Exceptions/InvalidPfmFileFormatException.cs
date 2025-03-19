namespace Exceptions;

using System;

public class InvalidPfmFileFormatException : FormatException
{
    public InvalidPfmFileFormatException() : base("Invalid Pfm file format.")
    {
    }

    public InvalidPfmFileFormatException(string message) : base(message)
    {
    }

    public InvalidPfmFileFormatException(string message, Exception innerException) : base(message, innerException)
    {
    }
}