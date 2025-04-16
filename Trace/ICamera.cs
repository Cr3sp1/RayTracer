namespace Trace;

/// <summary>
/// Interface representing a Camera object. Inheritors: Orthogonal Camera and Perspective Camera.
/// </summary>
public interface ICamera
{
    public Ray FireRay(float u, float v);
}