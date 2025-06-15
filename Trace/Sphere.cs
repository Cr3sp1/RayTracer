// ReSharper disable InconsistentNaming

using System.Diagnostics;

namespace Trace;

// Class representing a sphere
public class Sphere : Shape
{
    // Constructor of the sphere subject to an optional transformation and with an optional material
    public Sphere(Transformation? transform = null, Material? material = null) : base(transform, material)
    {
    }

    // Return normal to the surface on the Point p
    public Normal SphereNormal(Point p, Vec rayDir)
    {
        var res = new Normal(p.X, p.Y, p.Z);
        return res.Dot(rayDir) < 0.0f ? res : -res;
    }

    // Return coordinates of Point p on the surface 
    public Vec2D SpherePointToUV(Point p)
    {
        float u = MathF.Atan2(p.Y, p.X) / (2.0f * MathF.PI);
        if (u < 0.0f) u += 1.0f;
        float v = MathF.Acos(p.Z) / MathF.PI;
        return new Vec2D(u, v);
    }

    /// <summary>
    /// Method to compute the intersection between a ray and a <c>Sphere</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns>Return a <c>HitRecord</c> containing details of intersection if the <c>Sphere</c> intersects the <c>Ray</c>,
    /// and return null if the <c>Sphere</c> doesn't intersect the <c>Ray</c>.</returns>
    public override HitRecord? Intersect(Ray ray)
    {
        Ray invRay = Transform.Inverse() * ray;
        Vec originVec = invRay.Origin.ToVec();
        float a = invRay.Dir.SquaredNorm();
        float b = 2.0f * originVec.Dot(invRay.Dir);
        float c = originVec.SquaredNorm() - 1.0f;

        float delta = b * b - 4.0f * a * c;
        if (delta <= 0.0f) return null;
        float sqrtDelta = MathF.Sqrt(delta);
        float tMin = (-b - sqrtDelta) / (2.0f * a);
        float tMax = (-b + sqrtDelta) / (2.0f * a);

        float tHit;
        if (invRay.TMin < tMin && tMin < invRay.TMax) tHit = tMin;
        else if (invRay.TMin < tMax && tMax < invRay.TMax) tHit = tMax;
        else return null;

        Point hitPoint = invRay.At(tHit);

        return new HitRecord(this, Transform * hitPoint, (Transform * SphereNormal(hitPoint, invRay.Dir)).Normalize(),
            SpherePointToUV(hitPoint), ray, tHit);
    }

    /// <summary>
    /// Method to compute all the intersections between a <c>Ray</c> and a <c>Sphere</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns> Return a <c>List</c> of <c>HitRecord</c> containing all the intersections between the <c>Ray</c> and
    /// a <c>Sphere</c> from closest to <c>Ray</c> origin to furthest.</returns>
    public override List<HitRecord> AllIntersects(Ray ray)
    {
        var res = new List<HitRecord>();

        Ray invRay = Transform.Inverse() * ray;
        Vec originVec = invRay.Origin.ToVec();
        float a = invRay.Dir.SquaredNorm();
        float b = 2.0f * originVec.Dot(invRay.Dir);
        float c = originVec.SquaredNorm() - 1.0f;

        float delta = b * b - 4.0f * a * c;
        if (delta <= 0.0f) return res;
        float sqrtDelta = MathF.Sqrt(delta);
        float tMin = (-b - sqrtDelta) / (2.0f * a);
        float tMax = (-b + sqrtDelta) / (2.0f * a);

        if (invRay.TMin < tMin && tMin < invRay.TMax)
        {
            Point hitPoint = invRay.At(tMin);
            res.Add(new HitRecord(this, Transform * hitPoint, Transform * SphereNormal(hitPoint, invRay.Dir),
                SpherePointToUV(hitPoint), ray, tMin));
        }

        if (invRay.TMin < tMax && tMax < invRay.TMax)
        {
            Point hitPoint = invRay.At(tMax);
            res.Add(new HitRecord(this, Transform * hitPoint, Transform * SphereNormal(hitPoint, invRay.Dir),
                SpherePointToUV(hitPoint), ray, tMax));
        }

        return res;
    }
    
    /// <summary>
    /// Method to check if a <c>HitRecord</c> falls inside a <c>Sphere</c>.
    /// </summary>
    /// <param name="hit"><c>HitRecord</c> to check.</param>
    /// <returns>1 if it falls in the <c>Sphere</c>, -1 if it falls outside the <c>Sphere</c>, and 0 if it falls
    /// on the surface of the <c>Sphere</c></returns>
    public override int IsInside(in HitRecord hit)
    {
        var invPoint = Transform.Inverse() * hit.WorldPoint;
        return (invPoint.X * invPoint.X + invPoint.Y * invPoint.Y + invPoint.Z * invPoint.Z) switch
        {
            > 1 => -1,
            < 1 => 1,
            _ => 0
        };
    }
}