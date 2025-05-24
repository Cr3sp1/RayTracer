namespace Trace;

/// <summary>
/// Abstract class defining a Bidirectional Reflectance Distribution Function.
/// </summary>
public class Brdf
{
    public Pigment Pigment;

    // Constructor
    public Brdf(Pigment? pigment = null)
    {
        Pigment = pigment ?? new UniformPigment(Color.White);
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

    /// <summary>
    /// Scatter a ray from a surface.
    /// </summary>
    /// <param name="pcg"><c>Pcg</c> random number generator.</param>
    /// <param name="incomingDir"><c>Vec</c> representing the direction of the incident ray on the surface.</param>
    /// <param name="interactionPoint"><c>Point</c> of contact between the ray and the surface.</param>
    /// <param name="normal"><c>Normal</c> to the surface on point of contact.</param>
    /// <param name="depth"><c>int</c> representing the depth of the ray.</param>
    public virtual Ray ScatterRay(Pcg pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth) =>
        throw new NotImplementedException();
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

    /// <summary>
    /// Scatter a ray from a diffusive surface.
    /// </summary>
    /// <param name="pcg"><c>Pcg</c> random number generator.</param>
    /// <param name="incomingDir"><c>Vec</c> representing the direction of the incident ray on the surface.</param>
    /// <param name="interactionPoint"><c>Point</c> of contact between the ray and the surface.</param>
    /// <param name="normal"><c>Normal</c> to the surface on point of contact.</param>
    /// <param name="depth"><c>int</c> representing the depth of the ray.</param>
    public override Ray ScatterRay(Pcg pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
    {
        (Vec e1, Vec e2, Vec e3) = normal.CreateOnbFromZ();
        float cosThetaSq = pcg.RandomFloat();
        float cosTheta = MathF.Sqrt(cosThetaSq);
        float sinTheta = MathF.Sqrt(1.0f - cosThetaSq);
        float phi = 2.0f * MathF.PI * pcg.RandomFloat();

        return new Ray(origin: interactionPoint,
            dir: MathF.Cos(phi) * cosTheta * e1 + MathF.Sin(phi) * e2 + sinTheta * e3, tmin: 1e-3f,
            tmax: float.PositiveInfinity, depth: depth);
    }
}

/// <summary>
/// Class inheriting from <c>Brdf</c>, representing a perfectly reflective BRDF.
/// Members: <c>Pigment</c> where to evaluate the BRDF, <c>float</c> reflectance (default: 1.0).
/// </summary>
public class SpecularBrdf : Brdf
{
    public float ThresholdAngleRad;

    // Constructor
    public SpecularBrdf(Pigment? pigment = null, float? thresholdAngleRad = null) : base(pigment)
    {
        ThresholdAngleRad = thresholdAngleRad ?? MathF.PI / 1800.0f;
    }

    /// <summary>
    /// Evaluate the perfectly reflective BRDF for a specific pixel on a given surface.
    /// </summary>
    /// <param name="normal"><c>Normal</c> of the surface at point of 2D coordinates (u,v).</param>
    /// <param name="inDir"><c>Vec</c> representing the direction of the incident ray on the surface
    /// at point of 2D coordinates (u,v).</param>
    /// <param name="outDir"><c>Vec</c> representing the direction of the reflected ray from the surface
    /// at point of 2D coordinates (u,v).</param>
    /// <param name="uv"><c>Vec2D</c> representing coordinates of the pixel on the surface.</param>
    /// <returns></returns>
    public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
    {
        float thetaIn = MathF.Acos(normal.Dot(inDir));
        float thetaOut = MathF.Acos(normal.Dot(outDir));
        if (MathF.Abs(thetaIn - thetaOut) < ThresholdAngleRad) return Pigment.GetColor(uv);
        return Color.Black;
    }

    /// <summary>
    /// Scatter a ray from a reflective surface.
    /// </summary>
    /// <param name="pcg"><c>Pcg</c> random number generator.</param>
    /// <param name="incomingDir"><c>Vec</c> representing the direction of the incident ray on the surface.</param>
    /// <param name="interactionPoint"><c>Point</c> of contact between the ray and the surface.</param>
    /// <param name="normal"><c>Normal</c> to the surface on point of contact.</param>
    /// <param name="depth"><c>int</c> representing the depth of the ray.</param>
    public override Ray ScatterRay(Pcg pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
    {
        incomingDir.Normalize();
        normal.Normalize();
        float dotProd = normal.Dot(incomingDir);

        return new Ray(origin: interactionPoint,
            dir: incomingDir - 2.0f * dotProd * normal.ToVec(), tmin: 1e-5f,
            tmax: float.PositiveInfinity, depth: depth);
    }
}