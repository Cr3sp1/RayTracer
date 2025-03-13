namespace Trace;

// Color struct 
public struct Color
{
    // Color fields
    public float R, G, B;

    // Constructor
    public Color(float r, float g, float b) => (R, G, B) = (r, g, b);

    // Sum between two Color objects
    public static Color operator +(Color col1, Color col2) => new Color(col1.R + col2.R, col1.G + col2.G, col1.B + col2.B);

    // Product between scalar and Color
    public static Color operator *(float m, Color col) => new Color(m * col.R, m * col.G, m * col.B);

    // Element-wise product between two Color objects
    public static Color operator *(Color col1, Color col2) => new Color(col1.R * col2.R, col1.G * col2.G, col1.B * col2.B);

    // Checks if two Color objects are equal with a tolerance epsilon
    public static bool CloseEnough(Color col1, Color col2, float epsilon = 1e-5f) =>
        Utils.CloseEnough(col1.R, col2.R, epsilon) && Utils.CloseEnough(col1.G, col2.G, epsilon) &&
        Utils.CloseEnough(col1.B, col2.B, epsilon);
    
    public override string ToString()  => $"<R:{R}, G:{G}, B:{B}>";
}