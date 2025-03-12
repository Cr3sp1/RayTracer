namespace Trace;

public static class Utils
{
    // Checks if two floats are equal with a tolerance epsilon
    public static bool CloseEnough(float a, float b, float epsilon=1e-5f) => Math.Abs(a - b) < epsilon;
}