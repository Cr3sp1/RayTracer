﻿namespace Trace;

using Exceptions;
using System.Text;
using static Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class HdrImage
{
    // HdrImage fields
    public int Width;
    public int Height;
    public Color[] Pixels;

    // Constructor
    public HdrImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new Color[width * height];
    }

    // Construct from pfm file
    public HdrImage(Stream stream)
    {   
        Pixels = [];
        _ReadPfm(stream);
    }

    // Construct from pfm file
    public HdrImage(string filePath)
    {
        Pixels = [];
        using var pfmStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        _ReadPfm(pfmStream);
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

    // Read PFM file
    private void _ReadPfm(Stream stream)
    {
        var magic = ReadLine(stream);
        if (magic != "PF")
        {
            throw new InvalidPfmFileFormatException("Invalid magic in Pfm file.");
        }

        var imgSize = ReadLine(stream);
        (Width, Height) = ParseImgSize(imgSize);
        Pixels = new Color[Width * Height];

        var endianStr = ReadLine(stream);
        var endianness = ParseEndianness(endianStr);

        for (var y = Height - 1; y >= 0; y--)
        {
            for (var x = 0; x < Width; x++)
            {
                var color = new Color(_ReadFloat(stream, endianness), _ReadFloat(stream, endianness),
                    _ReadFloat(stream, endianness));
                SetPixel(x, y, color);
            }
        }
    }

    // Write HdrImage to PFM file
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
                WriteFloat(outStream, color.R, endianness);
                WriteFloat(outStream, color.G, endianness);
                WriteFloat(outStream, color.B, endianness);
            }
        }
    }
    
    // Write HdrImage to PFM file
    public void WritePfm(string fileName, Endianness endianness = Endianness.LittleEndian)
    {
        string directoryPath  = Path.Combine(FindSlnPath(), "PfmFiles");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine($"Directory '{directoryPath}' did not exist and was created.");
        }
        string filePath = Path.Combine(directoryPath, fileName);
        using var outStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

        WritePfm(outStream, endianness); 
    }

    // Compute average luminosity
    public float AverageLuminosity(float delta = 1e-10f)
    {
        var sum = 0.0f;
        for (int i = 0; i < Pixels.Length; i++)
        {
            sum += MathF.Log10(delta + Pixels[i].Luminosity());
        }

        return MathF.Pow(10, sum / Pixels.Length);
    }

    // Normalize image
    public void NormalizeImage(float factor, float? luminosity = null)
    {
        var lum = luminosity ?? AverageLuminosity();
        for (int i = 0; i < Pixels.Length; ++i) Pixels[i] = factor / lum * Pixels[i];
    }

    // Equation for preventing RGB from being too large
    private static float _Clamp(float x) => x / (1.0f + x); // only used in ClampImage!

    // Clip RGB values that are too large
    public void ClampImage()
    {
        for (int i = 0; i < Pixels.Length; ++i)
        {
            Pixels[i].R = _Clamp(Pixels[i].R);
            Pixels[i].G = _Clamp(Pixels[i].G);
            Pixels[i].B = _Clamp(Pixels[i].B);
        }
    }

    // Write image to file in ldr format (format must be one of "png", "bmp", "jpeg", "gif", "tga", "webp")
    public void WriteLdr(Stream outStream, string format = "png", float gamma = 1f)
    {
        var bitmap = new Image<Rgb24>(Configuration.Default, Width, Height);
        
        for (var y = 0; y < bitmap.Height; ++y)
        {
            for (var x = 0; x < bitmap.Width; ++x)
            {
                var pixel = Pixels[_PixelOffset(x, y)];
                var r = (byte)MathF.Round(255f * MathF.Pow(pixel.R, 1f / gamma));
                var g = (byte)MathF.Round(255f * MathF.Pow(pixel.G, 1f / gamma));
                var b = (byte)MathF.Round(255f * MathF.Pow(pixel.B, 1f / gamma));
                bitmap[x, y] = new Rgb24(r, g, b);
            }
        }
        
        switch (format)
        {
            case ("png"):
                bitmap.Save(outStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                break;
            case ("bmp"):
                bitmap.Save(outStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder());
                break;
            case ("jpeg"):
                bitmap.Save(outStream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
                break;
            case ("gif"):
                bitmap.Save(outStream, new SixLabors.ImageSharp.Formats.Gif.GifEncoder());
                break;
            case ("tga"):
                bitmap.Save(outStream, new SixLabors.ImageSharp.Formats.Tga.TgaEncoder());
                break;
            case ("webp"):
                bitmap.Save(outStream, new SixLabors.ImageSharp.Formats.Webp.WebpEncoder());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    // Write image to file in ldr format (format is inferred by file extension and must be one of "png", "bmp", "jpeg", "gif", "tga", "webp")
    public void WriteLdr(string fileName, float gamma = 1f)
    {
        // Get the extension and remove the dot
        string format = Path.GetExtension(fileName).TrimStart('.');
        
        string directoryPath  = Path.Combine(FindSlnPath(), "LdrFiles");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine($"Directory '{directoryPath}' did not exist and was created.");
        }
        string filePath = Path.Combine(directoryPath, fileName);
        using var outStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        Console.WriteLine($"Writing file '{filePath}'");

        WriteLdr(outStream, format, gamma);
    }

    // Override 'Equals' method
    public override bool Equals(object? obj)
    {
        if (obj is not HdrImage other)
            return false;

        for (int i = 0; i < Pixels.Length; i++)
        {
            if (!Pixels[i].Equals(other.Pixels[i]))
                return false;
        }

        return true;
    }

    // Overload '==' operator
    public static bool operator ==(HdrImage left, HdrImage right)
    {
        return ReferenceEquals(left, right) || left.Equals(right);
    }

    // Overload '!=' operator
    public static bool operator !=(HdrImage left, HdrImage right)
    {
        return !(left == right);
    }

    // Override 'GetHashCode' to ensure consistency with 'Equals'
    public override int GetHashCode()
    {
        var hash = HashCode.Combine(Width, Height);
        foreach (var pixel in Pixels)
        {
            hash = HashCode.Combine(hash, pixel.R, pixel.G, pixel.B);
        }

        return hash;
    }
}