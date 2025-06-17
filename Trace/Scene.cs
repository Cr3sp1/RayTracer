namespace Trace;

public class Scene
{
    // No need to use default constructor
    public Dictionary<string, Material> Materials { get; } = new();
    public Dictionary<string, float> FloatVariables { get; set; } = new();
    public HashSet<string> OverriddenVariables { get; set; } = new(); // Track externally-set variables
    public ICamera? SceneCamera { get; set; } = null;
    public World SceneWorld { get; } = new();

    // Expect functions
    
    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is a <c>SymbolToken</c>.
    /// </summary>
    /// <param name="inputFile"> <c>InputStream</c> that contains the scene description of the input file.</param>
    /// <param name="symbol">Expected symbol.</param>
    public void ExpectSymbol(InputStream inputFile, char symbol)
    {
        var token = inputFile.ReadToken();
        if (token is not SymbolToken symbolToken || symbolToken.Symbol != symbol)
        {
            throw new GrammarException($"Expected symbol '{symbol}' instead of '{token}'.", token.Location);
        }
    }

    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is one of a list of <c>KeywordTokens</c>.
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <param name="keywords">Expected keywords.</param>
    /// <returns><c>Keyword</c> to be stored.</returns>
    public Keyword ExpectKeywords(InputStream inputFile, List<Keyword> keywords)
    {
        var token = inputFile.ReadToken();
        if (token is not KeywordToken keywordToken)
        {
            throw new GrammarException($"Expected keyword instead of '{token}'.", token.Location);
        }

        if (!keywords.Contains(keywordToken.Keyword))
        {
            throw new GrammarException($"Expected one of [{string.Join(", ", keywords)}] instead of '{token}'.", token.Location);
        }
        return keywordToken.Keyword;
    }

    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is a <c>Float</c> or a variable storing a <c>Float</c>.
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>Float</c> to be stored.</returns>
    public float ExpectNumber(InputStream inputFile)
    {
        var token = inputFile.ReadToken();
        if (token is LiteralNumberToken numberToken)
        {
            return numberToken.Value;
        }

        if (token is IdentifierToken identifierToken)
        {
            var variableName = identifierToken.Identifier;
            if (!FloatVariables.ContainsKey(variableName))
            {
                throw new GrammarException($"Unknown variable '{variableName}'.", token.Location);
            }
            return FloatVariables[variableName];
        }
        
        throw new GrammarException($"Expected number instead of '{token}'.", token.Location);
    }

    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is a <c>String</c>.
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>String</c> to be stored.</returns>
    public string ExpectString(InputStream inputFile)
    {
        var token = inputFile.ReadToken();
        if (token is not LiteralStringToken stringToken)
        {
            throw new GrammarException($"Expected string instead of '{token}'.", token.Location);
        }
        return stringToken.String;
    }

    /// <summary>
    /// Read a token from the <c>InputStream</c> and check that it is an <c>IdentifierToken</c>.
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>String</c> to be stored.</returns>
    public string ExpectIdentifier(InputStream inputFile)
    {
        var token = inputFile.ReadToken();
        if (token is not IdentifierToken identifierToken)
        {
            throw new GrammarException($"Expected identifier instead of '{token}'.", token.Location);
        }
        return identifierToken.Identifier;
    }
    
    // Parse functions

