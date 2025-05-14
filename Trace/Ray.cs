namespace Trace;

using System;

/// <summary>
/// Struct representing 3-d rays of light, with members <c>Point Origin</c> (origin of the ray),
/// <c>Vec Dir</c> (direction of the ray) and <c>float</c> minimum/maximum distance and depth set by default.
/// </summary>
public struct Ray
{
    public Point Origin;
    public Vec Dir;
    public float TMin;
    public float TMax;
    public int Depth;

    // Constructor
    public Ray(Point origin, Vec dir, float tmin = 1e-5f, float tmax = Single.PositiveInfinity, int depth = 0)
    {
        Origin = origin;
        Dir = dir;
        TMin = tmin;
        TMax = tmax;
        Depth = depth;
    }

    ///<summary>
    /// Return a <c>Point</c> representing a position along a <c>Ray</c> for a given t.
    /// </summary>
    public Point At(float t) => Origin + t * Dir;

    /// <summary>
    /// Check if two <c>Ray</c> have the same origin and direction with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(Ray r1, Ray r2, float epsilon = 1e-5f) =>
        Point.CloseEnough(r1.Origin, r2.Origin, epsilon) && Vec.CloseEnough(r1.Dir, r2.Dir, epsilon);
}