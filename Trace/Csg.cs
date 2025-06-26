namespace Trace;

// Class representing a shape obtained from the union, difference or intersection of two other shapes
public class Csg : Shape
{
    public readonly Shape ShapeA;
    public readonly Shape ShapeB;
    public readonly CsgType Type;

    // Constructor
    public Csg(Shape shapeA, Shape shapeB, CsgType type, Transformation? transform = null) :
        base(transform)
    {
        (ShapeA, ShapeB, Type) = (shapeA, shapeB, type);
        BBox = GetBoundingBox();
    }

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
        if (BBox.HasValue)
        {
            if (!BBox.Value.DoesIntersect(ray)) return [];
        }

        var validHits = new List<HitRecord>();
        Ray invRay = Transform.Inverse() * ray;
        var allHitsA = ShapeA.AllIntersects(invRay);
        var allHitsB = ShapeB.AllIntersects(invRay);

        // Add valid intersections with ShapeA
        foreach (var hitRecordA in allHitsA)
        {
            if (Type is CsgType.Union) validHits.Add(hitRecordA);
            int hitRecordAIsInB = IsInside(hitRecordA, allHitsB);
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
            int hitRecordBIsInA = IsInside(hitRecordB, allHitsA);
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
    /// Method to check if a hit is inside the other <c>Shape</c>.
    /// WARNING: Doesn't work properly at the intersection between shape surfaces, but error is vanishingly small.
    /// </summary>
    /// <param name="hit"><c>HitRecord</c> to be checked.</param>
    /// <param name="allHits">List of <c>HitRecord</c> that represents all the hits on the other <c>Shape</c>.</param>
    /// <returns></returns>
    public int IsInside(in HitRecord hit, in List<HitRecord> allHits)
    {
        int isInside = -1;
        foreach (var hitShape in allHits)
        {
            if (hitShape.T < hit.T) isInside = -isInside;
        }

        return isInside;
    }

    /// <summary>
    /// Method that computes the axis aligned bounding box containing the <c>Csg</c>.
    /// </summary>
    /// <returns> a <c>BoundingBox</c> containing the <c>Sphere</c>.</returns>
    public sealed override BoundingBox? GetBoundingBox()
    {
        var bboxA = ShapeA.GetBoundingBox();
        var bboxB = ShapeB.GetBoundingBox();

        BoundingBox bboxRes = new BoundingBox();

        switch (Type)
        {
            case CsgType.Fusion or CsgType.Union:
                if (bboxA == null || bboxB == null) return null;
                bboxRes.MinX = float.Min(bboxA.Value.MinX, bboxB.Value.MinX);
                bboxRes.MinY = float.Min(bboxA.Value.MinY, bboxB.Value.MinY);
                bboxRes.MinZ = float.Min(bboxA.Value.MinZ, bboxB.Value.MinZ);
                bboxRes.MaxX = float.Max(bboxA.Value.MaxX, bboxB.Value.MaxX);
                bboxRes.MaxY = float.Max(bboxA.Value.MaxY, bboxB.Value.MaxY);
                bboxRes.MaxZ = float.Max(bboxA.Value.MaxZ, bboxB.Value.MaxZ);
                break;
            case CsgType.Difference:
                if (bboxA == null) return null;
                bboxRes = bboxA.Value;
                break;
            case CsgType.Intersection:
                if (bboxA == null && bboxB == null) return null;
                if (bboxA == null || bboxB == null)
                {
                    bboxRes = bboxA ?? bboxB!.Value;
                }
                else
                {
                    bboxRes.MinX = float.Max(bboxA.Value.MinX, bboxB.Value.MinX);
                    bboxRes.MinY = float.Max(bboxA.Value.MinY, bboxB.Value.MinY);
                    bboxRes.MinZ = float.Max(bboxA.Value.MinZ, bboxB.Value.MinZ);
                    bboxRes.MaxX = float.Min(bboxA.Value.MaxX, bboxB.Value.MaxX);
                    bboxRes.MaxY = float.Min(bboxA.Value.MaxY, bboxB.Value.MaxY);
                    bboxRes.MaxZ = float.Min(bboxA.Value.MaxZ, bboxB.Value.MaxZ);

                    if (bboxRes.MaxX < bboxRes.MinX) bboxRes.MaxX = bboxRes.MinX;
                    if (bboxRes.MaxY < bboxRes.MinY) bboxRes.MaxY = bboxRes.MinY;
                    if (bboxRes.MaxZ < bboxRes.MinZ) bboxRes.MaxZ = bboxRes.MinZ;
                }

                break;
        }

        ShapeA.BBox = null;
        ShapeB.BBox = null;
        return Transform * bboxRes;
    }
}

public enum CsgType
{
    Fusion,
    Difference,
    Intersection,
    Union
}