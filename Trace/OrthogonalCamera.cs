namespace Trace;

/// <summary>
/// Implementation of Camera Interface that represents an orthogonal projection:
/// it preserves parallelism and lengths. It contains the aspect ratio of the screen (<c>float</c>)
/// and a <c>Transformation</c> to allow different orientations in space.
/// </summary>
public class OrthogonalCamera : ICamera
{
    public float AspectRatio;
    public Transformation Transform;

    // Constructor: if not specified, the associated transformation is identity
    public OrthogonalCamera(Transformation? t = null, float aspectRatio = 1.0f)
    {
        Transform = t ?? new Transformation();
        AspectRatio = aspectRatio;
    }

    /// <summary>
    /// Fire a ray through point of coordinates (u,v) on the 2D screen (origin: bottom-left corner)
    /// via an orthogonal camera.
    /// </summary>
    /// <param name="u">x-coordinate of the screen: <c>float</c> in [0,1]</param>
    /// <param name="v">y-coordinate of the screen: <c>float</c> in [0,1]</param>
    /// <returns><c>Ray</c> fired through (u,v) on the screen</returns>
    public Ray FireRay(float u, float v)
    {
        var origin = new Point(-1.0f, (1.0f - 2.0f * u) * AspectRatio, 2.0f * v - 1.0f);
        return Transform * new Ray(origin: origin, dir: Vec.XAxis);
    }
    
    public float GetAspectRatio() => AspectRatio;
}