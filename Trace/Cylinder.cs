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
    
    // Return normal to the surface on Point p and coordinates on the surface for base cylinder
    public static (Normal, Vec2D) CylinderNormalAndUV(Point p, Vec rayDir)
    {
        // Determine the cylinder surface: 1=up-cap, 2=down-cap, 3=lateral
        int face;
        if (Utils.CloseEnough(p.Z, 1.0f)) face = 1;
        else if (Utils.CloseEnough(p.Z, -1.0f)) face = 2;
        else face = 3;

        float u, v;
        Normal normal;

        switch (face)
        {
            case 1:
                u = MathF.Atan2(p.Y, p.X) / (2.0f * MathF.PI);
                if (u < 0.0f) u += 1.0f;
                v = MathF.Sqrt(p.X * p.X + p.Y * p.Y);
                normal = new Normal(0f, 0f, 1f);
                break;
            case 2:
                u = MathF.Atan2(p.Y, p.X) / (2.0f * MathF.PI);
                if (u < 0.0f) u += 1.0f;
                v = MathF.Sqrt(p.X * p.X + p.Y * p.Y);
                normal = new Normal(0f, 0f, -1f);
                break;
            case 3:
                u = MathF.Atan2(p.Y, p.X) / (2.0f * MathF.PI);
                if (u < 0.0f) u += 1.0f;
                v = (p.Z + 1.0f) / 2.0f;
                normal = new Normal(p.X, p.Y, 0.0f);
                break;
            default:
                throw new RuntimeException("This line should not be reachable.");
        }
        
        normal = normal.Dot(rayDir) < 0.0f ? normal : -normal;

        return (normal, new Vec2D(u, v));
    }

    /// <summary>
    /// Compute ray intersection with caps: intended to be used just in the Intersect method.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns>The t-value of the intersection.</returns>
    public float? IntersectCaps(Ray ray)
    {
        Ray invRay = Transform.Inverse() * ray;
        Vec originVec = invRay.Origin.ToVec();
        
        float tCaps;
        
        float tUp = (1f - originVec.Z) / invRay.Dir.Z;
        float tDown = (-1f - originVec.Z) / invRay.Dir.Z;
        
        Point hitUp = invRay.At(tUp);
        Point hitDown = invRay.At(tDown);
        
        float radiusUp = hitUp.X * hitUp.X + hitUp.Y * hitUp.Y;
        float radiusDown = hitDown.X * hitDown.X + hitDown.Y * hitDown.Y;
        
        if (radiusUp <= 1f && radiusDown <= 1f)
        {
            float tLow = tUp < tDown ? tUp : tDown;
            float tHigh = tUp > tDown ? tUp : tDown;
            if (invRay.TMin < tLow && tLow < invRay.TMax) tCaps = tLow;
            else if (invRay.TMin < tHigh && tHigh < invRay.TMax) tCaps = tHigh;
            else return null;
        }
        else if (radiusUp <= 1f && radiusDown > 1f)
        {
            if (invRay.TMin < tUp && tUp < invRay.TMax) tCaps = tUp;
            else return null;
        }
        else if (radiusDown <= 1f && radiusUp > 1f)
        {
            if (invRay.TMin < tDown && tDown < invRay.TMax) tCaps = tDown;
            else return null;
        }
        else return null;

        return tCaps;
    }

    /// <summary>
    /// Compute ray intersection with lateral surface: intended to be used just in the Intersect method.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns>The t-value of the intersection.</returns>
    public float? IntersectLateralSurface(Ray ray)
    {
        Ray invRay = Transform.Inverse() * ray;
        Vec originVec = invRay.Origin.ToVec();
        
        float tLateral;
        
        float a = invRay.Dir.X * invRay.Dir.X + invRay.Dir.Y * invRay.Dir.Y;
        float b = 2.0f * (originVec.X * invRay.Dir.X + originVec.Y * invRay.Dir.Y);
        float c = originVec.X * originVec.X + originVec.Y * originVec.Y - 1.0f;

        float delta = b * b - 4.0f * a * c;
        float sqrtDelta;
        if (delta < 0.0f) return null;
        else if (Utils.CloseEnough(delta, 0.0f))
        {
            sqrtDelta = MathF.Sqrt(delta);
            float tSol = (-b - sqrtDelta) / (2.0f * a);
            if (invRay.TMin < tSol && tSol < invRay.TMax) tLateral = tSol;
            else return null;
        }
        else
        {
            sqrtDelta = MathF.Sqrt(delta);
            float tMin = (-b - sqrtDelta) / (2.0f * a);
            float tMax = (-b + sqrtDelta) / (2.0f * a);

            if (invRay.TMin < tMin && tMin < invRay.TMax) tLateral = tMin;
            else if (invRay.TMin < tMax && tMax < invRay.TMax) tLateral = tMax;
            else return null;
        }

        return tLateral;
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
        
        float tHit;
        
        // Compute intersections with caps 
        float? tCaps = IntersectCaps(ray);
        
        // Compute intersections with lateral surface
        float? tLateral = IntersectLateralSurface(invRay);
        
        // Compare the intersections and take the minimum
        if(tLateral.HasValue && tCaps.HasValue) tHit = tLateral.Value < tCaps.Value ? tCaps.Value : tLateral.Value;
        else if(tLateral.HasValue && tCaps == null) tHit = tLateral.Value;
        else if(tCaps.HasValue && tLateral == null) tHit = tCaps.Value;
        else return null;

        Point hitPoint = invRay.At(tHit);
        if (hitPoint.Z < -1.0f || hitPoint.Z > 1.0f) return null;
        (Normal normal, Vec2D uv) = CylinderNormalAndUV(hitPoint, invRay.Dir);

        return new HitRecord(this, Transform * hitPoint, (Transform * normal).Normalize(),
            uv, ray, tHit);
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