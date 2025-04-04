namespace Trace;

/// <summary>
/// Struct representing 4x4 matrix representing homogenous transformations (last row is always [0, 0, 0, 1]), with
/// elements represented as <c>float</c> values.
/// </summary>
public struct HomMat
{
    public readonly float[] Elements = new float[16];

    /// <summary>
    /// Default constructor, return an identity matrix.
    /// </summary>
    public HomMat()
    {
        Elements[0] = Elements[5] = Elements[10] = Elements[15] = 1.0f;
    }

    /// <summary>
    /// Constructor taking as arguments the first three rows, which must all have 4 elements each. Last row is
    /// automatically set to [0, 0, 0, 1].
    /// </summary>
    public HomMat(float[] row0, float[] row1, float[] row2)
    {
        Console.WriteLine("starting");
        if (row0.Length != 4 || row1.Length != 4 || row2.Length != 4)
        {
            throw new ArgumentException("HomMat rows must have 4 elements!");
        }

        for (int i = 0; i < 4; ++i)
        {
            Elements[i] = row0[i];
            Elements[i + 4] = row1[i];
            Elements[i + 8] = row2[i];
        }

        Elements[15] = 1.0f;
    }

    public float this[int i, int j]
    {
        get => Elements[4 * i + j];
        set => Elements[4 * i + j] = value;
    }

    // Return row i
    public float[] Row(int i) => [this[i, 0], this[i, 1], this[i, 2], this[i, 3]];

    // return column j
    public float[] Col(int j) => [this[0, j], this[1, j], this[2, j], this[3, j]];

    // Optimized product between two HomMat, assumes that last row of both a and b is [0, 0, 0, 1]
    public static HomMat operator *(HomMat a, HomMat b)
    {
        var row0 = new float[4];
        for (int j = 0; j < 4; j++)
        {
            // Skip k = 3, since b[3, j] = 0 for j != 3
            for (int k = 0; k < 3; k++)
            {
                row0[j] += a[0, k] * b[k, j];
            }
        }

        // Add a[0, 3] * b[3, 3] = a[0, 3] contribution
        row0[3] += a[0, 3];

        var row1 = new float[4];
        for (int j = 0; j < 4; j++)
        {
            for (int k = 0; k < 3; k++)
            {
                row1[j] += a[1, k] * b[k, j];
            }
        }

        row1[3] += a[1, 3];

        var row2 = new float[4];
        for (int j = 0; j < 4; j++)
        {
            for (int k = 0; k < 3; k++)
            {
                row2[j] += a[2, k] * b[k, j];
            }
        }

        row2[3] += a[2, 3];

        return new HomMat(row0, row1, row2);
    }

    /// <summary>
    /// Check if two <c>HomMat</c> have the same components with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(HomMat m1, HomMat m2, float epsilon = 1e-5f)
    {
        for (int i = 0; i < 16; i++)
        {
            Console.WriteLine(i);
            if (Utils.CloseEnough(m1.Elements[i], m2.Elements[i], epsilon) != true) return false;
        }

        return true;
    }

    public override string ToString() => $"[[{Elements[0]}, {Elements[1]}, {Elements[2]}, {Elements[3]}]\n" +
                                         $" [{Elements[4]}, {Elements[5]}, {Elements[6]}, {Elements[7]}]\n" +
                                         $" [{Elements[8]}, {Elements[9]}, {Elements[10]}, {Elements[11]}]\n" +
                                         $" [{Elements[12]}, {Elements[13]}, {Elements[14]}, {Elements[15]}]]";
}