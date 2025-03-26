using Exceptions;

namespace Trace;

using System;

public struct Parameters
{
    public string InputPfmFileName;
    public float Factor;
    public float Gamma;
    public string OutputPngFileName;

    // Constructor
    public Parameters()
    {
        InputPfmFileName = string.Empty;
        Factor = 0.2f;
        Gamma = 1.0f;
        OutputPngFileName = string.Empty;
    }
    
    // Read the parameters from command line
    public void ParseFromCommandLine(string[] argv)
    {
        if (argv.Length != 5)
        {
            throw new RuntimeException("Usage: Main INPUT_PFM_FILE FACTOR GAMMA OUTPUT_PNG_FILE");
        }
        
        InputPfmFileName = argv[1];
        
        try
        {
            Factor = float.Parse(argv[2]);
        }
        catch (FormatException)
        {
            throw new RuntimeException("Invalid factor value, it must be a floating point number.");
        }
        
        try
        {
            Gamma = float.Parse(argv[3]);
        }
        catch (FormatException)
        {
            throw new RuntimeException("Invalid gamma value, it must be a floating point number.");
        }
        
        OutputPngFileName = argv[4];
    }
    
}