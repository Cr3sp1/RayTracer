namespace Trace;

public class Renderer
{
    public World Scene = new World();

    public Renderer()
    {
    }

    public Renderer(World world) => Scene = world;

    public virtual Color Render(Ray ray) => new Color();
}

public class TestRenderer : Renderer
{
    public Color TestColor;
    public TestRenderer(World world, Color color) : base(world) => TestColor = color;
    public override Color Render(Ray ray) => TestColor;
}


/// <summary>
/// On-off renderer: if <c>Ray</c> intersects the scene return white, else return black.
/// </summary>
/// <param name="ray"><c>Ray</c> taken as input for the renderer.</param>
/// <returns><c>Color</c> of the pixel.</returns>
public class OnOffRenderer : Renderer
{
    public OnOffRenderer(World world) : base(world) { }
    
    public override Color Render(Ray ray)
    {
        if (Scene.IntersectAll(ray) != null)
        {
            return new Color(1.0f, 1.0f, 1.0f);
        }
        return new Color(0.0f, 0.0f, 0.0f);
    }
}