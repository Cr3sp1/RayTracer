using Exceptions;

namespace Trace;

using System;

public struct Parameters
{
    public string InputPfmFileName;
    public float Factor;
    public float Gamma;
    public string OutputLdrFileName;

    // Constructor
    public Parameters()
    {
        InputPfmFileName = string.Empty;
        Factor = 0.2f;
        Gamma = 1.0f;
        OutputLdrFileName = string.Empty;
    }
    
    // Read the parameters from command line
    public void ParseFromCommandLine(string[] argv)
    {
        if (argv.Length != 4)
        {
            throw new RuntimeException("Usage: INPUT_PFM_FILE FACTOR GAMMA OUTPUT_LDR_FILE");
        }
        
        InputPfmFileName = argv[0];
        
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
        
        OutputLdrFileName = argv[3];
    }
    
}