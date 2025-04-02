namespace Trace;

/// <summary>
/// Struct representing 3-d points, with x,y,z coordinates represented as <c>float</c> values.
/// </summary>
public struct Point
{
    public float X, Y, Z;
    
    // Constructor
    public Point(float x, float y, float z) => (X, Y, Z) = (x, y, z);
    
}