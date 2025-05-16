namespace Trace;

/// <summary>
/// Abstract class defining a Bidirectional Reflectance Distribution Function.
/// </summary>
public class Brdf
{
    public Pigment Pigment;

    // Default constructor
    public Brdf()
    {
    }

    // Constructor passing a pigment
    public Brdf(Pigment pigment)
    {
        Pigment = pigment;
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
    public virtual Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
    {
        return new Color(0.0f, 0.0f, 0.0f);
    }
}

/// <summary>
/// Class inheriting from <c>Brdf</c>, representing a diffuse BRDF.
/// Members: <c>Pigment</c> where to evaluate the BRDF, <c>float</c> reflectance (default: 1.0).
/// </summary>
public class DiffuseBrdf : Brdf
{
    public float Reflectance;

    // Default constructor
    public DiffuseBrdf() : base()
    {
    }

    // Constructor passing a pigment and a reflectance
    public DiffuseBrdf(Pigment pigment, float reflectance = 1.0f) : base(pigment)
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