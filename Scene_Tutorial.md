# Scene Generation Language Tutorial

This is a brief guide on how to write a suitable input file defining a scene. The tutorial will include the 
specification of all objects available and needed for the composition of a scene and all the language syntax rules. 

The **description of the scene** must include:
- a **camera** representing the observer.
- the declaration of **materials** that will be found in the scene.
- the **shapes** that make up the scene.

Both camera and shapes can be moved in space by applying transformations. 

Whitespaces and comments (starting by `#`) are skipped by the parser. See the [README](README.md) for details on how to 
overwrite scene variables from command line.

Let's walk through the grammar of the Scene Generation Language used in this RayTracer.

## Floats
The Scene Generation Language provides the user with the possibility to declare **`float`** variables. The correct 
syntax is the following:
```
float angle(30)
```
where the `float` number inside brackets is the value of the variable.

## Transformations
Available transformations:
- `identity`: **identity**.
- `translation([x,y,z])`: **translation** of vector with `float` components `x, y, z`.
- `rotation_x(angle)`: **rotation** of `float` angle `angle` (expressed in degrees) around the x-axis.
- `rotation_y(angle)`: **rotation** of `float` angle `angle` (expressed in degrees) around the y-axis.
- `rotation_z(angle)`: **rotation** of `float` angle `angle` (expressed in degrees) around the z-axis.
- `scaling([x,y,z])`: **scaling** by factor `x`, `y` or `z` respectively along the x-direction, y-direction or z-direction.

A transformation can also be identified with a **composition of multiple transformations** by the symbol `*`, for 
example: `rotation_x(60)*translation([0,0,3])`.

*Note:* `Vector` types are enclosed in brackets `[...]` and defined by their x, y, z `float` components.

## Camera
There are two possible **types of camera**, depending on the desired projection of the scene on the screen:
- **perspective camera**: rays that hit the pixels of the screen depart from the position of the observer, so their direction depends on the camera-screen distance.
- **orthogonal camera**: the direction of rays is orthogonal to the screen, independent of the camera-screen distance.

Parameters of a `Camera` object:
- **transformation**: a `Transformation` used to move the observer in space.
- **aspect ratio**: a `float` representing $\frac{\text{width}}{\text{height}}$ of the screen (it should be coherent with the resolution passed by command line).
- **distance**: a `float` for the distance of the observer from the screen, only relevant for the perspective type.

Example of camera definition:
```
camera (perspective, rotation_z(angle), aspect_ratio, distance)
```
where `angle`, `aspect_ratio` and `distance` must be previously defined `float` variables.

## Material
A material has **two fields**:
- a **`BRDF`**: the **Bidirectional Reflectance Distribution Function**, representing how the light is scattered when rays hit the material.
- a **`Pigment`**: the **emitted radiance**, representing the light emitted by the material.

A **pigment** can be of the following types:
- `uniform(color)`: uniformly colored in `color`.
- `checkered(color1, color2)`: checkered pattern of colors `color1` and `color2`.
- `image("image.pfm")`: texture of a Pfm image with path `"image.pfm"`.

Two types of **BRDF** are available:
- `diffuse(pigment)`: rays are scattered uniformly around the $2\pi$ hemisphere, and reflected radiance is weighed by the pigment.
- `specular(pigment)`: all rays are scattered along the reflection direction, and reflected radiance is weighed by the pigment.

*Note:* Colors are enclosed in brackets `<...>` and defined by their R, G, B `float` components $\in [0,1]$, for example: `<0.1, 0.3, 1.0>`.

Example of material definition:
```
material my_material (diffuse(uniform(<0,0,0>)), uniform(<1,1,1>))
```

## Basic Shapes
Available basic shapes:
- **`sphere`**, that by default is centered in the origin and has unitary radius.
- **`plane`**, that by default is $z=0$.
- **`cube`**, that by default is centered on the origin and has side of lenght 2.
- **`cylinder`**, that by default is centered on the origin, has unitary radius and has height of lenght 2.

All basic shapes must have the following **parameters**:
- a **`Material`**: name of the material of the shape, which must have been previously defined.
- a **`Transformation`**: the transformation the shape is subject to, used to modify and move around the shape.

Furthermore, since the **`cube`** and the **`cylinder`** have respectively six and three faces each, we offer the 
possibility to declare a different material for each face by substituting the single **`Material`** parameter with a 
list of **`Material`**. If the number of materials is inferior to the number of faces, the material of the 
first face will be assigned to the undefined face material. If the number is instead superior to the number of faces,
the extra materials will be ignored. <br>
The order of faces in the **`cube`** follows the order of the number of dots in a
right-handed dice, so the faces in order point to the following directions: $\hat{e}_x$, $\hat{e}_y$, $\hat{e}_z$,
$-\hat{e}_z$, $-\hat{e}_y$, $-\hat{e}_x$.<br>
the order of faces in the **`cylinder`** is the following: lateral face, top face, bottom face.

Examples of adding a shape to the scene, assuming materials have already been previously defined:
```
sphere (first_material, translation([1,0,0])*scaling([3,3,3]))
cube (second_material, identity)
cube ([first_material, second_material], translation([3,4.5,0]))
cylinder ([first_material, second_material, third_material], rotation_y(20))
```

## Constructive Solid Geometry
The **`csg`** shape implements the Constructive Solid Geometry technique to build complex shapes from simpler ones by 
combining them through the following operations. The parameters of  **`csg`** are the two shapes it is composed by,
which we call shape A and shape B, the type of operation they are subject to, and an ulterior **`Transformation`**.
The implemented types of operations are the following: 
- **`union`**: the union of the two shapes, comprised by all the points of shape A and all the points of shape B.
- **`difference`**: the difference between shape A and shape B, comprised by all the points of shape A that do not fall 
within shape B.
- **`intersection`**: the intersection between the two shapes, comprised by the points of shape A that fall within shape 
B and vice versa.

The shapes used in a **`csg`** must be previously defined as variables similarly to materials, and can both be a basic 
shape or a **`csg`** themselves. Note: **`plane`** shapes are not finite, and so they are not supported as elements of a
**`csg`**, and for this reason can not be defined as variables but only added to the scene.

Examples of adding a **`csg`** to the scene, assuming materials have already been previously defined:
```
shape shapeA( sphere (first_material, translation([1,0,0])*scaling([3,3,3])) )
shape shapeB( cube (second_material, identity) )
shape shapeC( cylinder([first_material, second_material, third_material], rotation_y(20)) )
shape firstCsg( csg(shapeA, shapeB, union, identity) )

csg(firstCsg, shapeC, difference, identity)
```
