namespace Trace;

// Class representing a generic shape
public class Shape
{
    // Transformation applied to the shape
    public readonly Transformation Transform = new Transformation();

    // Default constructor
    public Shape()
    {
    }

    // Constructor of the shape subject to a transformation
    public Shape(Transformation transform)
    {
        Transform = transform;
    }

    // Method to compute the intersection between a ray and a shape
    public virtual HitRecord Intersect(Ray ray)
    {
        throw new NotImplementedException();
    }
}

// Record struct containing information about ray and shape intersection
/// <summary>
/// <c>record struct</c> containing information about <c>Ray</c> and <c>Shape</c> intersection.
/// </summary>
/// <param name="WorldPoint"><c>Point</c> of contact between <c>Ray</c> and <c>Shape</c>.</param>
/// <param name="Normal"><c>Normal</c> to the surface on point of contact.</param>
/// <param name="SurfacePoint">Position of point of contact on the surface.</param>
/// <param name="T">Distance between point of contact and <c>Ray</c> origin.</param>
/// <param name="Ray"><c>Ray</c> that intersects the <c>Shape</c>.</param>
public record struct HitRecord(Point WorldPoint, Normal Normal, Vec2D SurfacePoint, float T, Ray Ray)
{
    /// <summary>
    /// Check if two <c>HitRecord</c> have the same components with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(HitRecord hr1, HitRecord hr2, float epsilon = 1e-5f) =>
        Point.CloseEnough(hr1.WorldPoint, hr2.WorldPoint, epsilon) &&
        Normal.CloseEnough(hr1.Normal, hr2.Normal, epsilon) &&
        Vec2D.CloseEnough(hr1.SurfacePoint, hr2.SurfacePoint, epsilon) &&
        Utils.CloseEnough(hr1.T, hr2.T, epsilon) && Ray.CloseEnough(hr1.Ray, hr2.Ray, epsilon);
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