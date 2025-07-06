namespace Trace;

/// <summary>
/// Class representing a cylinder: the basic cylinder has the z-axis as axis and is centered at the origin, with length 2 and radius 1
/// </summary>
public class Cylinder : Shape
{
    // Constructor of the cylinder subject to an optional transformation and with an optional material
    public Cylinder(Transformation? transform = null, Material? material = null) : base(transform, material)
    {
        Materials = [Materials[0], Materials[0], Materials[0]];
        BBox = GetBoundingBox();
    }

    // Constructor of the cube subject to an optional transformation and with a list of materials
    public Cylinder(List<Material> materials, Transformation? transform = null) : base(transform,
        materials.Count > 0 ? materials[0] : new Material())
    {
        BBox = GetBoundingBox();


        Materials.Capacity = 3;
        for (int i = 1; i < 3; i++)
        {
            Materials.Add(materials.Count > i ? materials[i] : Materials[0]);
        }
    }

    // Return normal to the surface on Point p and coordinates on the surface for base cylinder
    public static (Normal, int, Vec2D) CylinderNormalIndexUV(Point p, Vec rayDir)
    {
        // Determine the cylinder surface: 0=lateral, 1=up-cap, 2=down-cap 
        int face;
        if (Utils.CloseEnough(p.Z, 1.0f)) face = 1;
        else if (Utils.CloseEnough(p.Z, -1.0f)) face = 2;
        else face = 0;

        float u, v;
        Normal normal;

        switch (face)
        {
            case 0:
                u = MathF.Atan2(p.Y, p.X) / (2.0f * MathF.PI);
                if (u < 0.0f) u += 1.0f;
                v = (p.Z + 1.0f) / 2.0f;
                normal = new Normal(p.X, p.Y, 0.0f);
                break;
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
            default:
                throw new RuntimeException("This line should not be reachable.");
        }

        normal = normal.Dot(rayDir) < 0.0f ? normal : -normal;

        return (normal, face, new Vec2D(u, v));
    }

