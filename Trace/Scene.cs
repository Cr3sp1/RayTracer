namespace Trace;

/// <summary>
/// Class representing a scene to be rendered that is constructed from an input file
/// </summary>
public class Scene
{
    public InputStream InputFile;
    public Dictionary<string, Material> MaterialVariables = new();
    public Dictionary<string, Shape> ShapeVariables = new();
    public Dictionary<string, float> FloatVariables = new();
    public HashSet<string> OverriddenVariables = new(); // Track externally-set variables
    public ICamera? SceneCamera;
    public World SceneWorld = new();


    /// <summary>
    /// Constructs a Scene object with a given input token stream.
    /// </summary>
    /// <param name="inputFile">The token stream to parse from.</param>
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
    /// Read a token from the <c>InputStream</c> and check that it is present in a list of <c>KeywordTokens</c>.
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
            throw new GrammarException($"Expected one of [\'{string.Join("\', \'", keywords)}\'] instead of {token}.",
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
                throw new GrammarException($"Unknown float variable '{variableName}'.", token.Location);
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
        var pigmentKeyword = ExpectKeywords([Keyword.Uniform, Keyword.Checkered, Keyword.Striped, Keyword.Image]);
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

            case Keyword.Striped:
            {
                ExpectSymbol('(');
                var color1 = ParseColor();
                ExpectSymbol(',');
                var color2 = ParseColor();
                ExpectSymbol(',');
                var numStripes = (int)ExpectNumber();
                ExpectSymbol(',');
                var stripeDirection = ExpectKeywords([Keyword.Vertical, Keyword.Horizontal]);
                var isVertical = stripeDirection switch
                {
                    Keyword.Vertical => true,
                    Keyword.Horizontal => false,
                    _ => throw new RuntimeException("This line should not be reachable.")
                };
                ExpectSymbol(')');
                result = new StripedPigment(color1, color2, numStripes, isVertical);
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

            // Look ahead to see if there is another transformation chained
            var newToken = InputFile.ReadToken();
            if (newToken is SymbolToken { Symbol: '*' }) continue;
            InputFile.UnreadToken(newToken);
            break;
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
        if (!MaterialVariables.TryGetValue(materialName, out var material))
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
        if (!MaterialVariables.TryGetValue(materialName, out var material))
        {
            throw new GrammarException($"Unknown material '{materialName}'.", InputFile.Location);
        }

        ExpectSymbol(',');
        var transformation = ParseTransformation();
        ExpectSymbol(')');

        return new Sphere(transformation, material);
    }

    /// <summary>
    /// Parse a <c>Cube</c>: Cube(transformation, material).
    /// </summary>
    /// <returns><c>Cube</c> to be stored.</returns>
    public Cube ParseCube()
    {
        ExpectSymbol('(');

        var materials = new List<Material>(6);
        // Look ahead to see if a list of materials is present
        var newToken = InputFile.ReadToken();
        if (newToken is SymbolToken { Symbol: '[' })
        {
            while (true)
            {
                var materialName = ExpectIdentifier();
                if (!MaterialVariables.TryGetValue(materialName, out var material))
                {
                    throw new GrammarException($"Unknown material '{materialName}'.", InputFile.Location);
                }

                materials.Add(material);
                var newNewToken = InputFile.ReadToken();
                if (newNewToken is SymbolToken { Symbol: ',' }) continue;
                InputFile.UnreadToken(newNewToken);
                break;
            }

            ExpectSymbol(']');
        }
        else
        {
            InputFile.UnreadToken(newToken);
            var materialName = ExpectIdentifier();
            if (!MaterialVariables.TryGetValue(materialName, out var material))
            {
                throw new GrammarException($"Unknown material '{materialName}'.", InputFile.Location);
            }

            materials.Add(material);
        }


        ExpectSymbol(',');
        var transformation = ParseTransformation();
        ExpectSymbol(')');

        return new Cube(materials, transformation);
    }

    /// <summary>
    /// Parse a <c>Cylinder</c>: Cylinder(transformation, material).
    /// </summary>
    /// <returns><c>Cylinder</c> to be stored.</returns>
    public Cylinder ParseCylinder()
    {
        ExpectSymbol('(');

        var materials = new List<Material>(6);
        // Look ahead to see if a list of materials is present
        var newToken = InputFile.ReadToken();
        if (newToken is SymbolToken { Symbol: '[' })
        {
            while (true)
            {
                var materialName = ExpectIdentifier();
                if (!MaterialVariables.TryGetValue(materialName, out var material))
                {
                    throw new GrammarException($"Unknown material '{materialName}'.", InputFile.Location);
                }

                materials.Add(material);
                var newNewToken = InputFile.ReadToken();
                if (newNewToken is SymbolToken { Symbol: ',' }) continue;
                InputFile.UnreadToken(newNewToken);
                break;
            }

            ExpectSymbol(']');
        }
        else
        {
            InputFile.UnreadToken(newToken);
            var materialName = ExpectIdentifier();
            if (!MaterialVariables.TryGetValue(materialName, out var material))
            {
                throw new GrammarException($"Unknown material '{materialName}'.", InputFile.Location);
            }

            materials.Add(material);
        }

        ExpectSymbol(',');
        var transformation = ParseTransformation();
        ExpectSymbol(')');

        return new Cylinder(materials, transformation);
    }

    /// <summary>
    /// Parse a <c>Csg</c>: Csg(shapeA, shapeB, csgType, transformation).
    /// </summary>
    /// <returns><c>Csg</c> to be stored.</returns>
    public Csg ParseCsg()
    {
        ExpectSymbol('(');

        var shapeAName = ExpectIdentifier();
        if (!ShapeVariables.TryGetValue(shapeAName, out var shapeA))
        {
            throw new GrammarException($"Unknown shape '{shapeAName}'.", InputFile.Location);
        }

        ExpectSymbol(',');

        var shapeBName = ExpectIdentifier();
        if (!ShapeVariables.TryGetValue(shapeBName, out var shapeB))
        {
            throw new GrammarException($"Unknown shape '{shapeAName}'.", InputFile.Location);
        }

        ExpectSymbol(',');

        var csgType = ExpectKeywords([Keyword.Fusion, Keyword.Difference, Keyword.Intersection]) switch
        {
            Keyword.Fusion => CsgType.Fusion,
            Keyword.Intersection => CsgType.Intersection,
            Keyword.Difference => CsgType.Difference,
            _ => throw new RuntimeException("This line should not be reachable.")
        };
        ExpectSymbol(',');

        var transformation = ParseTransformation();
        ExpectSymbol(')');

        return new Csg(shapeA, shapeB, csgType, transformation);
    }


    /// <summary>
    /// Parse a <c>Shape</c>: identifier(...).
    /// </summary>
    /// <returns><c>Shape</c> to be stored.</returns>
    public (string, Shape) ParseShape()
    {
        var shapeName = ExpectIdentifier();
        ExpectSymbol('(');
        var shapeType = ExpectKeywords([Keyword.Sphere, Keyword.Cube, Keyword.Cylinder, Keyword.Csg]);
        Shape shape;

        switch (shapeType)
        {
            case Keyword.Sphere:
                shape = ParseSphere();
                break;

            case Keyword.Cube:
                shape = ParseCube();
                break;

            case Keyword.Cylinder:
                shape = ParseCylinder();
                break;

            case Keyword.Csg:
                shape = ParseCsg();
                break;

            default:
                throw new RuntimeException("This line should not be reachable.");
        }

        ExpectSymbol(')');
        return (shapeName, shape);
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

            InputFile.UnreadToken(whichToken);
            var keyword = ExpectKeywords([
                Keyword.Float, Keyword.Plane, Keyword.Sphere, Keyword.Cube, Keyword.Cylinder, Keyword.Csg,
                Keyword.Material, Keyword.Shape, Keyword.Camera
            ]);


            switch (keyword) // See which keyword it is and register the corresponding variable
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

                case Keyword.Cube:
                    SceneWorld.AddShape(ParseCube());
                    break;

                case Keyword.Cylinder:
                    SceneWorld.AddShape(ParseCylinder());
                    break;

                case Keyword.Csg:
                    SceneWorld.AddShape(ParseCsg());
                    break;

                case Keyword.Material:
                    var (materialName, material) = ParseMaterial();
                    if (!MaterialVariables.TryAdd(materialName, material))
                    {
                        throw new GrammarException($"Material '{materialName}' cannot be redefined.",
                            InputFile.Location);
                    }

                    break;

                case Keyword.Shape:
                    var (shapeName, shape) = ParseShape();
                    if (!ShapeVariables.TryAdd(shapeName, shape))
                    {
                        throw new GrammarException($"Shape '{shapeName}' cannot be redefined.",
                            InputFile.Location);
                    }

                    break;

                case Keyword.Camera:
                    if (SceneCamera != null)
                        throw new GrammarException("Scene camera already exists, cannot define two cameras.",
                            InputFile.Location);
                    SceneCamera = ParseCamera();


                    break;

                default:
                    throw new RuntimeException("This line should not be reachable.");
            }
        }
    }
}