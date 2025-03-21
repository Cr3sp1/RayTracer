namespace Trace;

using Exceptions;

public static class Utils
{
    // Checks if two floats are equal with a tolerance epsilon
    public static bool CloseEnough(float a, float b, float epsilon = 1e-5f) => Math.Abs(a - b) < epsilon;

    // Utilities for managing PFM format

    // Endianness
    public enum Endianness
    {
        BigEndian = 0,
        LittleEndian = 1
    }

    // Read endianness
    public static Endianness ParseEndianness(string line)
    {
        var value = 0f;

        try
        {
            value = Convert.ToSingle(line);
        }
        catch (FormatException)
        {
            throw new InvalidPfmFileFormatException("No endianness found.");
        }

        if (value > 0f)
        {
            return Endianness.BigEndian;
        }
        else if (value < 0f)
        {
            return Endianness.LittleEndian;
        }
        else
        {
            throw new InvalidPfmFileFormatException("Invalid endianness: it cannot be zero.");
        }
    }

    // Read width and height of the image
    public static (int, int) ParseImgSize(string line)
    {
        int width, height = 0;
        string[] elements = line.Split(' ');
        if (elements.Length != 2)
        {
            throw new InvalidPfmFileFormatException("No image size found.");
        }

        try
        {
            width = Convert.ToInt32(elements[0]);
            height = Convert.ToInt32(elements[1]);
            if (width < 0 || height < 0)
            {
                throw new InvalidPfmFileFormatException("Invalid image siz: cannot be negative.");
            }
        }
        catch (FormatException)
        {
            throw new InvalidPfmFileFormatException("Invalid image size");
        }

        return (width, height);
    }
}