    /// <summary>
    /// Compute ray intersection with caps: intended to be used just in the Intersect method.
    /// </summary>
    /// <param name="invRay"><c>Ray</c> to check.</param>
    /// <returns>The t-value of the intersection.</returns>
    public float? IntersectCaps(Ray invRay)
    {
        Vec originVec = invRay.Origin.ToVec();
        if (invRay.Dir.Z == 0.0f) return null;

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
    /// <param name="invRay"><c>Ray</c> to check.</param>
    /// <returns>The t-value of the intersection.</returns>
    public float? IntersectLateralSurface(Ray invRay)
    {
        Vec originVec = invRay.Origin.ToVec();

        float tLateral;

        float a = invRay.Dir.X * invRay.Dir.X + invRay.Dir.Y * invRay.Dir.Y;
        float b = 2.0f * (originVec.X * invRay.Dir.X + originVec.Y * invRay.Dir.Y);
        float c = originVec.X * originVec.X + originVec.Y * originVec.Y - 1.0f;

        float delta = b * b - 4.0f * a * c;
        if (delta <= 0.0f) return null;

        float sqrtDelta = MathF.Sqrt(delta);
        float tMin = (-b - sqrtDelta) / (2.0f * a);
        float hMin = invRay.At(tMin).Z;
        float tMax = (-b + sqrtDelta) / (2.0f * a);
        float hMax = invRay.At(tMax).Z;

        if (invRay.TMin < tMin && tMin < invRay.TMax && -1 < hMin && hMin < 1) tLateral = tMin;
        else if (invRay.TMin < tMax && tMax < invRay.TMax && -1 < hMax && hMax < 1) tLateral = tMax;
        else return null;

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

        float tHit;

        // Compute intersections with caps 
        float? tCaps = IntersectCaps(invRay);

        // Compute intersections with the lateral surface
        float? tLateral = IntersectLateralSurface(invRay);

        // Compare the intersections and take the minimum
        if (tLateral.HasValue && tCaps.HasValue) tHit = float.Min(tLateral.Value, tCaps.Value);
        else if (tLateral.HasValue && tCaps == null) tHit = tLateral.Value;
        else if (tCaps.HasValue && tLateral == null) tHit = tCaps.Value;
        else return null;

        Point hitPoint = invRay.At(tHit);
        (Normal normal, int faceIndex, Vec2D uv) = CylinderNormalIndexUV(hitPoint, invRay.Dir);

        return new HitRecord(this, Transform * hitPoint, (Transform * normal).Normalize(),
            uv, ray, tHit, faceIndex);
    }

    /// <summary>
    /// Method to compute all the intersections between a <c>Ray</c> and a <c>Cylinder</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns> Return a <c>List</c> of <c>HitRecord</c> containing all the intersections between the <c>Ray</c> and
    /// a <c>Cylinder</c> from closest to <c>Ray</c> origin to furthest.</returns>
    public override List<HitRecord> AllIntersects(Ray ray)
    {
        if (BBox.HasValue)
        {
            if (!BBox.Value.DoesIntersect(ray)) return [];
        }

        var res = new List<HitRecord>(2);

        Ray invRay = Transform.Inverse() * ray;
        Vec originVec = invRay.Origin.ToVec();

        // Compute intersections with caps if there are any
        float tUp = (1f - originVec.Z) / invRay.Dir.Z;
        float tDown = (-1f - originVec.Z) / invRay.Dir.Z;

        Point hitUp = invRay.At(tUp);
        Point hitDown = invRay.At(tDown);

        (Normal normalUp, int faceIndexUp, Vec2D uvUp) = CylinderNormalIndexUV(hitUp, invRay.Dir);
        (Normal normalDown, int faceIndexDown, Vec2D uvDown) = CylinderNormalIndexUV(hitDown, invRay.Dir);

        float radiusUp = hitUp.X * hitUp.X + hitUp.Y * hitUp.Y;
        float radiusDown = hitDown.X * hitDown.X + hitDown.Y * hitDown.Y;

        if (radiusUp <= 1f && invRay.TMin < tUp && tUp < invRay.TMax)
        {
            res.Add(new HitRecord(this, Transform * hitUp,
                (Transform * normalUp).Normalize(), uvUp, ray, tUp, faceIndexUp));
        }

        if (radiusDown <= 1f && invRay.TMin < tDown && tDown < invRay.TMax)
        {
            res.Add(new HitRecord(this, Transform * hitDown,
                (Transform * normalDown).Normalize(), uvDown, ray, tDown, faceIndexDown));
            if (res.Count == 2) return res;
        }

        // Compute intersections with lateral surface if there are any
        float a = invRay.Dir.X * invRay.Dir.X + invRay.Dir.Y * invRay.Dir.Y;
        float b = 2.0f * (originVec.X * invRay.Dir.X + originVec.Y * invRay.Dir.Y);
        float c = originVec.X * originVec.X + originVec.Y * originVec.Y - 1.0f;

        float delta = b * b - 4.0f * a * c;
        if (delta > 0f)
        {
            float sqrtDelta = MathF.Sqrt(delta);
            float tMin = (-b - sqrtDelta) / (2.0f * a);
            float hMin = invRay.At(tMin).Z;
            float tMax = (-b + sqrtDelta) / (2.0f * a);
            float hMax = invRay.At(tMax).Z;

            if (invRay.TMin < tMin && tMin < invRay.TMax && -1 < hMin && hMin < 1)
            {
                Point hitLateral = invRay.At(tMin);
                (Normal normal, int faceIndex, Vec2D uv) = CylinderNormalIndexUV(hitLateral, invRay.Dir);
                res.Add(new HitRecord(this, Transform * hitLateral,
                    (Transform * normal).Normalize(), uv, ray, tMin, faceIndex));
                if (res.Count == 2) return res;
            }

            if (invRay.TMin < tMax && tMax < invRay.TMax && -1 < hMax && hMax < 1)
            {
                Point hitLateral = invRay.At(tMax);
                (Normal normal, int faceIndex, Vec2D uv) = CylinderNormalIndexUV(hitLateral, invRay.Dir);
                res.Add(new HitRecord(this, Transform * hitLateral,
                    (Transform * normal).Normalize(), uv, ray, tMax, faceIndex));
                if (res.Count == 2) return res;
            }
        }

        return res;
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