    /// <summary>
    /// Parse a <c>Vec</c>: [x, y, z].
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>Vec</c> to be stored.</returns>
    public Vec ParseVector(InputStream inputFile)
    {
        ExpectSymbol(inputFile, '[');
        var x = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ',');
        var y = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ',');
        var z = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ']');
        return new Vec(x, y, z);
    }

    /// <summary>
    /// Parse a <c>Color</c>: &lt;r, g, b&gt;.
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>Color</c> to be stored.</returns>
    public Color ParseColor(InputStream inputFile)
    {
        ExpectSymbol(inputFile, '<');
        var r = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ',');
        var g = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ',');
        var b = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, '>');
        return new Color(r, g, b);
    }

    /// <summary>
    /// Parse a <c>Pigment</c>: Uniform(color), Checkered(color1, color2) or Image(imageName).
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>Pigment</c> to be stored.</returns>
    public Pigment ParsePigment(InputStream inputFile)
    {
        var pigmentKeyword = ExpectKeywords(inputFile, new List<Keyword> {Keyword.Uniform, Keyword.Checkered, Keyword.Image});
        Pigment result;

        switch (pigmentKeyword)
        {
            case Keyword.Uniform:
            {
                ExpectSymbol(inputFile, '(');
                var color = ParseColor(inputFile);
                ExpectSymbol(inputFile, ')');
                result = new UniformPigment(color);
                break;
            }

            case Keyword.Checkered:
            {
                ExpectSymbol(inputFile, '(');
                var color1 = ParseColor(inputFile);
                ExpectSymbol(inputFile, ',');
                var color2 = ParseColor(inputFile);
                ExpectSymbol(inputFile, ')');
                result = new CheckeredPigment(color1, color2);
                break;
            }

            case Keyword.Image:
            {
                ExpectSymbol(inputFile, '(');
                var imageName = ExpectString(inputFile);
                var image = new HdrImage(imageName);
                ExpectSymbol(inputFile, ')');
                result = new ImagePigment(image);
                break;
            }
            
            default:
                // This should never happen due to ExpectKeywords, but needed for compiler
                throw new InvalidOperationException("This line should not be reachable.");
        }

        return result;
    }

    /// <summary>
    /// Parse a <c>Brdf</c>: Diffuse(pigment) or Specular(pigment).
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>Brdf</c> to be stored.</returns>
    public Brdf ParseBrdf(InputStream inputFile)
    {
        var brdfKeyword = ExpectKeywords(inputFile, new List<Keyword> {Keyword.Diffuse, Keyword.Specular});
        Brdf result;
        Pigment pigment;

        switch (brdfKeyword)
        {
            case Keyword.Diffuse:
                ExpectSymbol(inputFile, '(');
                pigment = ParsePigment(inputFile);
                ExpectSymbol(inputFile, ')');
                result = new DiffuseBrdf(pigment);
                break;
            
            case Keyword.Specular:
                ExpectSymbol(inputFile, '(');
                pigment = ParsePigment(inputFile);
                ExpectSymbol(inputFile, ')');
                result = new SpecularBrdf(pigment);
                break;
            
            default:
                // This should never happen due to ExpectKeywords, but needed for compiler
                throw new InvalidOperationException("This line should not be reachable.");
        }
        
        return result;
    }

    /// <summary>
    /// Parse a <c>Material</c>: identifier(brdf, pigment).
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>Material</c> to be stored.</returns>
    public (String, Material) ParseMaterial(InputStream inputFile)
    {
        var materialName = ExpectString(inputFile);
        ExpectSymbol(inputFile, '(');
        var brdf = ParseBrdf(inputFile);
        ExpectSymbol(inputFile, ',');
        var emittedRadiance = ParsePigment(inputFile);
        ExpectSymbol(inputFile, ')');
        var material = new Material(brdf, emittedRadiance);
        
        return (materialName, material);
    }

    /// <summary>
    /// Parse a <c>Transformation</c>: Identity, Translation(Vec), RotationX(Float), RotationY(Float), RotationZ(Float), Scaling(Vec).
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>Transformation</c> to be stored.</returns>
    public Transformation ParseTransformation(InputStream inputFile)
    {
        var result = new Transformation();

        while (true)
        {
            var transformationKeyword = ExpectKeywords(inputFile,
                new List<Keyword>
                {
                    Keyword.Identity, Keyword.Translation, Keyword.RotationX, Keyword.RotationY, Keyword.RotationZ,
                    Keyword.Scaling
                });

            switch (transformationKeyword)
            {
                case Keyword.Identity:
                    break;
                
                case Keyword.Translation:
                    ExpectSymbol(inputFile, '(');
                    var vecTranslation = ParseVector(inputFile);
                    ExpectSymbol(inputFile, ')');
                    result *= Transformation.Translation(vecTranslation);
                    break;
                
                case Keyword.RotationX:
                    ExpectSymbol(inputFile, '(');
                    var angleX = ExpectNumber(inputFile);
                    ExpectSymbol(inputFile, ')');
                    result *= Transformation.RotationX(angleX);
                    break;
                
                case Keyword.RotationY:
                    ExpectSymbol(inputFile, '(');
                    var angleY = ExpectNumber(inputFile);
                    ExpectSymbol(inputFile, ')');
                    result *= Transformation.RotationY(angleY);
                    break;
                
                case Keyword.RotationZ:
                    ExpectSymbol(inputFile, '(');
                    var angleZ = ExpectNumber(inputFile);
                    ExpectSymbol(inputFile, ')');
                    result *= Transformation.RotationZ(angleZ);
                    break;
                
                case Keyword.Scaling:
                    ExpectSymbol(inputFile, '(');
                    var vecScale = ParseVector(inputFile);
                    ExpectSymbol(inputFile, ')');
                    result *= Transformation.Scaling(vecScale);
                    break;
                
                default:
                    throw new InvalidOperationException("This line should not be reachable.");
            }
            
            // Look-ahead to see if there is another transformation chained
            var newToken = inputFile.ReadToken();
            if (newToken is not SymbolToken symbol || symbol.Symbol != '*')
            {
                inputFile.UnreadToken(newToken);
                break;
            }
            
        }
        
        return result;
        
    }

    /// <summary>
    /// Parse a <c>Plane</c>: Plane(transformation, material).
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>Plane</c> to be stored.</returns>
    public Plane ParsePlane(InputStream inputFile)
    {
        ExpectSymbol(inputFile, '(');
        var transformation = ParseTransformation(inputFile);
        ExpectSymbol(inputFile, ',');
        var (materialName, material) = ParseMaterial(inputFile);
        if (!Materials.ContainsKey(materialName))
        {
            throw new GrammarException($"Unknown material '{materialName}'.", inputFile.Location);
        }
        ExpectSymbol(inputFile, ',');
        var plane = new Plane(transformation, material);
        ExpectSymbol(inputFile, ')');
        
        return plane;
    }
    
    /// <summary>
    /// Parse a <c>Sphere</c>: Sphere(transformation, material).
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>Sphere</c> to be stored.</returns>
    public Sphere ParseSphere(InputStream inputFile)
    {
        ExpectSymbol(inputFile, '(');
        var transformation = ParseTransformation(inputFile);
        ExpectSymbol(inputFile, ',');
        var (materialName, material) = ParseMaterial(inputFile);
        if (!Materials.ContainsKey(materialName))
        {
            throw new GrammarException($"Unknown material '{materialName}'.", inputFile.Location);
        }
        ExpectSymbol(inputFile, ',');
        var sphere = new Sphere(transformation, material);
        ExpectSymbol(inputFile, ')');
        
        return sphere;
    }

    /// <summary>
    /// Parse a <c>ICamera</c>: Perspective(transformation, distance, aspectRatio) or Orthogonal(transformation, distance, aspectRatio).
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <returns><c>ICamera</c> to be stored.</returns>
    public ICamera ParseCamera(InputStream inputFile)
    {
        var cameraType = ExpectKeywords(inputFile, new List<Keyword>() { Keyword.Orthogonal, Keyword.Perspective });
        ExpectSymbol(inputFile, '(');
        var transformation = ParseTransformation(inputFile);
        ExpectSymbol(inputFile, ',');
        var distance = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ',');
        var aspectRatio = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ')');

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
                throw new InvalidOperationException("This line should not be reachable.");
        }
        
        return camera;
    }

    /// <summary>
    /// Parse the whole <c>Scene</c> until reaching the end of the file.
    /// </summary>
    /// <param name="inputFile"><c>InputStream</c> that contains the scene description of the input file.</param>
    /// <param name="externalVariables"><c>Dictionary</c> of external variables that can be passed to override those inside the scene file.</param>
    public void ParseScene(InputStream inputFile, Dictionary<string, float>? externalVariables)
    {
        // Initialize with external variables if provided
        if (externalVariables != null)
        {
            FloatVariables = new Dictionary<string, float>(externalVariables);
            OverriddenVariables = new HashSet<string>(externalVariables.Keys);
        }
        
        while (true)    // Until EOF is reached
        {
            var whichToken = inputFile.ReadToken();

            if (whichToken is StopToken stopToken)  // Stop if EOF is reached
            {
                break;
            }

            if (whichToken is not KeywordToken keywordToken)    // First expect a keyword
            {
                throw new GrammarException($"Expected keyword instead of '{whichToken}'.", inputFile.Location);
            }

            switch (keywordToken.Keyword)   // See which keyword it is and register the corresponding variable
            {
                case Keyword.Float:
                    var floatName = ExpectIdentifier(inputFile);
                    ExpectSymbol(inputFile, '(');
                    var floatValue = ExpectNumber(inputFile);
                    ExpectSymbol(inputFile, ')');
                    if (!OverriddenVariables.Contains(floatName))   // If the identifier is not among the variables to override
                    {
                        if (FloatVariables.ContainsKey(floatName))  // If it is inside the dictionary of variables in the input file
                        {
                            throw new GrammarException($"Variable '{floatName}' cannot be redefined.",
                                inputFile.Location);
                        }
                        else
                        {
                            FloatVariables.Add(floatName, floatValue);
                        }
                    }

                    break;
                
                case Keyword.Plane:
                    SceneWorld.AddShape(ParsePlane(inputFile));
                    break;
                
                case Keyword.Sphere:
                    SceneWorld.AddShape(ParseSphere(inputFile));
                    break;
                
                case Keyword.Material:
                    var (materialName, material) = ParseMaterial(inputFile);
                    if (Materials.ContainsKey(materialName))
                    {
                        throw new GrammarException($"Material '{materialName}' cannot be redefined.", inputFile.Location);
                    }
                    else
                    {
                        Materials.Add(materialName, material);
                    }
                    break;
                
                case Keyword.Camera:
                    if (SceneCamera != null)
                    {
                        throw new GrammarException("Scene camera already exists, cannot define two cameras.", inputFile.Location);
                    }
                    else
                    {
                        SceneCamera = ParseCamera(inputFile);
                    }
                    break;
                
                default:
                    throw new InvalidOperationException("This line should not be reachable.");
            }
        }
    }
    
}
