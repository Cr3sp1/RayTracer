namespace Trace;

using Exceptions;
using System.Text;
using static Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// Class representing a high-dynamic range image. Contains image <c> int Width, Height</c>, and an array <c>Pixels</c> of <c>Color</c>.
/// </summary>
public class HdrImage
{
    // HdrImage fields
    public int Width, Height;
    public Color[] Pixels;

    // Constructor
    public HdrImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new Color[width * height];
    }

    /// <summary>
    /// Construct <c>HdrImage</c> from PFM image.
    /// </summary>
    /// <param name="stream"><c>Stream</c> from which PFM image is read.</param>
    public HdrImage(Stream stream)
    {
        Pixels = [];
        _ReadPfm(stream);
    }

    /// <summary>
    /// Construct <c>HdrImage</c> from PFM image.
    /// </summary>
    /// <param name="filePath"><c>String</c> representing the file path from which PFM image is read.</param>
    public HdrImage(string filePath)
    {
        Pixels = [];
        Console.WriteLine($"Attepting to open file '{filePath}'");
        using var pfmStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        Console.WriteLine($"File '{filePath}' loaded.");
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

    /// <summary>
    /// Read <c>float</c> from stream with specified <c>endianness</c>. To be used only to read PFM files!
    /// </summary>
    /// <exception cref="InvalidPfmFileFormatException"></exception>
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

    // Read a PFM file
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

    /// <summary>
    /// Method to write <c>HdrImage</c> as a PFM image.
    /// </summary>
    /// <param name="outStream"><c>Stream</c> on which PFM image is written.</param>
    /// <param name="endianness">Endianness of the PFM image.</param>
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

    /// <summary>
    /// Method to write <c>HdrImage</c> as a PFM image.
    /// </summary>
    /// <param name="filePath"><c>String</c> representing the file path on which PFM image is written.</param>
    /// <param name="endianness">Endianness of the PFM image.</param>
    public void WritePfm(string filePath, Endianness endianness = Endianness.LittleEndian)
    {
        // Create an output directory if it doesn't exist
        string directoryPath = Path.GetDirectoryName(filePath) ?? Directory.GetCurrentDirectory();
        if (string.IsNullOrEmpty(directoryPath)) directoryPath = Directory.GetCurrentDirectory();
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine($"Directory '{directoryPath}' did not exist and was created.");
        }

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

    /// <summary>
    /// Method to normalize <c>HdrImage</c>.
    /// </summary>
    /// <param name="factor">Normalization factor, higher means a more luminous image.</param>
    /// <param name="luminosity">Leave blank to use average luminosity of the <c>HdrImage</c>.</param>
    public void NormalizeImage(float factor, float? luminosity = null)
    {
        var lum = luminosity ?? AverageLuminosity();
        for (int i = 0; i < Pixels.Length; ++i) Pixels[i] = factor / lum * Pixels[i];
    }

    // Equation for preventing RGB from being too large
    private static float _Clamp(float x) => x / (1.0f + x); // only used in ClampImage!

    /// <summary>
    /// Clip RGB values so that they belong in a [0, 1] interval.
    /// </summary>
    public void ClampImage()
    {
        for (int i = 0; i < Pixels.Length; ++i)
        {
            Pixels[i].R = _Clamp(Pixels[i].R);
            Pixels[i].G = _Clamp(Pixels[i].G);
            Pixels[i].B = _Clamp(Pixels[i].B);
        }
    }

    /// <summary>
    /// Write <c>HdrImage</c> in low dynamic range format. RGB values must be in [0, 1] interval!
    /// </summary>
    /// <param name="outStream"><c>Stream</c> on which LDR image is written.</param>
    /// <param name="format">Valid formats are "png", "bmp", "jpeg", "tga", "webp".</param>
    /// <param name="gamma">Gamma correction.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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
                bitmap.SaveAsPng(outStream);
                break;
            case ("bmp"):
                bitmap.SaveAsBmp(outStream);
                break;
            case ("jpeg"):
                bitmap.SaveAsJpeg(outStream);
                break;
            case ("tga"):
                bitmap.SaveAsTga(outStream);
                break;
            case ("webp"):
                bitmap.SaveAsWebp(outStream);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    /// <summary>
    /// Write <c>HdrImage</c> in low dynamic range format. RGB values must be in [0, 1] interval!
    /// </summary>
    /// <param name="filePath"><c>String</c> representing the output file path on which LDR image is written.
    /// Format is inferred from the name extension.
    /// Valid formats are "png", "bmp", "jpeg", "tga", "webp".</param>
    /// <param name="gamma">Gamma correction.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void WriteLdr(string filePath, float gamma = 1f)
    {
        // Get the extension and remove the dot
        string format = Path.GetExtension(filePath).TrimStart('.');

        // Create the output directory if it doesn't exist
        string directoryPath = Path.GetDirectoryName(filePath) ?? Directory.GetCurrentDirectory();
        if (string.IsNullOrEmpty(directoryPath)) directoryPath = Directory.GetCurrentDirectory();
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine($"Directory '{directoryPath}' did not exist and was created.");
        }

        using var outStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        // Console.WriteLine($"Writing file '{filePath}'");

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