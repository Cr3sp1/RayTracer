namespace Trace;

using Exceptions;

public static class Utils
{
    // Checks if two floats are equal with a tolerance epsilon
    public static bool CloseEnough(float a, float b, float epsilon=1e-5f) => Math.Abs(a - b) < epsilon;
    
    // Utilities for managing PFM format
    
    // Endianness
    public enum Endianness {
        BigEndian = 0,
        LittleEndian = 1
    }
    
    // Get endianness
    public static Endianness ParseEndianness(string line)
    {
        var value = 0f;
        
        try
        {
            value = Convert.ToSingle(line);
        }
        catch
        {
            throw new InvalidPfmFileFormatException();
        }

        if (value > 0f)
        {
            return Endianness.BigEndian;
        }
        else if(value < 0f)
        {
            return Endianness.LittleEndian;
        }
        else
        {
            throw new InvalidPfmFileFormatException("Invalid endianness: it cannot be zero.");
        }
    }
    
}