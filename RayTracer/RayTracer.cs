using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Trace;

namespace RayTracer;

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

[Command("render",
    Description =
        "Render a scene from scene input file, automatically converting the resulting pfm file to a ldr image.")]
public class RenderCommand : ICommand
{
    // Parameters
    [CommandParameter(0, Description = "Input scene file path.")]
    public required string InputSceneFilePath { get; init; }


    // Options
    [CommandOption("width", 'W',
        Description = "Width of the image to render in pixels.")]
    public int Width { get; init; } = 720;

    [CommandOption("height", 'H',
        Description = "Height of the image to render in pixels.")]
    public int Height { get; init; } = 480;


    [CommandOption("algorithm", 'A',
        Description = "Rendering algorithm used. Available options are: \"on-off\", \"flat\", \"path-tracer\".")]
    public string Algorithm { get; init; } = "path-tracer";

    [CommandOption("num-rays", 'n',
        Description = "Number of rays scattered at each iteration by the path renderer.")]
    public int NumRays { get; init; } = 10;

    [CommandOption("max-depth", 'm',
        Description = "Maximum depth of rays scattered by the path renderer.")]
    public int MaxDepth { get; init; } = 3;

    [CommandOption("roulette", 'r',
        Description = "Depth of rays scattered by the path renderer before russian roulette starts taking place.")]
    public int RouletteLimit { get; init; } = 2;

    [CommandOption("init-state", 's',
        Description = "Initial seed for the random number generation.")]
    public ulong InitState { get; init; } = 42;

    [CommandOption("init-seq", 'S',
        Description = "Initial sequence value for the random number generation.")]
    public ulong InitSeq { get; init; } = 54;

    [CommandOption("pfm-output", 'p',
        Description = "Pfm output file path.")]
    public string OutputPfmFilePath { get; init; } = "output.pfm";

    [CommandOption("ldr-output", 'l',
        Description = "Ldr output file path, file extension specifies file format.")]
    public string OutputLdrFilePath { get; init; } = "output.png";

    [CommandOption("factor", 'f',
        Description = "Normalization factor, higher means a more luminous image.")]
    public float Factor { get; init; } = 0.5f;

    [CommandOption("gamma", 'g',
        Description = "Gamma correction.")]
    public float Gamma { get; init; } = 1.0f;

    [CommandOption("external-names", 'e',
        Description = "Names of variables to overwrite in input file.")]
    public List<string> ExtNames { get; init; } = [];

    [CommandOption("external-values", 'v',
        Description = "New values of variables to overwrite in input file.")]
    public List<float> ExtValues { get; init; } = [];


    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Output PFM File: {OutputPfmFilePath}");
        console.Output.WriteLine($"Output LDR File: {OutputLdrFilePath}");

        Stream stream;
        try
        {
            stream = new FileStream(InputSceneFilePath, FileMode.Open, FileAccess.Read);
        }
        catch
        {
            console.Output.WriteLine($"Error! Input file '{InputSceneFilePath}' couldn't be read!");
            return default;
        }

        var nExtVar = ExtNames.Count;
        if (nExtVar != ExtValues.Count)
        {
            console.Output.WriteLine($"Error! Number of external variables names and values provided must match!");
            return default;
        }

        var extDict = nExtVar == 0 ? null : new Dictionary<string, float>();
        for (int i = 0; i < ExtValues.Count; i++)
        {
            extDict!.Add(ExtNames[i], ExtValues[i]);
        }

        var inputStream = new InputStream(stream, InputSceneFilePath);
        var scene = new Scene(inputStream);

        try
        {
            scene.ParseScene(extDict);
        }
        catch (GrammarException e)
        {
            console.Output.WriteLine($"Error in file {e.Location.FileName}!\n{e.Message}");
            return default;
        }

        if (scene.SceneCamera == null)
        {
            console.Output.WriteLine($"Error! Camera was not defined in the input file '{InputSceneFilePath}'!");
            return default;
        }

        console.Output.WriteLine("Scene correctly parsed! Ready to render!");

        // Build renderer
        Renderer renderer;
        switch (Algorithm)
        {
            case "on-off":
                renderer = new OnOffRenderer(scene.SceneWorld);
                break;
            case "flat":
                renderer = new FlatRenderer(scene.SceneWorld);
                break;
            case "path-tracer":
                renderer = new PathTracer(scene.SceneWorld, NumRays, MaxDepth, RouletteLimit,
                    new Pcg(InitState, InitSeq));
                break;
            default:
                console.Output.WriteLine(Algorithm +
                                         "is not among implemented algorithms. Available options are: \"on-off\", \"flat\", \"path-tracer\".");
                console.Output.WriteLine("Instead using default path-tracer.");
                renderer = new PathTracer(scene.SceneWorld, NumRays, MaxDepth, RouletteLimit,
                    new Pcg(InitState, InitSeq));
                break;
        }

        // Warn if aspect ratio and image width and height are not coherent
        var whRatio = (float)Width / Height;
        var cameraRatio = scene.SceneCamera.GetAspectRatio();
        if (!Utils.CloseEnough(whRatio, cameraRatio))
        {
            console.Output.WriteLine(
                $"Warning! Camera's aspect ratio ({cameraRatio}) is different from output image aspect ratio ({whRatio}). Image will be distorted!");
        }

        // Render the scene
        var tracer = new ImageTracer(new HdrImage(Width, Height), scene.SceneCamera, renderer);
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