namespace Trace;

public class Plane : Shape
{
    // Default constructor
    public Plane() : base()
    {
    }

    // Constructor of a plane subject to a transformation
    public Plane(Transformation transform) : base(transform)
    {
    }

    // Return normal to the surface on Point p
    public Normal PlaneNormal(Vec rayDir)
    {
        var res = new Normal(0.0f, 0.0f, 1.0f);
        return rayDir.Z < 0.0f ? res : -res;
    }

    // Return 2D coordinates of Point p on the surface
    public Vec2D PlanePointToUV(Point p)
    {
        float u = (float)(p.X - Math.Floor(p.X));
        float v = (float)(p.Y - Math.Floor(p.Y));
        return new Vec2D(u, v);
    }

    /// <summary>
    /// Method to compute the intersection between a ray and a plane.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns>Return a <c>HitRecord</c> containing details of intersection if the Plane intersects the <c>Ray</c>,
    /// and return null if the Plane doesn't intersect the <c>Ray</c>.</returns>
    public override HitRecord? Intersect(Ray ray)
    {
        Ray invRay = Transform.Inverse() * ray;
        if (invRay.Dir.Z == 0.0f) return null;
        float tInters = -invRay.Origin.Z / invRay.Dir.Z;

        float tHit;
        if (tInters > invRay.TMin && tInters < invRay.TMax) tHit = tInters;
        else return null;

        Point hitPoint = invRay.At(tHit);

        return new HitRecord(Transform * hitPoint, Transform * PlaneNormal(invRay.Dir),
            PlanePointToUV(hitPoint), tHit, ray);
    }
}