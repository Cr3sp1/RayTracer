# RayTracer

RayTracer is a C# command-line tool that converts a PFM (Portable Float Map) file to an LDR (Low Dynamic Range) image file in various formats. The project is developed using .NET 9.

## Supported Output Formats
RayTracer supports the following LDR image formats:
- PNG (`.png`)
- BMP (`.bmp`)
- JPEG (`.jpeg`)
- TGA (`.tga`)
- WebP (`.webp`)

## Prerequisites
- .NET 9 SDK installed on your system.

## Usage
RayTracer is executed from the command line in the `RayTracer/RayTracer` directory.

```sh
cd RayTracer/RayTracer
```

To run the tool:
```sh
dotnet run <INPUT_PFM_FILE> <FACTOR> <GAMMA> <OUTPUT_LDR_FILE>
```

### Parameters:
- `<INPUT_PFM_FILE>`: Path to the input PFM file.
- `<FACTOR>`: Scaling factor for adjusting the exposure of the image.
- `<GAMMA>`: Gamma correction value.
- `<OUTPUT_LDR_FILE>`: Path to the output LDR file with the desired extension (e.g., `.png`, `.bmp`, `.jpeg`, `.tga`, `.webp`).

### Example:
```sh
dotnet run input.pfm 1.2 2.2 output.png
```
This command converts `input.pfm` to `output.png` using an exposure scaling factor of `1.2` and gamma correction of `2.2`.

## Building the Project
To build the project manually:
```sh
dotnet build
```

## License
This project is licensed under the EUPL-1.2 License.


## Authors
Developed by giorgiaiori and Cr3sp1.

