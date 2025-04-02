using System;
using Exceptions;
using SixLabors.ImageSharp.Memory;
using Trace;

class Program
{   
    // Example usage: dotnet run memorial.pfm 0.2 1. memorial.png
    static void Main(string[] args)
    { 
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
        Console.WriteLine($"Input PFM File: {parameters.InputPfmFilePath}");
        Console.WriteLine($"Factor: {parameters.Factor}");
        Console.WriteLine($"Gamma: {parameters.Gamma}");
        Console.WriteLine($"Output LDR File: {parameters.OutputLdrFilePath}");

        // Read image
        HdrImage img;
        try
        {
            img = new HdrImage(parameters.InputPfmFilePath);
        }
        catch
        {
            Console.WriteLine($"Error! Image '{parameters.InputPfmFilePath}' couldn't be read.");
            return;
        }
        Console.WriteLine($"Image '{parameters.InputPfmFilePath}' successfully read");

        // Prepare image
        img.NormalizeImage(parameters.Factor);
        Console.WriteLine("Image successfully normalized");
        img.ClampImage();
        Console.WriteLine("Image successfully clamped");
        
        // Print image
        try
        {
            img.WriteLdr(parameters.OutputLdrFilePath, parameters.Gamma);
            Console.WriteLine($"Image successfully written.");
        }
        catch
        {
            Console.WriteLine($"Error! Image couldn't be written.");
        }
    }
}