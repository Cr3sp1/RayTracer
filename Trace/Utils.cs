namespace Trace;

using Exceptions;
using System.Text;

public static class Utils
{
    /// <summary>
    /// Check if two <c>float</c> have the same values with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(float a, float b, float epsilon = 1e-5f) => Math.Abs(a - b) < epsilon;

    // Utilities for managing PFM format

    /// <summary>
    /// Read line of ascii characters from a <c>Stream</c>.
    /// </summary>
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

    // Write float to file in binary
    public static void WriteFloat(Stream outStream, float value, Endianness endianness = Endianness.LittleEndian)
    {
        var valueBytes = BitConverter.GetBytes(value);

        // Invert byte order if needed
        var converterEndianness = BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;
        if (converterEndianness != endianness) Array.Reverse(valueBytes);

        outStream.Write(valueBytes, 0, valueBytes.Length);
    }
}