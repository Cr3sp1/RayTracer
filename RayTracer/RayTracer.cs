using System;
using Exceptions;
using SixLabors.ImageSharp.Memory;
using Trace;

class Program
{   
    // Example usage: dotnet run memorial.pfm 0.2 1. memorial.png
    static void Main(string[] args)
    {
        Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
        Console.WriteLine("Path to LdrFiles: " + Path.Combine(Utils.FindSlnPath(), "LdrFiles"));

        // Read parameters
        var parameters = new Parameters();
        try
        {
            parameters.ParseFromCommandLine(args);
        }
        catch (RuntimeException error)
        {
            Console.WriteLine("Error! " + error.Message);
            return;
        }
        Console.WriteLine($"Input PFM File: {parameters.InputPfmFileName}");
        Console.WriteLine($"Factor: {parameters.Factor}");
        Console.WriteLine($"Gamma: {parameters.Gamma}");
        Console.WriteLine($"Output LDR File: {parameters.OutputLdrFileName}");

        // Read image
        var inFilePath = Path.Combine(Utils.FindSlnPath(), "PfmFiles", parameters.InputPfmFileName);
        HdrImage img;
        try
        {
            img = new HdrImage(inFilePath);
        }
        catch
        {
            Console.WriteLine($"Error! Image '{inFilePath}' couldn't be read.");
            return;
        }
        Console.WriteLine($"Image '{inFilePath}' successfully read");

        // Prepare image
        img.NormalizeImage(parameters.Factor);
        Console.WriteLine("Image successfully normalized");
        img.ClampImage();
        Console.WriteLine("Image successfully clamped");
        
        // Print image
        try
        {
            img.WriteLdr(parameters.OutputLdrFileName, parameters.Gamma);
        }
        catch
        {
            Console.WriteLine($"Error! Image couldn't be written.");
        }
        Console.WriteLine($"Image successfully written.");
    }
}