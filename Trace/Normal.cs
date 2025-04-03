namespace Trace;

/// <summary>
/// Struct representing 3-d normals (inclination of a surface in a point), with x,y,z coordinates represented as <c>float</c> values.
/// </summary>
public struct Normal
{
    public float X, Y, Z;

    // Constructor
    public Normal(float x, float y, float z) => (X, Y, Z) = (x, y, z);

    // Sum between two Normal objects
    public static Normal operator +(Normal n1, Normal n2) => new Normal(n1.X + n2.X, n1.Y + n2.Y, n1.Z + n2.Z);

    // Difference between two Normal objects
    public static Normal operator -(Normal n1, Normal n2) => new Normal(n1.X - n2.X, n1.Y - n2.Y, n1.Z - n2.Z);

    // Product between scalar (float) and Normal
    public static Normal operator *(float f, Normal n) => new Normal(f * n.X, f * n.Y, f * n.Z);

    // Negation
    public static Normal operator -(Normal n)
    {
        return new Normal(-n.X, -n.Y, -n.Z);
    }

    // Dot product between Vec and Normal
    public float Dot(Vec v) => X * v.X + Y * v.Y + Z * v.Z;

    // Cross product between Vec and Normal
    public Vec Cross(Vec v) => new Vec(Y * v.Z - Z * v.Y,
        Z * v.X - X * v.Z,
        X * v.Y - Y * v.X);

    // Cross product between two Normal
    public Normal Cross(Normal other) => new Normal(Y * other.Z - Z * other.Y,
        Z * other.X - X * other.Z,
        X * other.Y - Y * other.X);

    // Squared norm
    public float SquaredNorm() => (X * X) + (Y * Y) + (Z * Z);

    // Norm
    public float Norm() => MathF.Sqrt(SquaredNorm());

    public void Normalize()
    {
        float norm = Norm();
        X /= norm;
        Y /= norm;
        Z /= norm;
    }

    public override string ToString() => $"Normal<X:{X}, Y:{Y}, Z:{Z}>";
}