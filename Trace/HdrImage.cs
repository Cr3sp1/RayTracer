namespace Trace;

using Exceptions;
using System.Text;
using static Utils;

public class HdrImage
{
    // HdrImage fields
    public readonly int Width, Height;
    public Color[] Pixels;

    // Constructor
    public HdrImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new Color[width * height];
    }

    // Get a pixel
    public Color GetPixel(int x, int y)
    {
        if (!ValidCoords(x, y))
            throw new ArgumentOutOfRangeException(nameof(x), "Invalid coordinates");

        return Pixels[_PixelOffset(x, y)];
    }

    // Set a pixel
    public void SetPixel(int x, int y, Color c)
    {
        if (!ValidCoords(x, y))
            throw new ArgumentOutOfRangeException(nameof(x), "Invalid coordinates");

        Pixels[_PixelOffset(x, y)] = c;
    }

    // Check validity of coordinates
    public bool ValidCoords(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    // Get array index corresponding to coordinates
    public int _PixelOffset(int x, int y) => y * Width + x;

    // Method to read floats from file to be used exclusively in HdrImage.ReadPfmImage!
    private static float _ReadFloat(Stream stream, Endianness endianness = Endianness.LittleEndian)
    {
        var buffer = new byte[4];

        try
        {
            stream.ReadExactly(buffer, 0, 4);

            // Invert byte order if needed
            var converterEndianness = BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;
            if (converterEndianness != endianness) Array.Reverse(buffer);

            return BitConverter.ToSingle(buffer, 0);
        }
        catch
        {
            throw new InvalidPfmFileFormatException("Impossible to read binary data from the file");
        }
    }


    // Write HdrImage to pfm file
    public void WritePfm(Stream outStream, Endianness endianness = Endianness.LittleEndian)
    {
        // Write the header
        var endianStr = endianness == Endianness.LittleEndian ? "-1.0" : "1.0";
        var header = Encoding.ASCII.GetBytes($"PF\n{Width} {Height}\n{endianStr}\n");
        outStream.Write(header, 0, header.Length);
        
        // Write the image (bottom-to-up, left-to-right)
        for (var y = Height - 1; y >= 0; y--)
        {
            for (var x = 0; x < Width; x++)
            {
                var color = Pixels[_PixelOffset(x, y)];
                WriteFloat( outStream, color.R, endianness);
                WriteFloat( outStream, color.G, endianness);
                WriteFloat( outStream, color.B, endianness);
            }
        }
    }
}