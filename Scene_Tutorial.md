# Scene Generation Languange Tutorial

This is a brief guide on how to write a suitable input file for a scene generation. The tutorial will include the specification of all objects available and needed for the composition of a scene and the enumeration of the language syntax rules. 

The **description of the scene** must include:
- a **camera** representing the observer
- the declaration of **materials** that will be found in the scene
- the **shapes** that make up the scene

Both camera and shapes can be moved in space by applying transformations. 

Whitespaces and comments (starting by `#`) are skipped while parsing. See the [README](README.md) for details on how to overwrite scene variables from command line.

Let's walk through the grammar of the Scene Generation Language used in this RayTracer.

## Transformations
Available transformations:
- `identity`: **identity** 
- `translation([x,y,z])`: **translation** of vector with `float` components `x, y, z`
- `rotation_x(angle)`: **rotation** of `float` angle `angle` (expressed in degrees) around the x-axis
- `rotation_y(angle)`: **rotation** of `float` angle `angle` (expressed in degrees) around the y-axis
- `rotation_z(angle)`: **rotation** of `float` angle `angle` (expressed in degrees) around the z-axis
- `scaling([x,y,z])`: **scaling** by factor `x`, `y` or `z` respectively along the x-direction, y-direction or z-direction

A transformation can also be identified with a **composition of multiple transformations** by the symbol `*`, for example: `rotation_x(60)*translation([0,0,3])`.

*Note:* `Vector` types are enclosed in brackets `[...]` and defined by their x, y, z `float` components.

## Camera
There are two possible **types of camera**, depending on the desired projection of the scene on the screen:
- **perspective camera**: rays that hit the pixels of the screen depart from the position of the observer, therefore their direction depends on the camera-screen distance
- **orthogonal camera**: the direction of rays is orthogonal to the screen, independent on the camera-screen distance

Parameters of a `Camera` object:
- **transformation**: a `Transformation` used to move the observer in space
- **aspect ratio**: a `float` representing $\frac{\text{width}}{\text{height}}$ of the screen (it should be coherent with the resolution passed by command line)
- **distance**: a `float` for the distance of the observer from the screen, only relevant for the perspective type

Example of camera definition:
```
camera (perspective, rotation_z(angle), aspect_ratio, distance)
```
where `angle`, `aspect_ratio` and `distance` must be previously defined `float` variables.

## Material
A material has **two fields**:
- a **`BRDF`**: the **Bidirectional Reflectance Distribution Function**, representing how the light is scattered when rays hit the material
- a **`Pigment`**: the **emitted radiance**, representing the light emitted by the material

A **pigment** can be of the following types:
- `uniform(color)`: uniformly colored in `color`
- `checkered(color1, color2)`: checkered pattern of colors `color1` and `color2`
- `image("image.pfm")`: texture of a Pfm image with path `"image.pfm"`

Two types of **BRDF** are available:
- `diffuse(pigment)`: rays are scattered uniformly around the $2\pi$ hemisphere and reflected radiance is weighed by the pigment
- `specular(pigment)`: all rays are scattered along the reflection direction and reflected radiance is weighed by the pigment

*Note:* Colors are enclosed in brackets `<...>` and defined by their R, G, B `float` components $\in [0,1]$, for example: `<0.1, 0.3, 1.0>`.

Example of material definition:
```
material my_material ( diffuse( uniform(<0,0,0>) ), uniform(<1,1,1>) )
```

## Shapes
Available shapes:
- **`sphere`**, that by default is centered in the origin and has unitary radius
- **`plane`**, that by default is $z=0$

All shapes must have the following **parameters**:
- a **`Material`**: the material of the object, that must have been previously defined
- a **`Transformation`**: the transformation to shape and move the object as needed

Example of adding a shape to the scene:
```
material sphere_material ( diffuse(uniform(<0,0,0>)), uniform(<1,0,0>))

sphere (sphere_material, translation([1,0,0])*scaling([3,3,3]))
```

## Floats
The Scene Generation Language provides the user with the possibility to declare **`float`** variables. The correct syntax is the following:
```
float angle(30)
```
where the `float` number inside brackets is the value of the variable.