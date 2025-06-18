namespace Trace.Tests;

using System.IO;
using System.Text;

public class SceneTests
{
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
        var input = "camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 1.0)" +
                    "camera(orthogonal, identity, 1.0, 1.0)";
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