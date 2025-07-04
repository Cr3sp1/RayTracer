namespace Trace;

// Class representing a cube (base cube has sides of length 2 parallel to the axes and is centered on the origin,
// face indexes follow a right-handed dice with face 0 oriented towards x-axis)
public class Cube : Shape
{
    // Constructor of the cube subject to an optional transformation and with an optional material
    public Cube(Transformation? transform = null, Material? material = null) : base(transform, material)
    {
        Materials = [Materials[0], Materials[0], Materials[0], Materials[0], Materials[0], Materials[0]];
        BBox = GetBoundingBox();
    }

    // Constructor of the cube subject to an optional transformation and with a list of materials
    public Cube(List<Material> materials, Transformation? transform = null) : base(transform,
        materials.Count > 0 ? materials[0] : new Material())
    {
        BBox = GetBoundingBox();
        
        
        Materials.Capacity = 6;
        for (int i = 1; i < 6; i++)
        {
            Materials.Add(materials.Count > i ? materials[i] : Materials[0]);
        }
    }


    // Return normal to the surface on Point p and coordinates on the surface for base cube
    public static (Normal, int, Vec2D) CubeNormalIndexUV(Point p, Vec rayDir)
    {
        // Determine the cube face
        int face;
        var absX = MathF.Abs(p.X);
        var absY = MathF.Abs(p.Y);
        var absZ = MathF.Abs(p.Z);
        if (absX > absZ)
        {
            if (absX > absY)
            {
                face = p.X > 0 ? 0 : 5;
            }
            else
            {
                face = p.Y > 0 ? 1 : 4;
            }
        }
        else
        {
            if (absY > absZ)
            {
                face = p.Y > 0 ? 1 : 4;
            }
            else
            {
                face = p.Z > 0 ? 2 : 3;
            }
        }

        float u, v;
        Normal normal;

        switch (face)
        {
            case 0:
                u = (p.Y + 1f) / 2f;
                v = (p.Z + 1f) / 2f;
                normal = rayDir.X < 0f ? new Normal(1f, 0f, 0f) : new Normal(-1f, 0f, 0f);
                break;
            case 1:
                u = (-p.X + 1f) / 2f;
                v = (p.Z + 1f) / 2f;
                normal = rayDir.Y < 0f ? new Normal(0f, 1f, 0f) : new Normal(0f, -1f, 0f);
                break;
            case 2:
                u = (p.X + 1f) / 2f;
                v = (p.Y + 1f) / 2f;
                normal = rayDir.Z < 0f ? new Normal(0f, 0f, 1f) : new Normal(0f, 0f, -1f);
                break;
            case 3:
                u = (p.X + 1f) / 2f;
                v = (-p.Y + 1f) / 2f;
                normal = rayDir.Z < 0f ? new Normal(0f, 0f, 1f) : new Normal(0f, 0f, -1f);
                break;
            case 4:
                u = (p.X + 1f) / 2f;
                v = (p.Z + 1f) / 2f;
                normal = rayDir.Y < 0f ? new Normal(0f, 1f, 0f) : new Normal(0f, -1f, 0f);
                break;
            case 5:
                u = (-p.Y + 1f) / 2f;
                v = (p.Z + 1f) / 2f;
                normal = rayDir.X < 0f ? new Normal(1f, 0f, 0f) : new Normal(-1f, 0f, 0f);
                break;
            default:
                throw new RuntimeException("This line should not be reachable.");
        }

        return (normal, face,  new Vec2D(u, v));
    }

    /// <summary>
    /// Method to compute the intersection between a ray and a <c>Cube</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns>Return a <c>HitRecord</c> containing details of intersection if the <c>Cube</c> intersects the <c>Ray</c>,
    /// and return null if the <c>Cube</c> doesn't intersect the <c>Ray</c>.</returns>
    public override HitRecord? Intersect(Ray ray)
    {
        if (BBox.HasValue)
        {
            if (!BBox.Value.DoesIntersect(ray)) return null;
        }

        Ray invRay = Transform.Inverse() * ray;
        float invInvDirX = 1.0f / invRay.Dir.X;
        float invInvDirY = 1.0f / invRay.Dir.Y;
        float tLow, tHigh, tLowY, tHighY;
        if (invInvDirX > 0f)
        {
            tLow = (-1 - invRay.Origin.X) * invInvDirX;
            tHigh = (1 - invRay.Origin.X) * invInvDirX;
        }
        else
        {
            tLow = (1 - invRay.Origin.X) * invInvDirX;
            tHigh = (-1 - invRay.Origin.X) * invInvDirX;
        }

        if (invInvDirY > 0f)
        {
            tLowY = (-1 - invRay.Origin.Y) * invInvDirY;
            tHighY = (1 - invRay.Origin.Y) * invInvDirY;
        }
        else
        {
            tLowY = (1 - invRay.Origin.Y) * invInvDirY;
            tHighY = (-1 - invRay.Origin.Y) * invInvDirY;
        }

        tLow = float.Max(tLow, tLowY);
        tHigh = float.Min(tHigh, tHighY);
        if (tLow > tHigh) return null;

        float invInvDirZ = 1.0f / invRay.Dir.Z;
        float tLowZ, tHighZ;
        if (invInvDirZ > 0f)
        {
            tLowZ = (-1 - invRay.Origin.Z) * invInvDirZ;
            tHighZ = (1 - invRay.Origin.Z) * invInvDirZ;
        }
        else
        {
            tLowZ = (1 - invRay.Origin.Z) * invInvDirZ;
            tHighZ = (-1 - invRay.Origin.Z) * invInvDirZ;
        }

        tLow = float.Max(tLow, tLowZ);
        tHigh = float.Min(tHigh, tHighZ);
        if (tLow > tHigh) return null;

        float tHit;
        if (invRay.TMin < tLow && tLow < invRay.TMax) tHit = tLow;
        else if (invRay.TMin < tHigh && tHigh < invRay.TMax) tHit = tHigh;
        else return null;

        Point hitPoint = invRay.At(tHit);
        (Normal normal, int faceIndex, Vec2D uv) = CubeNormalIndexUV(hitPoint, invRay.Dir);

        return new HitRecord(this, Transform * hitPoint, (Transform * normal).Normalize(), uv, ray, tHit, faceIndex);
    }

