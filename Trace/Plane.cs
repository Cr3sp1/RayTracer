namespace Trace;

public class Plane : Shape
{
    // Constructor of a plane subject to an optional transformation and with an optional material
    public Plane(Transformation? transform = null, Material? material = null) : base(transform, material)
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

        return new HitRecord(this, Transform * hitPoint, (Transform * PlaneNormal(invRay.Dir)).Normalize(),
            PlanePointToUV(hitPoint), ray, tHit);
    }

    /// <summary>
    /// Method to compute all the intersections between a ray and a plane.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns> Return a <c>List</c> of <c>HitRecord</c> containing all the intersections between the <c>Ray</c> and
    /// a <c>Plane</c> from closest to <c>Ray</c> origin to furthest.</returns>
    public override List<HitRecord> AllIntersects(Ray ray)
    {
        var res = new List<HitRecord>();
        var hit = Intersect(ray);
        if (hit.HasValue) res.Add(hit.Value);
        return res;
    }

    /// <summary>
    /// Method to check if a <c>HitRecord</c> falls inside a <c>Plane</c>.
    /// </summary>
    /// <param name="hit"><c>HitRecord</c> to check.</param>
    /// <returns>1 if it falls in the <c>Plane</c>, -1 if it falls outside the <c>Plane</c>, and 0 if it falls
    /// on the surface of the <c>Plane</c></returns>
    public override int IsInside(in HitRecord hit)
    {
        var invPoint = Transform.Inverse() * hit.WorldPoint;
        var invRayOrigin = Transform.Inverse() * hit.Ray.Origin;
        if (invRayOrigin.Z == 0.0f) return -1;
        return (invPoint.Z * invRayOrigin.Z) switch
        {
            < 0f => 1,
            > 0f => -1,
            _ => 0
        };
    }
}