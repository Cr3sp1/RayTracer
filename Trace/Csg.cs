namespace Trace;

// Class representing a shape obtained from the union, difference or intersection of two other shapes
public class Csg : Shape
{
    public Shape ShapeA;
    public Shape ShapeB;
    public CsgType Type;
    public static bool Efficient = true;

    // Constructor
    public Csg(Shape shapeA, Shape shapeB, CsgType type, Transformation? transform = null) :
        base(transform) => (ShapeA, ShapeB, Type) = (shapeA, shapeB, type);

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
        var allHitsA = ShapeA.AllIntersects(invRay);
        var allHitsB = ShapeB.AllIntersects(invRay);

        // Add valid intersections with ShapeA
        foreach (var hitRecordA in allHitsA)
        {
            if (Type is CsgType.Union) validHits.Add(hitRecordA);
            int hitRecordAIsInB = Efficient ? EfficientIsInside(hitRecordA, allHitsB) : ShapeB.IsInside(hitRecordA);
            switch (Type)
            {
                case CsgType.Fusion or CsgType.Difference:
                    if (hitRecordAIsInB is -1 or 0) validHits.Add(hitRecordA);
                    break;
                case CsgType.Intersection:
                    if (hitRecordAIsInB is 1 or 0) validHits.Add(hitRecordA);
                    break;
            }
        }

        // Add valid intersections with ShapeB
        foreach (var hitRecordB in allHitsB)
        {
            if (Type is CsgType.Union) validHits.Add(hitRecordB);
            int hitRecordBIsInA = Efficient ? EfficientIsInside(hitRecordB, allHitsA) : ShapeA.IsInside(hitRecordB);
            switch (Type)
            {
                case CsgType.Fusion:
                    if (hitRecordBIsInA is -1) validHits.Add(hitRecordB);
                    break;
                case CsgType.Difference or CsgType.Intersection:
                    if (hitRecordBIsInA is 1) validHits.Add(hitRecordB);
                    break;
            }
        }

        // Sort hits and modify them accordingly
        validHits.Sort(new CloserHit());
        for (int i = 0; i < validHits.Count; ++i)
        {
            validHits[i] = validHits[i] with
            {
                Ray = ray,
                WorldPoint = Transform * validHits[i].WorldPoint,
                Normal = (Transform * validHits[i].Normal).Normalize()
            };
        }

        return validHits;
    }

    /// <summary>
    /// Method to check if a <c>HitRecord</c> falls inside a <c>Csg</c>.
    /// </summary>
    /// <param name="hit"><c>HitRecord</c> to check.</param>
    /// <returns>1 if it falls in the <c>Csg</c>, -1 if it falls outside the <c>Csg</c>, and 0 if it falls
    /// on the surface of the <c>Csg</c></returns>
    public override int IsInside(in HitRecord hit)
    {
        var invHit = hit with
        {
            WorldPoint = Transform.Inverse() * hit.WorldPoint,
            Normal = (Transform.Inverse() * hit.Normal).Normalize(),
            Ray = Transform.Inverse() * hit.Ray
        };

        return Type switch
        {
            CsgType.Fusion or CsgType.Union => int.Max(ShapeA.IsInside(invHit), ShapeB.IsInside(invHit)),
            CsgType.Difference => ShapeB.IsInside(invHit) switch
            {
                1 => -1,
                -1 => ShapeA.IsInside(invHit),
                0 => ShapeA.IsInside(invHit) == -1 ? -1 : 0,
                _ => throw new RuntimeException("Shape.IsInside returned a value outside range [-1, 0, 1]!")
            },
            CsgType.Intersection => int.Min(ShapeA.IsInside(invHit), ShapeB.IsInside(invHit)),
            _ => throw new RuntimeException("Invalid Csg.Type!")
        };
    }

    /// <summary>
    /// Efficient method to check if a hit is inside the other <c>Shape</c>.
    /// WARNING: Doesn't work properly at the intersection between shape surfaces, but error is vanishingly small.
    /// </summary>
    /// <param name="hit"><c>HitRecord</c> to be checked.</param>
    /// <param name="allHits">List of <c>HitRecord</c> that represents all the hits on the other <c>Shape</c>.</param>
    /// <returns></returns>
    public int EfficientIsInside(in HitRecord hit, in List<HitRecord> allHits)
    {
        int isInside = -1;
        foreach (var hitShape in allHits)
        {
            if (hitShape.T < hit.T) isInside = -isInside;
        }

        return isInside;
    }
}

public enum CsgType
{
    Fusion,
    Difference,
    Intersection,
    Union
}