    /// <summary>
    /// Method to compute all the intersections between a <c>Ray</c> and a <c>Cube</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns> Return a <c>List</c> of <c>HitRecord</c> containing all the intersections between the <c>Ray</c> and
    /// a <c>Cube</c> from closest to <c>Ray</c> origin to furthest.</returns>
    public override List<HitRecord> AllIntersects(Ray ray)
    {
        if (BBox.HasValue)
        {
            if (!BBox.Value.DoesIntersect(ray)) return [];
        }

        var res = new List<HitRecord>(2);

        Ray invRay = Transform.Inverse() * ray;
        float invInvDirX = 1.0f / invRay.Dir.X;
        float invInvDirY = 1.0f / invRay.Dir.Y;
        float tLow, tHigh, tLowY, tHighY;
        if (invInvDirX > 0f)
        {
            tLow = (-1 - invRay.Origin.X) * invInvDirX;
            tHigh = (1 - invRay.Origin.X) * invInvDirX;
        }
        else
        {
            tLow = (1 - invRay.Origin.X) * invInvDirX;
            tHigh = (-1 - invRay.Origin.X) * invInvDirX;
        }

        if (invInvDirY > 0f)
        {
            tLowY = (-1 - invRay.Origin.Y) * invInvDirY;
            tHighY = (1 - invRay.Origin.Y) * invInvDirY;
        }
        else
        {
            tLowY = (1 - invRay.Origin.Y) * invInvDirY;
            tHighY = (-1 - invRay.Origin.Y) * invInvDirY;
        }

        tLow = float.Max(tLow, tLowY);
        tHigh = float.Min(tHigh, tHighY);
        if (tLow > tHigh) return [];

        float invInvDirZ = 1.0f / invRay.Dir.Z;
        float tLowZ, tHighZ;
        if (invInvDirZ > 0f)
        {
            tLowZ = (-1 - invRay.Origin.Z) * invInvDirZ;
            tHighZ = (1 - invRay.Origin.Z) * invInvDirZ;
        }
        else
        {
            tLowZ = (1 - invRay.Origin.Z) * invInvDirZ;
            tHighZ = (-1 - invRay.Origin.Z) * invInvDirZ;
        }

        tLow = float.Max(tLow, tLowZ);
        tHigh = float.Min(tHigh, tHighZ);
        if (tLow > tHigh) return [];

        if (invRay.TMin < tLow && tLow < invRay.TMax)
        {
            Point hitPoint = invRay.At(tLow);
            (Normal normal, int faceIndex, Vec2D uv) = CubeNormalIndexUV(hitPoint, invRay.Dir);
            res.Add(new HitRecord(this, Transform * hitPoint,
                (Transform * normal).Normalize(), uv, ray, tLow, faceIndex));
        }

        if (invRay.TMin < tHigh && tHigh < invRay.TMax)
        {
            Point hitPoint = invRay.At(tHigh);
            (Normal normal, int faceIndex, Vec2D uv) = CubeNormalIndexUV(hitPoint, invRay.Dir);
            res.Add(new HitRecord(this, Transform * hitPoint,
                (Transform * normal).Normalize(), uv, ray, tHigh, faceIndex));
        }

        return res;
    }


    /// <summary>
    /// Method that computes the axis-aligned bounding box containing the <c>Cube</c>.
    /// </summary>
    /// <returns> a <c>BoundingBox</c> containing the <c>Cube</c>.</returns>
    public sealed override BoundingBox? GetBoundingBox()
    {
        var bbox = new BoundingBox(-1f, -1f, -1f, 1f, 1f, 1f);
        return Transform * bbox;
    }
}