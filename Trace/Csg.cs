namespace Trace;

// Class representing a shape obtained from the union, difference or intersection of two other shapes
public class Csg : Shape
{
    public Shape ShapeA;
    public Shape ShapeB;
    public CsgType Type;

    // Constructor
    public Csg(Shape shapeA, Shape shapeB, CsgType type, Transformation? transform = null) : base(transform) =>
        (ShapeA, ShapeB, Type) = (shapeA, shapeB, type);

    /// <summary>
    /// Method to compute the intersection between a ray and a <c>Csg</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns>Return a <c>HitRecord</c> containing details of intersection if the <c>Csg</c> intersects the <c>Ray</c>,
    /// and return null if the <c>Csg</c> doesn't intersect the <c>Ray</c>.</returns>
    public override HitRecord? Intersect(Ray ray)
    {
        var allHits = AllIntersects(ray);
        return allHits.Count == 0 ? null : allHits[0];
    }


    /// <summary>
    /// Method to compute all the intersections between a <c>Ray</c> and a <c>Csg</c>.
    /// </summary>
    /// <param name="ray"><c>Ray</c> to check.</param>
    /// <returns> Return a <c>List</c> of <c>HitRecord</c> containing all the intersections between the <c>Ray</c> and
    /// a <c>Csg</c> from closest to <c>Ray</c> origin to furthest.</returns>
    public override List<HitRecord> AllIntersects(Ray ray)
    {
        var validHits = new List<HitRecord>();
        Ray invRay = Transform.Inverse() * ray;
        var allHits1 = ShapeA.AllIntersects(invRay);
        var allHits2 = ShapeB.AllIntersects(invRay);

        // Add valid intersections with ShapeA
        foreach (var hitRecord in allHits1)
        {
            if (IsInside(hitRecord, allHits2))
            {
                if (Type is CsgType.Intersection) validHits.Add(hitRecord);
            }
            else
            {
                if (Type is CsgType.Union or CsgType.Difference) validHits.Add(hitRecord);
            }
        }

        // Add valid intersections with ShapeB
        foreach (var hitRecord in allHits2)
        {
            if (IsInside(hitRecord, allHits1))
            {
                if (Type is CsgType.Intersection or CsgType.Difference) validHits.Add(hitRecord);
            }
            else
            {
                if (Type is CsgType.Union) validHits.Add(hitRecord);
            }
        }

        // Sort hits and modify them accordingly
        validHits.Sort(new CloserHit());
        for (int i = 0; i < validHits.Count; ++i)
        {
            validHits[i] = validHits[i] with
            {
                WorldPoint = Transform * validHits[i].WorldPoint,
                Normal = (Transform * validHits[i].Normal).Normalize()
            };
        }

        return validHits;
    }

    // Given a hit on a shape and all the hits on the other shape, return if the hit on the first shape falls inside the
    // second shape
    public bool IsInside(in HitRecord hit, in List<HitRecord> allHits)
    {
        bool isInside = false;
        foreach (var hitShape in allHits)
        {
            if (hitShape.T < hit.T) isInside = !isInside;
        }

        return isInside;
    }
}

public enum CsgType
{
    Union,
    Difference,
    Intersection
}