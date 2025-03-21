namespace Trace;

using Exceptions;
using System.Text;

public static class Utils
{
    // Checks if two floats are equal with a tolerance epsilon
    public static bool CloseEnough(float a, float b, float epsilon = 1e-5f) => Math.Abs(a - b) < epsilon;

    // Utilities for managing PFM format

    // Read line of ascii characters
    public static string ReadLine(Stream stream)
    {
        using var buffer = new MemoryStream();
        while (true)
        {
            int read = stream.ReadByte();
            if (read is -1 or (byte)'\n') // EOF or newline
                break;

            buffer.WriteByte((byte)read);
        }

        return Encoding.ASCII.GetString(buffer.ToArray());
    }

    // Endianness
    public enum Endianness
    {
        BigEndian = 0,
        LittleEndian = 1
    }

    // Get Endianness
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
        else if (value < 0f)
        {
            return Endianness.LittleEndian;
        }
        else
        {
            throw new InvalidPfmFileFormatException("Invalid endianness: it cannot be zero.");
        }
    }
}