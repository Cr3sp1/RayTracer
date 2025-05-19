namespace Trace;

/// <summary>
/// Abstract class defining a surface pigment.
/// </summary>
public class Pigment
{
    // Default constructor
    public Pigment()
    {
    }

    /// <summary>
    /// Return the color of pixel (u,v) on the surface.
    /// </summary>
    /// <param name="uv"><c>Vec2D</c> representing coordinates of the pixel on the surface.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public virtual Color GetColor(Vec2D uv)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Class inheriting from <c>Pigment</c>, representing a uniform surface pigmentation.
/// Member: <c>Color</c> of the uniform pigment.
/// </summary>
public class UniformPigment : Pigment
{
    public Color Col;

    // Default constructor
    public UniformPigment() : base()
    {
    }

    // Constructor of a uniform pigment specifying the color
    public UniformPigment(Color color)
    {
        Col = color;
    }

    /// <summary>
    /// Return the color of pixel (u,v) on the surface for a uniformly-pigmented surface.
    /// </summary>
    /// <param name="uv"><c>Vec2D</c> representing coordinates of the pixel on the surface.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public override Color GetColor(Vec2D uv)
    {
        return Col;
    }
}

/// <summary>
/// Class inheriting from <c>Pigment</c>, representing a checkered surface pigmentation.
/// Members: the two <c>Color</c> types of the checkered pigment,
/// the <c>int</c> number of rows/columns of the checkered pattern (numbers of rows and columns must be equal).
/// </summary>
public class CheckeredPigment : Pigment
{
    public Color Col1, Col2;
    public int NumSquares;

    // Default constructor
    public CheckeredPigment() : base()
    {
    }

    // Constructor of a uniform pigment specifying the color
    public CheckeredPigment(Color color1, Color color2, int numSquares)
    {
        Col1 = color1;
        Col2 = color2;
        NumSquares = numSquares;
    }

    /// <summary>
    /// Return the color of pixel (u,v) on the surface for a checkered surface.
    /// </summary>
    /// <param name="uv"><c>Vec2D</c> representing coordinates of the pixel on the surface.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public override Color GetColor(Vec2D uv)
    {
        int u =  (int)Math.Floor(uv.U * NumSquares);
        int v = (int)Math.Floor(uv.V * NumSquares);
        return ((u + v) % 2 == 0) ? Col1 : Col2;
    }
}

/// <summary>
/// Class inheriting from <c>Pigment</c>, representing a textured surface pigmentation given by a PFM image.
/// Member: <c>HdrImage</c> representing the texture.
/// </summary>
public class ImagePigment : Pigment
{
    public HdrImage Image;

    // Default constructor
    public ImagePigment() : base()
    {
    }

    // Constructor of a uniform pigment specifying the color
    public ImagePigment(HdrImage image)
    {
        Image = image;
    }

    /// <summary>
    /// Return the color of pixel (u,v) on the surface for a Pfm image wrapping surface.
    /// </summary>
    /// <param name="uv"><c>Vec2D</c> representing coordinates of the pixel on the surface.</param>
    /// <returns><c>Color</c> of the pixel.</returns>
    public override Color GetColor(Vec2D uv)
    {
        int u = (int)(Image.Width * uv.U);
        int v = (int)(Image.Height * uv.V);
        if (u >= Image.Width) u = Image.Width - 1;
        if (v >= Image.Height) v = Image.Height - 1;
        return Image.GetPixel(u, v);
    }
}