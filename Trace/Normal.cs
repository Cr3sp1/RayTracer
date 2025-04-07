namespace Trace;

/// <summary>
/// Struct representing 3-d normals (inclination of a surface in a point), with x,y,z coordinates represented as <c>float</c> values.
/// </summary>
public struct Normal
{
    public float X, Y, Z;

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

    public void Normalize()
    {
        float norm = Norm();
        X /= norm;
        Y /= norm;
        Z /= norm;
    }
    
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
    /// Check if two <c>Normal</c> have the same components with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(Normal n1, Normal n2, float epsilon = 1e-5f) =>
        Utils.CloseEnough(n1.X, n2.X, epsilon) && Utils.CloseEnough(n1.Y, n2.Y, epsilon) &&
        Utils.CloseEnough(n1.Z, n2.Z, epsilon);

    public override string ToString() => $"Normal<X:{X}, Y:{Y}, Z:{Z}>";
}