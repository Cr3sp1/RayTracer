namespace Trace;

// Class representing the world containing all shapes
public class World
{
    // World fields
    public List<Shape> Shapes = new List<Shape>();

    // Add a Shape to the World
    public void AddShape(Shape shape)
    {
        Shapes.Add(shape);
    }

    /// <summary>
    /// Method to compute the intersection between a ray and a set of surfaces.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns>Return a <c>HitRecord</c> containing details of intersection if the <c>Ray</c> intersects the set,
    /// and return null otherwise.</returns>
    public HitRecord? IntersectAll(Ray ray)
    {
        HitRecord? closestHit = null;
        float closestDist = float.MaxValue;

        foreach (var shape in Shapes)
        {
            var intersection = shape.Intersect(ray);
            if (intersection is not HitRecord hit) continue;
            if (closestHit is null || hit.T < closestDist)
            {
                closestDist = hit.T;
                closestHit = hit;
            }
        }

        return closestHit;
    }
}