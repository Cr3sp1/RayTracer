namespace Trace;

/// <summary>
/// Class defining a material, with members <c>Pigment</c> (default: uniform black)
/// and <c>Brdf</c> (default: diffuse BRDF).
/// </summary>
public class Material
{
    public Pigment EmittedRadiance;
    public Brdf Brdf;

    // Default constructor
    public Material(Brdf? brdf = null, Pigment? emittedRadiance = null)
    {
        Brdf = brdf ?? new DiffuseBrdf(new UniformPigment());
        EmittedRadiance = emittedRadiance ?? new UniformPigment();
    }
}