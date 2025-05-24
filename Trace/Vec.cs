namespace Trace;

/// <summary>
/// Struct representing 3-d vectors (direction of light propagation), with x,y,z coordinates represented as <c>float</c> values.
/// </summary>
public struct Vec
{
    public float X, Y, Z;

    // Constructor
    public Vec(float x, float y, float z) => (X, Y, Z) = (x, y, z);

    // Vec objects along X, Y, Z axes
    public static readonly Vec XAxis = new Vec(1.0f, 0.0f, 0.0f);
    public static readonly Vec YAxis = new Vec(0.0f, 1.0f, 0.0f);
    public static readonly Vec ZAxis = new Vec(0.0f, 0.0f, 1.0f);

    // Sum between two Vec objects
    public static Vec operator +(Vec v1, Vec v2) => new Vec(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

    // Difference between two Vec objects
    public static Vec operator -(Vec v1, Vec v2) => new Vec(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

    // Product between scalar (float) and Vec
    public static Vec operator *(float f, Vec v) => new Vec(f * v.X, f * v.Y, f * v.Z);

    // Negation
    public static Vec operator -(Vec v)
    {
        return new Vec(-v.X, -v.Y, -v.Z);
    }

    // Dot product between two Vec
    public float Dot(Vec other) => X * other.X + Y * other.Y + Z * other.Z;

    // Cross product between two Vec
    public Vec Cross(Vec other) => new Vec(Y * other.Z - Z * other.Y,
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

    public Normal ToNormal() => new Normal(X, Y, Z);

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
    /// Check if two <c>Vec</c> have the same components with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(Vec v1, Vec v2, float epsilon = 1e-5f) =>
        Utils.CloseEnough(v1.X, v2.X, epsilon) && Utils.CloseEnough(v1.Y, v2.Y, epsilon) &&
        Utils.CloseEnough(v1.Z, v2.Z, epsilon);

    public override string ToString() => $"Vec<X:{X}, Y:{Y}, Z:{Z}>";
}