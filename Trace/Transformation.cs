namespace Trace;

public readonly struct Transformation
{
    public readonly HomMat M;
    public readonly HomMat InvM;

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
        M = m;
        InvM = invm;
    }

    // Return inverse transformation
    public Transformation Inverse() => new Transformation(InvM, M);

    // Composition of two transformations
    public static Transformation operator *(Transformation t1, Transformation t2)
    {
        return new Transformation(t1.M * t2.M, t2.InvM * t1.InvM);
    }

    /// <summary>
    /// Return a <c>Transformation</c> encoding a translation.
    /// </summary>
    /// <param name="delta">The components along X, Y, Z represent the translation in each direction X, Y, Z.</param>
    /// <returns></returns>
    public static Transformation Translation(Vec delta)
    {
        return new Transformation(
            new HomMat([1.0F, 0.0F, 0.0F, delta.X], [0.0f, 1.0f, 0.0f, delta.Y], [0.0f, 0.0f, 1.0f, delta.Z]),
            new HomMat([1.0F, 0.0F, 0.0F, -delta.X], [0.0f, 1.0f, 0.0f, -delta.Y], [0.0f, 0.0f, 1.0f, -delta.Z]));
    }

    /// <summary>
    /// Return a <c>Transformation</c> encoding a scaling.
    /// </summary>
    /// <param name="scale">The components along X, Y, Z represent the scaling in each direction X, Y, Z.</param>
    /// <returns></returns>
    public static Transformation Scaling(Vec scale)
    {
        return new Transformation(
            new HomMat(
                [scale.X, 0.0F, 0.0F, 0.0F],
                [0.0f, scale.Y, 0.0f, 0.0f],
                [0.0f, 0.0f, scale.Z, 0.0f]),
            new HomMat(
                [1.0f / scale.X, 0.0F, 0.0F, 0.0F],
                [0.0f, 1.0f / scale.Y, 0.0f, 0.0f],
                [0.0f, 0.0f, 1.0f / scale.Z, 0.0f]));
    }

    /// <summary>
    /// Return a <c>Transformation</c> encoding a rotation around the X axis.
    /// </summary>
    /// <param name="angle">Rotation angle expressed in degrees, sign follows the right hand rule.</param>
    /// <returns></returns>
    public static Transformation RotationX(float angle)
    {
        float sinAngle = MathF.Sin(float.Pi * angle / 180.0f);
        float cosAngle = MathF.Cos(float.Pi * angle / 180.0f);

        return new Transformation(
            new HomMat(
                [1.0F, 0.0F, 0.0F, 0.0F],
                [0.0f, cosAngle, -sinAngle, 0.0F],
                [0.0f, sinAngle, cosAngle, 0.0F]),
            new HomMat(
                [1.0F, 0.0F, 0.0F, 0.0F],
                [0.0f, cosAngle, sinAngle, 0.0F],
                [0.0f, -sinAngle, cosAngle, 0.0F]));
    }

    /// <summary>
    /// Return a <c>Transformation</c> encoding a rotation around the Y axis.
    /// </summary>
    /// <param name="angle">Rotation angle expressed in degrees, sign follows the right hand rule.</param>
    /// <returns></returns>
    public static Transformation RotationY(float angle)
    {
        float sinAngle = MathF.Sin(float.Pi * angle / 180.0f);
        float cosAngle = MathF.Cos(float.Pi * angle / 180.0f);

        return new Transformation(
            new HomMat(
                [cosAngle, 0.0F, sinAngle, 0.0F],
                [0.0f, 1.0f, 0.0f, 0.0F],
                [-sinAngle, 0.0F, cosAngle, 0.0F]),
            new HomMat([cosAngle, 0.0F, -sinAngle, 0.0F], [0.0f, 1.0f, 0.0f, 0.0F], [sinAngle, 0.0F, cosAngle, 0.0F]));
    }

    /// <summary>
    /// Return a <c>Transformation</c> encoding a rotation around the Z axis.
    /// </summary>
    /// <param name="angle">Rotation angle expressed in degrees, sign follows the right hand rule.</param>
    /// <returns></returns>
    public static Transformation RotationZ(float angle)
    {
        float sinAngle = MathF.Sin(float.Pi * angle / 180.0f);
        float cosAngle = MathF.Cos(float.Pi * angle / 180.0f);

        return new Transformation(
            new HomMat(
                [cosAngle, -sinAngle, 0.0f, 0.0F],
                [sinAngle, cosAngle, 0.0f, 0.0F],
                [0.0F, 0.0F, 1.0f, 0.0F]),
            new HomMat(
                [cosAngle, sinAngle, 0.0f, 0.0F],
                [-sinAngle, cosAngle, 0.0f, 0.0F],
                [0.0F, 0.0F, 1.0f, 0.0F]));
    }

    // Apply a transformation to a Point object
    public static Point operator *(in Transformation t, in Point p)
    {
        var p0 = 0f;
        var p1 = 0f;
        var p2 = 0f;

        p0 += t.M[0, 0] * p.X;
        p1 += t.M[1, 0] * p.X;
        p2 += t.M[2, 0] * p.X;
        p0 += t.M[0, 1] * p.Y;
        p1 += t.M[1, 1] * p.Y;
        p2 += t.M[2, 1] * p.Y;
        p0 += t.M[0, 2] * p.Z;
        p1 += t.M[1, 2] * p.Z;
        p2 += t.M[2, 2] * p.Z;
        p0 += t.M[0, 3];
        p1 += t.M[1, 3];
        p2 += t.M[2, 3];

        return new Point(p0, p1, p2);
    }

    // Apply a transformation to a Vec object
    public static Vec operator *(in Transformation t, in Vec v)
    {
        var v0 = 0f;
        var v1 = 0f;
        var v2 = 0f;

        v0 += t.M[0, 0] * v.X;
        v1 += t.M[1, 0] * v.X;
        v2 += t.M[2, 0] * v.X;
        v0 += t.M[0, 1] * v.Y;
        v1 += t.M[1, 1] * v.Y;
        v2 += t.M[2, 1] * v.Y;
        v0 += t.M[0, 2] * v.Z;
        v1 += t.M[1, 2] * v.Z;
        v2 += t.M[2, 2] * v.Z;

        return new Vec(v0, v1, v2);
    }

    // Apply a transformation to a Normal object
    public static Normal operator *(in Transformation t, in Normal n)
    {
        var n0 = 0f;
        var n1 = 0f;
        var n2 = 0f;

        n0 += t.InvM[0, 0] * n.X;
        n1 += t.InvM[0, 1] * n.X;
        n2 += t.InvM[0, 2] * n.X;
        n0 += t.InvM[1, 0] * n.Y;
        n1 += t.InvM[1, 1] * n.Y;
        n2 += t.InvM[1, 2] * n.Y;
        n0 += t.InvM[2, 0] * n.Z;
        n1 += t.InvM[2, 1] * n.Z;
        n2 += t.InvM[2, 2] * n.Z;

        return new Normal(n0, n1, n2);
    }

    // Apply a Transformation object to a Ray
    public static Ray operator *(in Transformation t, in Ray r) =>
        new Ray(t * r.Origin, t * r.Dir, r.TMin, r.TMax, r.Depth);

    /// <summary>
    /// Check if Transformation object has been built with a <c>HomMat</c> object and its inverse.
    /// </summary>
    public bool IsConsistent()
    {
        return (HomMat.CloseEnough(M * InvM, new HomMat()) && HomMat.CloseEnough(InvM * M, new HomMat()));
    }

    /// <summary>
    /// Check if two <c>Transformation</c> have the same components with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(Transformation tr1, Transformation tr2, float epsilon = 1e-5f)
    {
        return HomMat.CloseEnough(tr1.M, tr2.M, epsilon) && HomMat.CloseEnough(tr1.InvM, tr2.InvM, epsilon);
    }
}