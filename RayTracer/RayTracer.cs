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
            .SetExecutableName("RayTracer")
            .Build()
            .RunAsync();
}

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
        Description = "Normalization factor, higher means a more luminous image.")]
    public float Factor { get; init; } = 0.2f;

    [CommandOption("gamma", 'g',
        Description = "Gamma correction.")]
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


[Command("demo",
    Description =
        "Render a scene, automatically converting the pfm file to a ldr image.")]
public class DemoCommand : ICommand
{
    // Options
    [CommandOption("width", 'W',
        Description = "Width of the image to render in pixels.")]
    public int Width { get; init; } = 640;

    [CommandOption("height", 'H',
        Description = "Height of the image to render in pixels.")]
    public int Height { get; init; } = 480;

    [CommandOption("angle-deg", 'a',
        Description = "Angle of view, expressed in degrees.")]
    public float Angle { get; init; } = 0.0f;

    [CommandOption("orthogonal", 'o',
        Description = "Set camera projection to orthogonal instead of perspective.")]
    public bool UseOrthogonalCamera { get; init; } = false;

    [CommandOption("distance", 'd',
        Description = "Distance of a perspective observer from the screen.")]
    public float Distance { get; init; } = 1.0f;

    [CommandOption("pfm-output", 'p',
        Description = "Pfm output file path.")]
    public string OutputPfmFilePath { get; init; } = "output.pfm";

    [CommandOption("ldr-output", 'l',
        Description = "Ldr output file path, file extension specifies file format.")]
    public string OutputLdrFilePath { get; init; } = "output.png";

    [CommandOption("factor", 'f',
        Description = "Normalization factor, higher means a more luminous image.")]
    public float Factor { get; init; } = 0.2f;

    [CommandOption("gamma", 'g',
        Description = "Gamma correction.")]
    public float Gamma { get; init; } = 1.0f;


    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Output PFM File: {OutputPfmFilePath}");
        console.Output.WriteLine($"Output LDR File: {OutputLdrFilePath}");

        var aspectRatio = Width / (float)Height;

        // Prepare World and Camera
        var scene = new World();
        ICamera camera;
        if (UseOrthogonalCamera)
        {
            camera = new OrthogonalCamera(
                Transformation.RotationZ(Angle) * Transformation.Translation(new Vec(-1.0f, 0.0f, 0.0f)), aspectRatio);
        }
        else
        {
            camera = new PerspectiveCamera(
                Transformation.RotationZ(Angle) * Transformation.Translation(new Vec(-1.0f, 0.0f, 0.0f)), Distance,
                aspectRatio);
        }

        // Set the scene
        float rad = 0.1f;
        var matRed = new Material(new Brdf(new UniformPigment(Color.Red)));
        var matGreen = new Material(new Brdf(new UniformPigment(Color.Green)));
        var matBlue = new Material(new Brdf(new UniformPigment(Color.Blue)));
        var matWhite = new Material(new Brdf(new UniformPigment(Color.White)));
        var matCheck1 = new Material(new Brdf(new CheckeredPigment(Color.Red, Color.White)));
        var matCheck2 = new Material(new Brdf(new CheckeredPigment(Color.Green, Color.Blue, 3)));
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(-0.5f, -0.5f, 0.5f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matCheck2));
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(-0.5f, 0.5f, 0.5f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matGreen));
        ;
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(0.5f, -0.5f, 0.5f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matBlue));
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(0.5f, 0.5f, 0.5f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matCheck1));
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(-0.5f, -0.5f, -0.5f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matBlue));
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(-0.5f, 0.5f, -0.5f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matRed));
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(0.5f, -0.5f, -0.5f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matGreen));
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(0.5f, 0.5f, -0.5f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matWhite));
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(0.0f, 0.0f, -0.5f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matBlue));
        scene.AddShape(new Sphere(Transformation.Translation(new Vec(0.0f, 0.5f, 0.0f)) *
                                  Transformation.Scaling(new Vec(rad, rad, rad)), matRed));
        console.Output.WriteLine("Scene successfully set");

        // Render the scene with on-off renderer
        var renderer = new FlatRenderer(scene);
        var tracer = new ImageTracer(new HdrImage(Width, Height), camera, renderer);
        tracer.FireAllRays();

        // Read rendered Pfm image
        var img = tracer.Image;
        img.WritePfm(OutputPfmFilePath);
        console.Output.WriteLine($"Image '{OutputPfmFilePath}' successfully produced.");

        // Prepare image
        img.NormalizeImage(Factor);
        img.ClampImage();

        // Produce Ldr image
        try
        {
            img.WriteLdr(OutputLdrFilePath, Gamma);
            console.Output.WriteLine($"Image '{OutputLdrFilePath}' successfully produced.");
        }
        catch
        {
            console.Output.WriteLine($"Error! Image '{OutputLdrFilePath}' couldn't be written.");
        }

        return default;
    }
}