namespace Trace;

public struct Transformation
{
    public HomMat M;
    public HomMat InvM;

    /// <summary>
    /// Default constructor, return an identity transformation.
    /// </summary>
    public Transformation()
    {
        M = new HomMat();
        InvM = new HomMat();
    }

    /// <summary>
    /// Constructor taking as arguments two <c>HomMat</c> objects, one the inverse of the other.
    /// </summary>
    public Transformation(HomMat m, HomMat invm)
    {
        M = new HomMat(m.Row(0), m.Row(1), m.Row(2));
        InvM = new HomMat(invm.Row(0), invm.Row(1), invm.Row(2));
    }

    // Return inverse transformation
    public Transformation Inverse() => new Transformation(InvM, M);

    // Composition of two transformations
    public static Transformation operator *(Transformation t1, Transformation t2)
    {
        return new Transformation(t1.M * t2.M, t2.InvM * t1.InvM);
    }
    
    // Apply a transformation to a Point object
    public static Point operator *(Transformation t, Point p)
    {
        var p0 = 0f;
        var p1 = 0f;
        var p2 = 0f;

        for (int j = 0; j < 3; j++)
        {
            p0 += t.M[0, j] * p[j];
            p1 += t.M[1, j] * p[j];
            p2 += t.M[2, j] * p[j];
        }

        p0 += t.M[0, 3];
        p1 += t.M[1, 3];
        p2 += t.M[2, 3];

        return new Point(p0, p1, p2);

    } 
    
    // Apply a transformation to a Vec object
    public static Vec operator *(Transformation t, Vec v)
    {
        var v0 = 0f;
        var v1 = 0f;
        var v2 = 0f;

        for (int j = 0; j < 3; j++)
        {
            v0 += t.M[0, j] * v[j];
            v1 += t.M[1, j] * v[j];
            v2 += t.M[2, j] * v[j];
        }

        return new Vec(v0, v1, v2);

    }
    
    // Apply a transformation to a Normal object
    public static Normal operator *(Transformation t, Normal n)
    {
        var n0 = 0f;
        var n1 = 0f;
        var n2 = 0f;

        for (int j = 0; j < 3; j++)
        {
            n0 += t.InvM[j, 0] * n[j];
            n1 += t.InvM[j, 1] * n[j];
            n2 += t.InvM[j, 2] * n[j];
        }

        return new Normal(n0, n1, n2);

    }
    
    // Apply a Transformation object to a Ray
    public static Ray operator *(Transformation t, Ray r) => new Ray(t * r.Origin, t * r.Dir, r.TMin, r.TMax, r.Depth);
    
    /// <summary>
    /// Check if Transformation object has been built with a <c>HomMat</c> object and its inverse.
    /// </summary>
    public bool IsConsistent()
    {
        return (HomMat.CloseEnough(M * InvM, new HomMat()) && HomMat.CloseEnough(InvM * M, new HomMat()));
    }
}