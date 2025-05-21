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
/// if <c>Ray</c> intersects the scene return white, else return black.
/// </summary>
public class OnOffRenderer : Renderer
{
    // Constructor passing a scene
    public OnOffRenderer(World world) : base(world) { }
    
    /// <summary>
    /// Return the color that solves the rendering equation on a given pixel using an on-off renderer.
    /// </summary>
    /// <param name="ray"><c>Ray</c> taken as input for the renderer.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public override Color Render(Ray ray)
    {
        if (Scene.IntersectAll(ray) != null)
        {
            return new Color(1.0f, 1.0f, 1.0f);
        }
        return new Color(0.0f, 0.0f, 0.0f);
    }
}