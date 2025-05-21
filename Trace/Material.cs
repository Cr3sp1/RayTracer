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
    public Material(Pigment? pigment = null, Brdf? brdf = null)
    {
        EmittedRadiance = pigment ?? new UniformPigment(Color.Black);
        Brdf = brdf ?? new DiffuseBrdf(EmittedRadiance);
    }
}