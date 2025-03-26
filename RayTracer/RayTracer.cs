using System;
using Exceptions;
using Trace;

class Program
{
    static void Main(string[] args)
    {
        
        var parameters = new Parameters();
        try
        {
            parameters.ParseFromCommandLine(args);
        }
        catch (RuntimeException error)
        {
            Console.WriteLine("Error! " + error.Message);
        }
        
        
        
    }
    
}
