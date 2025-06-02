# RayTracer

RayTracer is a C# command-line tool that has two main functionalities:
- converting a PFM (Portable Float Map) file to an LDR (Low Dynamic Range) image file in various formats, with the possibility to specify the normalization factor and the gamma correction used for the conversion 
- rendering a demo scene with various options such as the rendering algorithm (on-off, flat, path-tracing) type of projection (perspective or orthogonal), the distance of the perspective observer from the screen, the angle of view, the resolution of the resulting image; the output is a PFM file that is automatically converted to a LDR image with the options listed before.

The project is developed using .NET 9.

## Supported Output Formats
RayTracer supports the following LDR image formats:
- PNG (`.png`)
- BMP (`.bmp`)
- JPEG (`.jpeg`)
- TGA (`.tga`)
- WebP (`.webp`)

## Prerequisites
- Linux or Windows

## Usage
Download the executable corresponding to your OS.
RayTracer is executed from the command line.

To run the tool on Windows:
```sh
.\RayTracer
```
To run the tool on Linux:
```sh
./RayTracer
```
This will show all arguments that have to be specified from command line.

### Parameters:
Use the option -h to show all available commands and options.

### Example:
Pfm to Ldr converter:
```sh
./RayTracer pfm2ldr input.pfm output.png -f 1.2 -g 2.2
```
This command converts `input.pfm` to `output.png` using an exposure scaling factor of `1.2` and gamma correction of `2.2`.

Demo renderer:
```sh
./RayTracer demo -W 640 -H 480 -p output.pfm -l output.png -a 20 -A path-tracer
```
This command renders a demo scene creating a pfm file `output.pfm` and automatically converting it into a ldr file `output.png`. The resulting image has resolution 640x480 pixels; the scene is rendered using a perspective projection with a 20 degrees angle of view, and using a path tracing algorithm.

## License
This project is licensed under the EUPL-1.2 License.

## Authors
Developed by [giorgiaiori](https://github.com/giorgiaiori) and [Cr3sp1](giorgiaiori).

