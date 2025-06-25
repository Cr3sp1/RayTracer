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
    public bool Intersects(in Ray ray)
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
}