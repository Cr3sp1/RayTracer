namespace Trace;

// Class representing a generic shape
public class Shape
{
    // Transformation applied to the shape
    public readonly Transformation Transform;
    public readonly List<Material> Materials = new List<Material>(1);
    public BoundingBox? BBox = null;

    // Constructor of the shape subject to a transformation and with a material
    public Shape(Transformation? transform = null, Material? material = null)
    {
        Transform = transform ?? new Transformation();
        Materials.Add(material ?? new Material());
    }

    /// <summary>
    /// Method to compute the intersection closest to the origin between a <c>Ray</c> and a <c>Shape</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual HitRecord? Intersect(Ray ray)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Method to compute all the intersections between a <c>Ray</c> and a <c>Shape</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual List<HitRecord> AllIntersects(Ray ray)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Method that computes the axis-aligned bounding box containing the <c>Shape</c>.
    /// </summary>
    /// <returns> a <c>BoundingBox</c> containing the  <c>Shape</c> if it is possible, otherwise <c>null</c>.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual BoundingBox? GetBoundingBox()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// <c>record struct</c> containing information about <c>Ray</c> and <c>Shape</c> intersection.
/// </summary>
/// <param name="Shape"><c>Shape</c> intersected by the <c>Ray</c>.</param>
/// <param name="WorldPoint"><c>Point</c> of contact between <c>Ray</c> and <c>Shape</c>.</param>
/// <param name="Normal"><c>Normal</c> to the surface on point of contact.</param>
/// <param name="SurfacePoint">Position of point of contact on the surface.</param>
/// <param name="Ray"><c>Ray</c> that intersects the <c>Shape</c>.</param>
/// <param name="T">Distance between point of contact and <c>Ray</c> origin.</param>
/// <param name="SurfaceIndex">Index of the surface that intersects the <c>Ray</c>.</param>
public record struct HitRecord(
    Shape Shape,
    Point WorldPoint,
    Normal Normal,
    Vec2D SurfacePoint,
    Ray Ray,
    float T,
    int SurfaceIndex = 0)
{
    /// <summary>
    /// Check if two <c>HitRecord</c> have the same components with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(HitRecord hr1, HitRecord hr2, float epsilon = 1e-5f) =>
        hr1.Shape == hr2.Shape &&
        Point.CloseEnough(hr1.WorldPoint, hr2.WorldPoint, epsilon) &&
        Normal.CloseEnough(hr1.Normal, hr2.Normal, epsilon) &&
        Vec2D.CloseEnough(hr1.SurfacePoint, hr2.SurfacePoint, epsilon) &&
        hr1.SurfaceIndex == hr2.SurfaceIndex &&
        Ray.CloseEnough(hr1.Ray, hr2.Ray, epsilon) && Utils.CloseEnough(hr1.T, hr2.T, epsilon);
}

// Record struct representing 2d vector
public record struct Vec2D(float U, float V)
{
    /// <summary>
    /// Check if two <c>Vec2D</c> have the same components with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(Vec2D v1, Vec2D v2, float epsilon = 1e-5f) =>
        Utils.CloseEnough(v1.U, v2.U, epsilon) && Utils.CloseEnough(v1.V, v2.V, epsilon);
}

// Compare HitRecord by T; allows sorting HitRecord objects from closest to furthest from ray origin
public class CloserHit : Comparer<HitRecord>
{
    public override int Compare(HitRecord hit1, HitRecord hit2)
    {
        return (hit1.T - hit2.T) switch
        {
            < 0 => -1,
            > 0 => 1,
            _ => 0
        };
    }
}