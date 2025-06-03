namespace Trace;

/// <summary>
/// Struct representing 3-d normals (inclination of a surface in a point), with x,y,z coordinates represented as <c>float</c> values.
/// </summary>
public readonly struct Normal
{
    public readonly float X, Y, Z;

    // Constructor
    public Normal(float x, float y, float z) => (X, Y, Z) = (x, y, z);

    // Product between scalar (float) and Normal
    public static Normal operator *(float f, Normal n) => new Normal(f * n.X, f * n.Y, f * n.Z);

    // Negation
    public static Normal operator -(Normal n)
    {
        return new Normal(-n.X, -n.Y, -n.Z);
    }

    // Dot product between Normal and  Vec
    public float Dot(Vec v) => X * v.X + Y * v.Y + Z * v.Z;

    // Cross product between Normal and Vec 
    public Vec Cross(Vec v) => new Vec(Y * v.Z - Z * v.Y,
        Z * v.X - X * v.Z,
        X * v.Y - Y * v.X);

    // Dot product between two Normal
    public float Dot(Normal n) => X * n.X + Y * n.Y + Z * n.Z;

    // Cross product between two Normal
    public Normal Cross(Normal other) => new Normal(Y * other.Z - Z * other.Y,
        Z * other.X - X * other.Z,
        X * other.Y - Y * other.X);

    // Squared norm
    public float SquaredNorm() => (X * X) + (Y * Y) + (Z * Z);

    // Norm
    public float Norm() => MathF.Sqrt(SquaredNorm());

    public Normal Normalize()
    {
        float norm = Norm();
        return new Normal(X/norm, Y/norm, Z/norm);
    }

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
    }

    /// <summary>
    /// Create an orthonormal basis from a <c>Normal</c>: the z-axis of the ONB will be this normal.
    /// The normal has to be normalized.
    /// WARNING: <c>Normal</c> is not normalized, so it is the caller responsibility to do so.
    /// </summary>
    /// <returns>(<c>Vec</c> e1, <c>Vec</c> e2, <c>Vec</c> e3) of the ONB.</returns>
    public (Vec, Vec, Vec) CreateOnbFromZ()
    {
        float sign = MathF.CopySign(1.0f, Z);
        float a = -1.0f / (sign + Z);
        float b = X * Y * a;

        var e1 = new Vec(1.0f + sign * X * X * a, sign * b, -sign * X);
        var e2 = new Vec(b, sign + Y * Y * a, -Y);

        return (e1, e2, new Vec(X, Y, Z));
    }

    /// <summary>
    /// Check if two <c>Normal</c> have the same components with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(Normal n1, Normal n2, float epsilon = 1e-5f) =>
        Utils.CloseEnough(n1.X, n2.X, epsilon) && Utils.CloseEnough(n1.Y, n2.Y, epsilon) &&
        Utils.CloseEnough(n1.Z, n2.Z, epsilon);

    public override string ToString() => $"Normal<X:{X}, Y:{Y}, Z:{Z}>";
}