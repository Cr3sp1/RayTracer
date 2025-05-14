using System.Runtime.CompilerServices;

namespace Trace;

/// <summary>
/// Struct representing 3-d points, with x,y,z coordinates represented as <c>float</c> values.
/// </summary>
public struct Point
{
    public float X, Y, Z;

    // Constructor
    public Point(float x, float y, float z) => (X, Y, Z) = (x, y, z);

    // Difference between two Points objects
    public static Vec operator -(Point p1, Point p2) => new Vec(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);

    // Sum between a Point and a Vec
    public static Point operator +(Point p, Vec v) => new Point(p.X + v.X, p.Y + v.Y, p.Z + v.Z);

    // Difference between a Point and a Vec
    public static Point operator -(Point p, Vec v) => new Point(p.X - v.X, p.Y - v.Y, p.Z - v.Z);

    // Product between scalar (float) and Point
    public static Point operator *(float f, Point p) => new Point(f * p.X, f * p.Y, f * p.Z);

    // Conversion from Point to Vec
    public Vec ToVec() => new Vec(X, Y, Z);

    public float this[int index]
    {
        get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException("Index must be 0, 1 or 2")
            };
        }
        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                default:
                    throw new IndexOutOfRangeException("Index must be 0, 1 or 2");
            }
        }
    }

    /// <summary>
    /// Check if two <c>Point</c> have the same coordinates with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(Point p1, Point p2, float epsilon = 1e-5f) =>
        Utils.CloseEnough(p1.X, p2.X, epsilon) && Utils.CloseEnough(p1.Y, p2.Y, epsilon) &&
        Utils.CloseEnough(p1.Z, p2.Z, epsilon);

    public override string ToString() => $"Point<X:{X}, Y:{Y}, Z:{Z}>";
}