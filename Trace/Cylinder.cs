namespace Trace;

/// <summary>
/// Class representing a cylinder: the basic cylinder has the z-axis as axis and is centered at the origin, with length 2 and radius 2
/// </summary>
public class Cylinder : Shape
{
    // Constructor of the cylinder subject to an optional transformation and with an optional material
    public Cylinder(Transformation? transform = null, Material? material = null) : base(transform, material)
    {
        BBox = GetBoundingBox();
    }

    // Return normal to the surface on the Point p
    public static Normal CylinderNormal(Point p, Vec rayDir)
    {
        var res = new Normal(p.X, p.Y, 0.0f);
        return res.Dot(rayDir) < 0.0f ? res : -res;
    }

    // Return coordinates of Point p on the surface 
    public static Vec2D CylinderPointToUV(Point p)
    {
        float u = MathF.Atan2(p.Y, p.X) / (2.0f * MathF.PI);
        if (u < 0.0f) u += 1.0f;
        float v = (p.Z + 1.0f) / 2.0f;
        return new Vec2D(u, v);
    }

    /// <summary>
    /// Method to compute the intersection between a ray and a <c>Cylinder</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns>Return a <c>HitRecord</c> containing details of intersection if the <c>Cylinder</c> intersects the <c>Ray</c>,
    /// and return null if the <c>Cylinder</c> doesn't intersect the <c>Ray</c>.</returns>
    public override HitRecord? Intersect(Ray ray)
    {
        if (BBox.HasValue)
        {
            if (!BBox.Value.DoesIntersect(ray)) return null;
        }

        Ray invRay = Transform.Inverse() * ray;
        Vec originVec = invRay.Origin.ToVec();
        float a = invRay.Dir.X * invRay.Dir.X + invRay.Dir.Y * invRay.Dir.Y;
        float b = 2.0f * (originVec.X * invRay.Dir.X + originVec.Y * invRay.Dir.Y);
        float c = originVec.X * originVec.X + originVec.Y * originVec.Y - 1.0f;

        float delta = b * b - 4.0f * a * c;
        float tHit;
        float sqrtDelta;
        if (delta < 0.0f) return null;
        else if (Utils.CloseEnough(delta, 0.0f))
        {
            sqrtDelta = MathF.Sqrt(delta);
            float tSol = (-b - sqrtDelta) / (2.0f * a);
            if (invRay.TMin < tSol && tSol < invRay.TMax) tHit = tSol;
            else return null;
        }
        else
        {
            sqrtDelta = MathF.Sqrt(delta);
            float tMin = (-b - sqrtDelta) / (2.0f * a);
            float tMax = (-b + sqrtDelta) / (2.0f * a);

            if (invRay.TMin < tMin && tMin < invRay.TMax) tHit = tMin;
            else if (invRay.TMin < tMax && tMax < invRay.TMax) tHit = tMax;
            else return null;
        }

        Point hitPoint = invRay.At(tHit);
        if (hitPoint.Z < -1.0f || hitPoint.Z > 1.0f) return null;

        return new HitRecord(this, Transform * hitPoint, (Transform * CylinderNormal(hitPoint, invRay.Dir)).Normalize(),
            CylinderPointToUV(hitPoint), ray, tHit);
    }

    /// <summary>
    /// Method that computes the axis-aligned bounding box containing the <c>Cylinder</c>.
    /// </summary>
    /// <returns> a <c>BoundingBox</c> containing the <c>Cylinder</c>.</returns>
    public sealed override BoundingBox? GetBoundingBox()
    {
        var bbox = new BoundingBox(-1f, -1f, -1f, 1f, 1f, 1f);
        return Transform * bbox;
    }
}