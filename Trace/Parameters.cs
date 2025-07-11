namespace Trace;

using System;

/// <summary>
/// Struct containing parameters passed from main used in PFM to LDR image conversion.
/// </summary>
public struct Parameters()
{
    public string InputPfmFilePath = string.Empty;
    public float Factor = 0.2f;
    public float Gamma = 1.0f;
    public string OutputLdrFilePath = string.Empty;

    // Read the parameters from command line
    public void ParseFromCommandLine(string[] argv)
    {
        if (argv.Length != 4)
        {
            throw new RuntimeException("Usage: INPUT_PFM_FILE FACTOR GAMMA OUTPUT_LDR_FILE");
        }

        InputPfmFilePath = argv[0];

        try
        {
            Factor = float.Parse(argv[1]);
        }
        catch (FormatException)
        {
            throw new RuntimeException("Invalid factor value, it must be a floating point number.");
        }

        try
        {
            Gamma = float.Parse(argv[2]);
        }
        catch (FormatException)
        {
            throw new RuntimeException("Invalid gamma value, it must be a floating point number.");
        }

        OutputLdrFilePath = argv[3];

        string format = Path.GetExtension(OutputLdrFilePath).TrimStart('.');
        string[] validFormats = ["png", "bmp", "jpeg", "gif", "tga", "webp"];
        if (validFormats.Contains(format) != true) throw new RuntimeException("Invalid output file format.");
    }
}