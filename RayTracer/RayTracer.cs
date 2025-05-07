using System;
using Exceptions;
using SixLabors.ImageSharp.Memory;
using Trace;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

class Program
{
    public static async Task<int> Main() =>
        await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .Build()
            .RunAsync();
}

    // // Example usage: dotnet run memorial.pfm 0.2 1. memorial.png
    // static void Main(string[] args)
    // {
    //     // Read parameters
    //     var parameters = new Parameters();
    //     try
    //     {
    //         parameters.ParseFromCommandLine(args);
    //     }
    //     catch (RuntimeException error)
    //     {
    //         Console.WriteLine("Error! " + error.Message);
    //         return;
    //     }
    //
    //     Console.WriteLine($"Input PFM File: {parameters.InputPfmFilePath}");
    //     Console.WriteLine($"Factor: {parameters.Factor}");
    //     Console.WriteLine($"Gamma: {parameters.Gamma}");
    //     Console.WriteLine($"Output LDR File: {parameters.OutputLdrFilePath}");
    //
    //     // Read image
    //     HdrImage img;
    //     try
    //     {
    //         img = new HdrImage(parameters.InputPfmFilePath);
    //     }
    //     catch
    //     {
    //         Console.WriteLine($"Error! Image '{parameters.InputPfmFilePath}' couldn't be read.");
    //         return;
    //     }
    //
    //     Console.WriteLine($"Image '{parameters.InputPfmFilePath}' successfully read");
    //
    //     // Prepare image
    //     img.NormalizeImage(parameters.Factor);
    //     Console.WriteLine("Image successfully normalized");
    //     img.ClampImage();
    //     Console.WriteLine("Image successfully clamped");
    //
    //     // Print image
    //     try
    //     {
    //         img.WriteLdr(parameters.OutputLdrFilePath, parameters.Gamma);
    //         Console.WriteLine($"Image successfully written.");
    //     }
    //     catch
    //     {
    //         Console.WriteLine($"Error! Image couldn't be written.");
    //     }
    // }


[Command("pfm2ldr",
    Description =
        "Convert a .pfm file to a ldr format file. Supported formats are: \"png\", \"bmp\", \"jpeg\", \"tga\", \"webp\".")]
public class ConverterCommand : ICommand
{
    // Parameters
    [CommandParameter(0, Description = "Input file path.")]
    public required string InputPfmFilePath { get; init; }

    [CommandParameter(1, Description = "Output file path, file extension specifies file format.")]
    public required string OutputLdrFilePath { get; init; }


    // Options
    [CommandOption("factor", 'f',
        Description = "Normalization factor, higher means a more luminous image. Default: \"0.2\".")]
    public float Factor { get; init; } = 0.2f;

    [CommandOption("gamma", 'g',
        Description = "Gamma correction. Default: \"1.0\".")]
    public float Gamma { get; init; } = 1.0f;


    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Input PFM File: {InputPfmFilePath}");
        console.Output.WriteLine($"Factor: {Factor}");
        console.Output.WriteLine($"Gamma: {Gamma}");
        console.Output.WriteLine($"Output LDR File: {OutputLdrFilePath}");

        // Read image
        HdrImage img;
        try
        {
            img = new HdrImage(InputPfmFilePath);
        }
        catch
        {
            console.Output.WriteLine($"Error! Image '{InputPfmFilePath}' couldn't be read.");
            return default;
        }

        console.Output.WriteLine($"Image '{InputPfmFilePath}' successfully read");

        // Prepare image
        img.NormalizeImage(Factor);
        console.Output.WriteLine("Image successfully normalized");
        img.ClampImage();
        console.Output.WriteLine("Image successfully clamped");

        // Print image
        try
        {
            img.WriteLdr(OutputLdrFilePath, Gamma);
            console.Output.WriteLine($"Image successfully written.");
        }
        catch
        {
            console.Output.WriteLine($"Error! Image couldn't be written.");
        }
        return default;
    }
}