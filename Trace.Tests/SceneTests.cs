namespace Trace.Tests;

using System.IO;
using System.Text;

public class SceneTests
{
    [Fact]
    public void TestParser()
    {
        var input = """
                    float clock(150)

                    material sky_material(
                        diffuse(uniform(<0, 0, 0>)),
                        uniform(<0.7, 0.5, 1>)
                    )

                    # Here is a comment

                    material ground_material(
                        diffuse(checkered(<0.3, 0.5, 0.1>,
                                          <0.1, 0.2, 0.5>, 4)),
                        uniform(<0, 0, 0>)
                    )

                    material sphere_material(
                        specular(uniform(<0.5, 0.5, 0.5>)),
                        uniform(<0, 0, 0>)
                    )

                    plane (sky_material, translation([0, 0, 100]) * rotation_y(clock))
                    plane (ground_material, identity)

                    sphere(sphere_material, translation([0, 0, 1]))

                    camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 2.0)
                    """;
        var stream = new InputStream(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        var scene = new Scene();
        scene.ParseScene(stream);
        
        // Check materials
        Assert.Equal(3, scene.Materials.Count);
        
    }

    [Fact]
    public void TestParserUndefinedMaterial()
    {
        var input = "plane(this_material_does_not_exist, identity)";
        var stream = new InputStream(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        var scene = new Scene();
        try
        {
            scene.ParseScene(stream);
            Assert.Fail("Code did not throw an exception!");
        }
        catch (GrammarException e)
        {
            // Test passed
        }
    }

    [Fact]
    public void TestParserDoubleCamera()
    {
        var input = """
                    camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 1.0) 
                    camera(orthogonal, identity, 1.0, 1.0)
                    """;
        var stream = new InputStream(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        var scene = new Scene();
        try
        {
            scene.ParseScene(stream);
            Assert.Fail("Code did not throw an exception!");
        }
        catch (GrammarException e)
        {
            // Test passed
        }
    }
}