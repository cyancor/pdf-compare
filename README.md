# PDFCompare for .NET

Compares two PDF files with ImageMagick.

```csharp

var pdfCompare = new PdfCompare
{
    Reference = "./Assets/Reference.pdf",
    Comparison = "./Assets/Comparison.pdf",
    Masks = [ new MaskImage(Path: "./Assets/Reference.page-3.mask.png", PageNumber: 2) ]
};

pdfCompare.Compare();
```

## Requirements
On Linux and macOS the `fontconfig` library is used to read fonts. This needs to be installed on the system or container that is running Magick.NET. It might also be required to run `fc-cache` to update the font cache.

`Ghostscript` must be installed on the system. This is needed by ImageMagick to convert the PDF files into images.
