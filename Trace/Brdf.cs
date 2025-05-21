namespace Trace;

/// <summary>
/// Abstract class defining a Bidirectional Reflectance Distribution Function.
/// </summary>
public class Brdf
{
    public Pigment Pigment;

    // Constructor passing a pigment
    public Brdf(Pigment? pigment = null)
    {
        Pigment = pigment ?? new Pigment();
    }

    /// <summary>
    /// Evaluate the BRDF for a specific pixel on a given surface.
    /// </summary>
    /// <param name="normal"><c>Normal</c> of the surface at point of 2D coordinates (u,v).</param>
    /// <param name="inDir"><c>Vec</c> representing the direction of the incident ray on the surface
    /// at point of 2D coordinates (u,v).</param>
    /// <param name="outDir"><c>Vec</c> representing the direction of the diffused ray from the surface
    /// at point of 2D coordinates (u,v).</param>
    /// <param name="uv"><c>Vec2D</c> representing coordinates of the pixel on the surface.</param>
    /// <returns></returns>
    public virtual Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv) => Color.Black;
}

/// <summary>
/// Class inheriting from <c>Brdf</c>, representing a diffuse BRDF.
/// Members: <c>Pigment</c> where to evaluate the BRDF, <c>float</c> reflectance (default: 1.0).
/// </summary>
public class DiffuseBrdf : Brdf
{
    public float Reflectance;

    // Constructor passing a pigment and a reflectance
    public DiffuseBrdf(Pigment? pigment = null, float reflectance = 1.0f) : base(pigment)
    {
        Reflectance = reflectance;
    }

    /// <summary>
    /// Evaluate the diffuse BRDF for a specific pixel on a given surface.
    /// </summary>
    /// <param name="normal"><c>Normal</c> of the surface at point of 2D coordinates (u,v).</param>
    /// <param name="inDir"><c>Vec</c> representing the direction of the incident ray on the surface
    /// at point of 2D coordinates (u,v).</param>
    /// <param name="outDir"><c>Vec</c> representing the direction of the diffused ray from the surface
    /// at point of 2D coordinates (u,v).</param>
    /// <param name="uv"><c>Vec2D</c> representing coordinates of the pixel on the surface.</param>
    /// <returns></returns>
    public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
    {
        return (float)(Reflectance / Math.PI) * Pigment.GetColor(uv);
    }
}