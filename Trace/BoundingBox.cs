using System.Numerics;

namespace Trace;

/// <summary>
/// Struct representing an axis-aligned bounding box
/// </summary>
public struct BoundingBox(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
{
    public float MinX = minX, MinY = minY, MinZ = minZ, MaxX = maxX, MaxY = maxY, MaxZ = maxZ;

    /// <summary>
    /// Method that finds whether a <c>Ray</c> intersects the axis-aligned bounding box.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns><c>true</c> if the <c>Ray</c> intersects the bounding box, otherwise <c>false</c>.</returns>
    public bool DoesIntersect(in Ray ray)
    {
        float invDirX = 1.0f / ray.Dir.X;
        float invDirY = 1.0f / ray.Dir.Y;
        float tLow, tHigh, tLowY, tHighY;
        if (invDirX > 0f)
        {
            tLow = (MinX - ray.Origin.X) * invDirX;
            tHigh = (MaxX - ray.Origin.X) * invDirX;
        }
        else
        {
            tLow = (MaxX - ray.Origin.X) * invDirX;
            tHigh = (MinX - ray.Origin.X) * invDirX;
        }

        if (invDirY > 0f)
        {
            tLowY = (MinY - ray.Origin.Y) * invDirY;
            tHighY = (MaxY - ray.Origin.Y) * invDirY;
        }
        else
        {
            tLowY = (MaxY - ray.Origin.Y) * invDirY;
            tHighY = (MinY - ray.Origin.Y) * invDirY;
        }

        tLow = float.Max(tLow, tLowY);
        tHigh = float.Min(tHigh, tHighY);
        if (tLow > tHigh) return false;

        float invDirZ = 1.0f / ray.Dir.Z;
        float tLowZ, tHighZ;
        if (invDirZ > 0f)
        {
            tLowZ = (MinZ - ray.Origin.Z) * invDirZ;
            tHighZ = (MaxZ - ray.Origin.Z) * invDirZ;
        }
        else
        {
            tLowZ = (MaxZ - ray.Origin.Z) * invDirZ;
            tHighZ = (MinZ - ray.Origin.Z) * invDirZ;
        }

        tLow = float.Max(tLow, tLowZ);
        tHigh = float.Min(tHigh, tHighZ);
        if (tLow > tHigh) return false;

        tLow = float.Max(tLow, ray.TMin);
        tHigh = float.Min(tHigh, ray.TMax);
        return tLow <= tHigh;
    }

    /// <summary>
    /// Apply a <c>Transformation</c> to a <c>BoundingBox</c>.
    /// </summary>
    /// <param name="t"><c>Transformation</c> to apply.</param>
    /// <param name="b"><c>BoundingBox</c> to transform,</param>
    /// <returns>transformed <c>BoundingBox</c></returns>
    public static BoundingBox operator *(in Transformation t, in BoundingBox b)
    {
        // Calculate all vertices
        var vertices = new List<Point>(8);
        vertices.Add(new Point(b.MinX, b.MinY, b.MinZ));
        vertices.Add(new Point(b.MinX, b.MinY, b.MaxZ));
        vertices.Add(new Point(b.MinX, b.MaxY, b.MaxZ));
        vertices.Add(new Point(b.MinX, b.MaxY, b.MinZ));
        vertices.Add(new Point(b.MaxX, b.MinY, b.MinZ));
        vertices.Add(new Point(b.MaxX, b.MinY, b.MaxZ));
        vertices.Add(new Point(b.MaxX, b.MaxY, b.MaxZ));
        vertices.Add(new Point(b.MaxX, b.MaxY, b.MinZ));

        // Transform vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = t * vertices[i];
        }

        // Find new min and max
        float minX = float.PositiveInfinity;
        float minY = float.PositiveInfinity;
        float minZ = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float maxY = float.NegativeInfinity;
        float maxZ = float.NegativeInfinity;
        foreach (var vertex in vertices)
        {
            minX = Math.Min(minX, vertex.X);
            minY = Math.Min(minY, vertex.Y);
            minZ = Math.Min(minZ, vertex.Z);
            maxX = Math.Max(maxX, vertex.X);
            maxY = Math.Max(maxY, vertex.Y);
            maxZ = Math.Max(maxZ, vertex.Z);
        }

        return new BoundingBox(minX, minY, minZ, maxX, maxY, maxZ);
    }

    /// <summary>
    /// Check if two <c>BoundingBox</c> have the same components with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(BoundingBox bb1, BoundingBox bb2, float epsilon = 1e-5f)
    {
        return Utils.CloseEnough(bb1.MinX, bb2.MinX) && Utils.CloseEnough(bb1.MaxX, bb2.MaxX) &&
               Utils.CloseEnough(bb1.MinY, bb2.MinY) && Utils.CloseEnough(bb1.MaxY, bb2.MaxY) &&
               Utils.CloseEnough(bb1.MinZ, bb2.MinZ) && Utils.CloseEnough(bb1.MaxZ, bb2.MaxZ);
    }
}