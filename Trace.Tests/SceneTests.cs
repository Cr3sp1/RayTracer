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
                        specular(striped(<0.3, 0.5, 0.1>,
                                         <0.2, 0.1, 0.7>, 4, horizontal)),
                        uniform(<0, 0, 0>)
                    )

                    plane (sky_material, translation([0, 0, 100]) * rotation_y(clock))
                    plane (ground_material, identity)

                    sphere(sphere_material, translation([0, 0, 1]))

                    shape shapeA(sphere(sphere_material, translation([0, 0, 1])))
                    shape shapeB(cylinder (ground_material, identity))
                    shape shapeC(csg(shapeA, shapeB, difference, rotation_y(clock)))

                    csg( shapeB, shapeC, fusion, identity)

                    cube(sphere_material, translation([0, 0, 1]))

                    cube([sphere_material, ground_material] , translation([0, 0, 2]))

                    cylinder(sphere_material, translation([0, 0, 3]))

                    camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 2.0)
                    """;
        var stream = new InputStream(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        var scene = new Scene(stream);
        scene.ParseScene();

        // Check materials
        Assert.Equal(3, scene.MaterialVariables.Count);
        Assert.Contains("sky_material", scene.MaterialVariables);
        Assert.Contains("ground_material", scene.MaterialVariables);
        Assert.Contains("sphere_material", scene.MaterialVariables);

        var skyMaterial = scene.MaterialVariables["sky_material"];
        Assert.IsType<DiffuseBrdf>(skyMaterial.Brdf);
        var skyPigment = skyMaterial.Brdf.Pigment as UniformPigment;
        Assert.NotNull(skyPigment);
        Assert.True(Color.CloseEnough(Color.Black, skyPigment.Col));
        var skyRadiance = skyMaterial.EmittedRadiance as UniformPigment;
        Assert.NotNull(skyRadiance);
        Assert.True(Color.CloseEnough(new Color(0.7f, 0.5f, 1f), skyRadiance.Col));

        var groundMaterial = scene.MaterialVariables["ground_material"];
        Assert.IsType<DiffuseBrdf>(groundMaterial.Brdf);
        var groundPigment = groundMaterial.Brdf.Pigment as CheckeredPigment;
        Assert.NotNull(groundPigment);
        Assert.True(Color.CloseEnough(new Color(0.3f, 0.5f, 0.1f), groundPigment.Col1));
        Assert.True(Color.CloseEnough(new Color(0.1f, 0.2f, 0.5f), groundPigment.Col2));
        var groundRadiance = groundMaterial.EmittedRadiance as UniformPigment;
        Assert.NotNull(groundRadiance);
        Assert.True(Color.CloseEnough(Color.Black, groundRadiance.Col));

        var sphereMaterial = scene.MaterialVariables["sphere_material"];
        Assert.IsType<SpecularBrdf>(sphereMaterial.Brdf);
        var spherePigment = sphereMaterial.Brdf.Pigment as StripedPigment;
        Assert.NotNull(spherePigment);
        Assert.True(Color.CloseEnough(new Color(0.3f, 0.5f, 0.1f), spherePigment.Col1));
        Assert.True(Color.CloseEnough(new Color(0.2f, 0.1f, 0.7f), spherePigment.Col2));
        Assert.False(spherePigment.IsVertical);
        var sphereRadiance = sphereMaterial.EmittedRadiance as UniformPigment;
        Assert.NotNull(sphereRadiance);
        Assert.True(Color.CloseEnough(Color.Black, sphereRadiance.Col));

        // Check saved shapes
        Assert.Equal(3, scene.ShapeVariables.Count);
        Assert.Contains("shapeA", scene.ShapeVariables);
        Assert.Contains("shapeB", scene.ShapeVariables);
        Assert.Contains("shapeC", scene.ShapeVariables);
        Assert.True(scene.ShapeVariables["shapeA"] is Sphere);
        Assert.True(scene.ShapeVariables["shapeB"] is Cylinder);
        Assert.True(scene.ShapeVariables["shapeC"] is Csg);

        // Check scene shapes
        Assert.Equal(7, scene.SceneWorld.Shapes.Count);

        Assert.True(scene.SceneWorld.Shapes[0] is Plane);
        Assert.True(Transformation.CloseEnough(scene.SceneWorld.Shapes[0].Transform,
            Transformation.Translation(new Vec(0.0f, 0.0f, 100.0f)) * Transformation.RotationY(150.0f)));

        Assert.True(scene.SceneWorld.Shapes[1] is Plane);
        Assert.True(Transformation.CloseEnough(scene.SceneWorld.Shapes[1].Transform, new Transformation()));

        Assert.True(scene.SceneWorld.Shapes[2] is Sphere);
        Assert.True(Transformation.CloseEnough(scene.SceneWorld.Shapes[2].Transform,
            Transformation.Translation(new Vec(0.0f, 0.0f, 1.0f))));

        Assert.True(scene.SceneWorld.Shapes[3] is Csg);
        var csg = scene.SceneWorld.Shapes[3] as Csg;
        Assert.NotNull(csg);
        Assert.Equal(CsgType.Fusion, csg.Type);
        Assert.Equal(csg.ShapeA, scene.ShapeVariables["shapeB"]);
        Assert.Equal(csg.ShapeB, scene.ShapeVariables["shapeC"]);

        Assert.True(scene.SceneWorld.Shapes[4] is Cube);
        Assert.True(Transformation.CloseEnough(scene.SceneWorld.Shapes[4].Transform,
            Transformation.Translation(new Vec(0.0f, 0.0f, 1.0f))));
        Assert.Equal(6, scene.SceneWorld.Shapes[4].Materials.Count);
        Assert.True(scene.SceneWorld.Shapes[4].Materials[0] == scene.MaterialVariables["sphere_material"]);
        Assert.True(scene.SceneWorld.Shapes[4].Materials[1] == scene.MaterialVariables["sphere_material"]);

        Assert.True(scene.SceneWorld.Shapes[5] is Cube);
        Assert.True(Transformation.CloseEnough(scene.SceneWorld.Shapes[5].Transform,
            Transformation.Translation(new Vec(0.0f, 0.0f, 2.0f))));
        Assert.Equal(6, scene.SceneWorld.Shapes[5].Materials.Count);
        Assert.True(scene.SceneWorld.Shapes[5].Materials[0] == scene.MaterialVariables["sphere_material"]);
        Assert.True(scene.SceneWorld.Shapes[5].Materials[1] == scene.MaterialVariables["ground_material"]);

        Assert.True(scene.SceneWorld.Shapes[6] is Cylinder);
        Assert.True(Transformation.CloseEnough(scene.SceneWorld.Shapes[6].Transform,
            Transformation.Translation(new Vec(0.0f, 0.0f, 3.0f))));

        // Check camera
        Assert.True(scene.SceneCamera is PerspectiveCamera);
        PerspectiveCamera? camera = scene.SceneCamera as PerspectiveCamera;
        Assert.True(camera != null && Transformation.CloseEnough(camera.Transform,
            Transformation.RotationZ(30.0f) * Transformation.Translation(new Vec(-4.0f, 0.0f, 1.0f))));
        Assert.True(Utils.CloseEnough(camera.AspectRatio, 1.0f));
        Assert.True(Utils.CloseEnough(camera.Distance, 2.0f));
    }

    [Fact]
    public void TestParserUndefinedMaterial()
    {
        var input = "plane(this_material_does_not_exist, identity)";
        var stream = new InputStream(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        var scene = new Scene(stream);
        try
        {
            scene.ParseScene();
            Assert.Fail("Code did not throw an exception!");
        }
        catch (GrammarException)
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
        var scene = new Scene(stream);
        try
        {
            scene.ParseScene();
            Assert.Fail("Code did not throw an exception!");
        }
        catch (GrammarException)
        {
            // Test passed
        }
    }
}