namespace Trace;

/// <summary>
/// Struct representing 3-d normals (inclination of a surface in a point), with x,y,z coordinates represented as <c>float</c> values.
/// </summary>
public struct Normal
{
    public float X, Y, Z;
    
    // Constructor
    public Normal(float x, float y, float z) => (X, Y, Z) = (x, y, z);
}