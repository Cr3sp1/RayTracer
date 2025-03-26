namespace Exceptions;

using System;

public class RuntimeException : Exception
{
    public RuntimeException() : base("Runtime Error.")
    {
    }

    public RuntimeException(string message) : base(message)
    {
    }

    public RuntimeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}