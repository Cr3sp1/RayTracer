namespace Trace;

/// <summary>
/// Struct representing RGB colors, with R,G,B values represented as <c>float</c> values.
/// </summary>
public struct Color
{
    // Color fields
    public float R, G, B;

    // Constructor
    public Color(float r, float g, float b) => (R, G, B) = (r, g, b);

    // Sum between two Color objects
    public static Color operator +(Color col1, Color col2) =>
        new Color(col1.R + col2.R, col1.G + col2.G, col1.B + col2.B);

    // Product between scalar and Color
    public static Color operator *(float m, Color col) => new Color(m * col.R, m * col.G, m * col.B);

    // Element-wise product between two Color objects
    public static Color operator *(Color col1, Color col2) =>
        new Color(col1.R * col2.R, col1.G * col2.G, col1.B * col2.B);

    /// <summary>
    /// Check if two <c>Color</c> have the same rgb values with tolerance <c>epsilon</c>.
    /// </summary>
    public static bool CloseEnough(Color col1, Color col2, float epsilon = 1e-5f) =>
        Utils.CloseEnough(col1.R, col2.R, epsilon) && Utils.CloseEnough(col1.G, col2.G, epsilon) &&
        Utils.CloseEnough(col1.B, col2.B, epsilon);

    // Computes luminosity
    public float Luminosity() => (Math.Max(R, Math.Max(G, B)) + Math.Min(R, Math.Min(G, B))) / 2;


    public override string ToString() => $"<R:{R}, G:{G}, B:{B}>";
}