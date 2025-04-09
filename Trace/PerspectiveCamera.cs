namespace Trace;

public class PerspectiveCamera : ICamera
{
    
    public float Distance;
    public float AspectRatio;
    public Transformation Transform;
    
    // Constructor: if not specified the associated transformation is identity
    public PerspectiveCamera(Transformation? t = null, float distance = 1.0f, float aspectRatio = 1.0f)
    {
        Transform = t ?? new Transformation();
        Distance = distance;
        AspectRatio = aspectRatio;
    }
    
    /// <summary>
    /// Fire a ray through point of coordinates (u,v) on the 2D screen (origin: bottom-left corner)
    /// via a perspective camera.
    /// </summary>
    /// <param name="u">x-coordinate of the screen: <c>float</c> in [0,1]</param>
    /// <param name="v">y-coordinate of the screen: <c>float</c> in [0,1]</param>
    /// <returns><c>Ray</c> fired through (u,v) on the screen</returns>
    public Ray FireRay(float u, float v)
    {
        throw new NotImplementedException();
    }
}