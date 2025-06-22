using Exceptions;

namespace Trace;

public class Scene
{
    // No need to use default constructor
    public InputStream InputFile;
    public Dictionary<string, Material> Materials = new();
    public Dictionary<string, float> FloatVariables = new();
    public HashSet<string> OverriddenVariables = new(); // Track externally-set variables
    public ICamera? SceneCamera = null;
    public World SceneWorld = new();

    // Expect functions
    public Scene(InputStream inputFile) => InputFile = inputFile;


    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is a <c>SymbolToken</c>.
    /// </summary>
    /// <param name="symbol">Expected symbol.</param>
    public void ExpectSymbol(char symbol)
    {
        var token = InputFile.ReadToken();
        if (token is not SymbolToken symbolToken || symbolToken.Symbol != symbol)
        {
            throw new GrammarException($"Expected symbol '{symbol}' instead of {token}.", token.Location);
        }
    }

    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is one of a list of <c>KeywordTokens</c>.
    /// </summary>
    /// <param name="keywords">Expected keywords.</param>
    /// <returns><c>Keyword</c> to be stored.</returns>
    public Keyword ExpectKeywords(List<Keyword> keywords)
    {
        var token = InputFile.ReadToken();
        if (token is not KeywordToken keywordToken)
        {
            throw new GrammarException($"Expected keyword instead of {token}.", token.Location);
        }

        if (!keywords.Contains(keywordToken.Keyword))
        {
            throw new GrammarException($"Expected one of [{string.Join(", ", keywords)}] instead of {token}.",
                token.Location);
        }

        return keywordToken.Keyword;
    }

    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is a <c>Float</c> or a variable storing a <c>Float</c>.
    /// </summary>
    /// <returns><c>Float</c> to be stored.</returns>
    public float ExpectNumber()
    {
        var token = InputFile.ReadToken();
        if (token is LiteralNumberToken numberToken)
        {
            return numberToken.Value;
        }

        if (token is IdentifierToken identifierToken)
        {
            var variableName = identifierToken.Identifier;
            if (!FloatVariables.TryGetValue(variableName, out var number))
            {
                throw new GrammarException($"Unknown variable '{variableName}'.", token.Location);
            }

            return number;
        }

        throw new GrammarException($"Expected number instead of {token}.", token.Location);
    }

    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is a <c>String</c>.
    /// </summary>
    /// <returns><c>String</c> to be stored.</returns>
    public string ExpectString()
    {
        var token = InputFile.ReadToken();
        if (token is not LiteralStringToken stringToken)
        {
            throw new GrammarException($"Expected string instead of {token}.", token.Location);
        }

        return stringToken.String;
    }

    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is an <c>IdentifierToken</c>.
    /// </summary>
    /// <returns><c>String</c> to be stored.</returns>
    public string ExpectIdentifier()
    {
        var token = InputFile.ReadToken();
        if (token is not IdentifierToken identifierToken)
        {
            throw new GrammarException($"Expected identifier instead of {token}.", token.Location);
        }

        return identifierToken.Identifier;
    }

    // Parse functions

    /// <summary>
    /// Parse a <c>Vec</c>: [x, y, z].
    /// </summary>
    /// <returns><c>Vec</c> to be stored.</returns>
    public Vec ParseVector()
    {
        ExpectSymbol('[');
        var x = ExpectNumber();
        ExpectSymbol(',');
        var y = ExpectNumber();
        ExpectSymbol(',');
        var z = ExpectNumber();
        ExpectSymbol(']');
        return new Vec(x, y, z);
    }

    /// <summary>
    /// Parse a <c>Color</c>: &lt;r, g, b&gt;
    /// </summary>
    /// <returns><c>Color</c> to be stored.</returns>
    public Color ParseColor()
    {
        ExpectSymbol('<');
        var r = ExpectNumber();
        ExpectSymbol(',');
        var g = ExpectNumber();
        ExpectSymbol(',');
        var b = ExpectNumber();
        ExpectSymbol('>');
        return new Color(r, g, b);
    }

