using System.Diagnostics;
using SixLabors.ImageSharp.Formats.Png;

namespace Trace;

/// <summary>
/// Abstract class defining a renderer.
/// </summary>
public class Renderer
{
    public World Scene = new World();

    // Default constructor
    public Renderer()
    {
    }

    // Costructor passing a scene
    public Renderer(World world) => Scene = world;

    /// <summary>
    /// Return the color that solves the rendering equation on a given pixel.
    /// </summary>
    /// <param name="ray"><c>Ray</c> taken as input for the renderer.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public virtual Color Render(Ray ray) => new Color();
}

/// <summary>
/// Class inheriting from Renderer, representing a test renderer
/// that always returns a specific <c>Color</c>.
/// </summary>
public class TestRenderer : Renderer
{
    public Color TestColor;

    // Constructor passing a scene and a color
    public TestRenderer(World world, Color color) : base(world) => TestColor = color;

    /// <summary>
    /// Always return the same color.
    /// </summary>
    /// <param name="ray"><c>Ray</c> taken as input for the renderer.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public override Color Render(Ray ray) => TestColor;
}

/// <summary>
/// Class inheriting from Renderer, representing an on-off renderer:
/// if <c>Ray</c> intersects a shape in the scene return white, else return black.
/// </summary>
public class OnOffRenderer : Renderer
{
    // Constructor passing a scene
    public OnOffRenderer(World world) : base(world)
    {
    }

    /// <summary>
    /// Return white if <c>Ray</c> intersects a shape in the scene return white, else return black.
    /// </summary>
    /// <param name="ray"><c>Ray</c> taken as input for the renderer.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public override Color Render(Ray ray)
    {
        if (Scene.IntersectAll(ray) != null)
        {
            return Color.White;
        }

        return Color.Black;
    }
}

/// <summary>
/// Class inheriting from Renderer, representing a flat renderer:
/// if <c>Ray</c> intersects a shape in the scene return the color of the shape in the intersection, else return black.
/// </summary>
public class FlatRenderer : Renderer
{
    public Color BackgroundColor;

    // Constructor passing a scene and optionally a background color
    public FlatRenderer(World world, Color? backgroundColor = null) : base(world)
    {
        BackgroundColor = backgroundColor ?? Color.Black;
    }

    /// <summary>
    /// If <c>Ray</c> intersects a shape in the scene return the color of the shape in the intersection, else return
    /// black.
    /// </summary>
    /// <param name="ray"><c>Ray</c> taken as input for the renderer.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public override Color Render(Ray ray)
    {
        var hit = Scene.IntersectAll(ray);
        if (hit.HasValue)
        {
            var hitVal = hit.Value;
            Material mat = hitVal.Shape.Materials[hitVal.SurfaceIndex];
            return mat.Brdf.Pigment.GetColor(hitVal.SurfacePoint);
        }

        return BackgroundColor;
    }
}

// <summary>
/// Class inheriting from Renderer, representing a path tracer:
/// if <c>Ray</c> intersects a shape in the scene return the solution of the rendering equation, else return black.
/// </summary>
public class PathTracer : Renderer
{
    public Color BackgroundColor;
    public Pcg Pcg;
    public int NumRays;
    public int MaxDepth;
    public int RussianRouletteLimit;

    // Constructor passing a scene and optionally a background color, the random number generator, the number of rays
    // to generate for the integration, the maximum ray depth, the limit beyond which to use Russian Roulette
    public PathTracer(World world, int numRays = 10, int maxDepth = 10, int russianRouletteLimit = 2, Pcg? pcg = null,
        Color? backgroundColor = null) : base(world)
    {
        NumRays = numRays;
        MaxDepth = maxDepth;
        RussianRouletteLimit = russianRouletteLimit;
        Pcg = pcg ?? new Pcg();
        BackgroundColor = backgroundColor ?? Color.Black;
    }

    /// <summary>
    /// If <c>Ray</c> intersects a shape in the scene return the solution of the rendering equation, else return
    /// black.
    /// </summary>
    /// <param name="ray"><c>Ray</c> taken as input for the renderer.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public override Color Render(Ray ray)
    {
        // Return black color if the maximum depth for recursion is exceeded
        if (ray.Depth > MaxDepth) return Color.Black;

        // Find the closest intersection between ray and the scene
        var hit = Scene.IntersectAll(ray);
        if (!hit.HasValue) return BackgroundColor;

        // Register properties of the surface at the intersection
        var hitVal = hit.Value;
        var hitMaterial = hitVal.Shape.Materials[hitVal.SurfaceIndex];
        var hitColor = hitMaterial.Brdf.Pigment.GetColor(hitVal.SurfacePoint);
        var emittedRadiance = hitMaterial.EmittedRadiance.GetColor(hitVal.SurfacePoint);
        var hitColorLum = MathF.Max(MathF.Max(hitColor.R, hitColor.G), hitColor.B);

        // Russian roulette to avoid infinite recursion
        if (ray.Depth >= RussianRouletteLimit)
        {
            var q = MathF.Max(0.05f, 1.0f - hitColorLum); // Probability to kill recursion
            if (Pcg.RandomFloat() < q)
                return emittedRadiance; // Kill recursion
            hitColor = 1.0f / (1.0f - q) * hitColor; // Reweight luminosity to compensate for lost rays
        }

        // Monte Carlo integration
        var cumRadiance = new Color(0.0f, 0.0f, 0.0f);

        // Do it only if the surface reflects some light, unless it would be useless
        if (hitColorLum > 0.0f)
        {
            for (int rayIndex = 0; rayIndex < NumRays; rayIndex++)
            {
                // Generate new scattered ray
                var newRay = hitMaterial.Brdf.ScatterRay(Pcg, hitVal.Ray.Dir, hitVal.WorldPoint, hitVal.Normal,
                    ray.Depth + 1);
                // Trace the new ray
                var newRadiance = Render(newRay);
                // Update the radiance accumulator, modulated by the surface color
                cumRadiance += hitColor * newRadiance;
            }
        }

        // Return emitted light + averaged reflected light
        return emittedRadiance + 1.0f / NumRays * cumRadiance;
    }
    
    /// <summary>
    /// Class inheriting from Renderer, representing a test renderer for anti-aliasing
    /// that always returns black.
    /// </summary>
    public class TestAntiAliasing : Renderer
    {
        public int NumRays;

        // Constructor passing a scene and a color
        public TestAntiAliasing(World world) : base(world) => NumRays = 0;

        /// <summary>
        /// Always return black.
        /// </summary>
        /// <param name="ray"><c>Ray</c> taken as input for the renderer.</param>
        /// <returns><c>Color</c> black.</returns>
        public override Color Render(Ray ray)
        {
            Point rayOnScreen = ray.At(1f);
            Debug.Assert(Utils.CloseEnough(rayOnScreen.X, 0.0f, 1E-03F));
            Debug.Assert(rayOnScreen.Y >= -1.0f && rayOnScreen.Y <= 1.0f);
            Debug.Assert(rayOnScreen.Z >= -1.0f && rayOnScreen.Z <= 1.0f);
            
            NumRays += 1;
            return new Color(0.0f, 0.0f, 0.0f);
        }
    }
}