namespace Trace;

/// <summary>
/// Struct representing 3-d vectors (direction of light propagation), with x,y,z coordinates represented as <c>float</c> values.
/// </summary>
public struct Vec
{
    public float X, Y, Z;
    
    // Constructor
    public Vec(float x, float y, float z) => (X, Y, Z) = (x, y, z);
}