    /// <summary>
    /// Parse a <c>Pigment</c>: Uniform(color), Checkered(color1, color2, numSquares) or Image(imageName).
    /// </summary>
    /// <returns><c>Pigment</c> to be stored.</returns>
    public Pigment ParsePigment()
    {
        var pigmentKeyword = ExpectKeywords([Keyword.Uniform, Keyword.Checkered, Keyword.Image]);
        Pigment result;

        switch (pigmentKeyword)
        {
            case Keyword.Uniform:
            {
                ExpectSymbol('(');
                var color = ParseColor();
                ExpectSymbol(')');
                result = new UniformPigment(color);
                break;
            }

            case Keyword.Checkered:
            {
                ExpectSymbol('(');
                var color1 = ParseColor();
                ExpectSymbol(',');
                var color2 = ParseColor();
                ExpectSymbol(',');
                var numSquares = (int)ExpectNumber();
                ExpectSymbol(')');
                result = new CheckeredPigment(color1, color2, numSquares);
                break;
            }

            case Keyword.Image:
            {
                ExpectSymbol('(');
                var imageName = ExpectString();
                var image = new HdrImage(imageName);
                ExpectSymbol(')');
                result = new ImagePigment(image);
                break;
            }

            default:
                // This should never happen due to ExpectKeywords, but needed for compiler
                throw new RuntimeException("This line should not be reachable.");
        }

        return result;
    }

    /// <summary>
    /// Parse a <c>Brdf</c>: Diffuse(pigment) or Specular(pigment).
    /// </summary>
    /// <returns><c>Brdf</c> to be stored.</returns>
    public Brdf ParseBrdf()
    {
        var brdfKeyword = ExpectKeywords([Keyword.Diffuse, Keyword.Specular]);
        Brdf result;

        ExpectSymbol('(');
        var pigment = ParsePigment();
        ExpectSymbol(')');

        switch (brdfKeyword)
        {
            case Keyword.Diffuse:
                result = new DiffuseBrdf(pigment);
                break;

            case Keyword.Specular:
                result = new SpecularBrdf(pigment);
                break;

            default:
                // This should never happen due to ExpectKeywords, but needed for compiler
                throw new RuntimeException("This line should not be reachable.");
        }

        return result;
    }

    /// <summary>
    /// Parse a <c>Material</c>: identifier(brdf, pigment).
    /// </summary>
    /// <returns><c>Material</c> to be stored.</returns>
    public (String, Material) ParseMaterial()
    {
        var materialName = ExpectIdentifier();
        ExpectSymbol('(');
        var brdf = ParseBrdf();
        ExpectSymbol(',');
        var emittedRadiance = ParsePigment();
        ExpectSymbol(')');
        var material = new Material(brdf, emittedRadiance);

        return (materialName, material);
    }

    /// <summary>
    /// Parse a <c>Transformation</c>: Identity, Translation(Vec), RotationX(Float), RotationY(Float), RotationZ(Float), Scaling(Vec).
    /// </summary>
    /// <returns><c>Transformation</c> to be stored.</returns>
    public Transformation ParseTransformation()
    {
        var result = new Transformation();

        while (true)
        {
            var transformationKeyword = ExpectKeywords(
            [
                Keyword.Identity, Keyword.Translation, Keyword.RotationX, Keyword.RotationY, Keyword.RotationZ,
                Keyword.Scaling
            ]);

            switch (transformationKeyword)
            {
                case Keyword.Identity:
                    break;

                case Keyword.Translation:
                    ExpectSymbol('(');
                    var vecTranslation = ParseVector();
                    ExpectSymbol(')');
                    result *= Transformation.Translation(vecTranslation);
                    break;

                case Keyword.RotationX:
                    ExpectSymbol('(');
                    var angleX = ExpectNumber();
                    ExpectSymbol(')');
                    result *= Transformation.RotationX(angleX);
                    break;

                case Keyword.RotationY:
                    ExpectSymbol('(');
                    var angleY = ExpectNumber();
                    ExpectSymbol(')');
                    result *= Transformation.RotationY(angleY);
                    break;

                case Keyword.RotationZ:
                    ExpectSymbol('(');
                    var angleZ = ExpectNumber();
                    ExpectSymbol(')');
                    result *= Transformation.RotationZ(angleZ);
                    break;

                case Keyword.Scaling:
                    ExpectSymbol('(');
                    var vecScale = ParseVector();
                    ExpectSymbol(')');
                    result *= Transformation.Scaling(vecScale);
                    break;

                default:
                    throw new RuntimeException("This line should not be reachable.");
            }

            // Look-ahead to see if there is another transformation chained
            var newToken = InputFile.ReadToken();
            if (newToken is not SymbolToken symbol || symbol.Symbol != '*')
            {
                InputFile.UnreadToken(newToken);
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Parse a <c>Plane</c>: Plane(transformation, material).
    /// </summary>
    /// <returns><c>Plane</c> to be stored.</returns>
    public Plane ParsePlane()
    {
        ExpectSymbol('(');
        var materialName = ExpectIdentifier();
        if (!Materials.TryGetValue(materialName, out var material))
        {
            throw new GrammarException($"Unknown material '{materialName}'.", InputFile.Location);
        }

        ExpectSymbol(',');
        var transformation = ParseTransformation();
        ExpectSymbol(')');

        return new Plane(transformation, material);
    }

    /// <summary>
    /// Parse a <c>Sphere</c>: Sphere(transformation, material).
    /// </summary>
    /// <returns><c>Sphere</c> to be stored.</returns>
    public Sphere ParseSphere()
    {
        ExpectSymbol('(');
        var materialName = ExpectIdentifier();
        if (!Materials.TryGetValue(materialName, out var material))
        {
            throw new GrammarException($"Unknown material '{materialName}'.", InputFile.Location);
        }

        ExpectSymbol(',');
        var transformation = ParseTransformation();
        ExpectSymbol(')');

        return new Sphere(transformation, material);
    }

    /// <summary>
    /// Parse a <c>ICamera</c>: Perspective (transformation, distance, aspectRatio) or Orthogonal (transformation, distance, aspectRatio).
    /// </summary>
    /// <returns><c>ICamera</c> to be stored.</returns>
    public ICamera ParseCamera()
    {
        ExpectSymbol('(');
        var cameraType = ExpectKeywords([Keyword.Orthogonal, Keyword.Perspective]);
        ExpectSymbol(',');
        var transformation = ParseTransformation();
        ExpectSymbol(',');
        var aspectRatio = ExpectNumber();
        ExpectSymbol(',');
        var distance = ExpectNumber();
        ExpectSymbol(')');

        ICamera camera;

        switch (cameraType)
        {
            case Keyword.Orthogonal:
                camera = new OrthogonalCamera(transformation, aspectRatio);
                break;

            case Keyword.Perspective:
                camera = new PerspectiveCamera(transformation, distance, aspectRatio);
                break;

            default:
                throw new RuntimeException("This line should not be reachable.");
        }

        return camera;
    }

    /// <summary>
    /// Parse the whole <c>Scene</c> until reaching the end of the file.
    /// </summary>
    /// <param name="externalVariables"><c>Dictionary</c> of external variables that can be passed to override those inside the scene file.</param>
    public void ParseScene(Dictionary<string, float>? externalVariables = null)
    {
        // Initialize with external variables if provided
        if (externalVariables != null)
        {
            FloatVariables = new Dictionary<string, float>(externalVariables);
            OverriddenVariables = new HashSet<string>(externalVariables.Keys);
        }

        while (true) // Until EOF is reached
        {
            var whichToken = InputFile.ReadToken();

            if (whichToken is StopToken) // Stop if EOF is reached
            {
                break;
            }

            if (whichToken is not KeywordToken keywordToken) // First expect a keyword
            {
                throw new GrammarException($"Expected keyword instead of '{whichToken}'.", InputFile.Location);
            }

            switch (keywordToken.Keyword) // See which keyword it is and register the corresponding variable
            {
                case Keyword.Float:
                    var floatName = ExpectIdentifier();
                    ExpectSymbol('(');
                    var floatValue = ExpectNumber();
                    ExpectSymbol(')');
                    if (!OverriddenVariables
                            .Contains(floatName)) // If the identifier is not among the variables to override
                    {
                        if (!FloatVariables.TryAdd(floatName,
                                floatValue)) // If it is inside the dictionary of variables in the input file
                        {
                            throw new GrammarException($"Variable '{floatName}' cannot be redefined.",
                                InputFile.Location);
                        }
                    }

                    break;

                case Keyword.Plane:
                    SceneWorld.AddShape(ParsePlane());
                    break;

                case Keyword.Sphere:
                    SceneWorld.AddShape(ParseSphere());
                    break;

                case Keyword.Material:
                    var (materialName, material) = ParseMaterial();
                    if (!Materials.TryAdd(materialName, material))
                    {
                        throw new GrammarException($"Material '{materialName}' cannot be redefined.",
                            InputFile.Location);
                    }

                    break;

                case Keyword.Camera:
                    if (SceneCamera != null)
                    {
                        throw new GrammarException("Scene camera already exists, cannot define two cameras.",
                            InputFile.Location);
                    }
                    else
                    {
                        SceneCamera = ParseCamera();
                    }

                    break;

                default:
                    throw new RuntimeException("This line should not be reachable.");
            }
        }
    }
}