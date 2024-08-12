using ImageMagick;

namespace PdfCompare;

public class ImageCompare
{
    /// <summary>
    /// Specifies the output directory of the difference.png images, resulting from the comparison.
    /// Default: "./ComparisonOutput"
    /// </summary>
    public string OutputDirectory { get; init; } = "ComparisonOutput";

    /// <summary>
    /// The reference image, which the comparison is based on.
    /// </summary>
    public required string Reference { get; init; }
    /// <summary>
    /// The image to compare to.
    /// </summary>
    public required string Comparison { get; init; }
    /// <summary>
    /// Optionally a mask image to ignore certain parts of the image in the comparison.
    /// </summary>
    public string? Mask { get; set; }

    /// <summary>
    /// Compares the Reference image to the Comparison image.
    /// </summary>
    /// <returns>The distortion and the path to the difference.png image.</returns>
    public (double Distortion, string DifferenceImagePath) Compare()
    {
        using var referenceImage = new MagickImage(Reference);
        using var comparisonImage = new MagickImage(Comparison);

        if (!string.IsNullOrEmpty(Mask))
        {
            using var maskImage = new MagickImage(Mask);
            referenceImage.Composite(maskImage, CompositeOperator.CopyAlpha);
            comparisonImage.Composite(maskImage, CompositeOperator.CopyAlpha);
        }

        using var diffImage = new MagickImage();
        var distortion = referenceImage.Compare(comparisonImage, ErrorMetric.MeanErrorPerPixel, diffImage);

        var referenceImageName = Path.GetFileNameWithoutExtension(Reference);
        var differenceImagePath = Path.Combine(OutputDirectory, $"{referenceImageName}.difference.png");
        diffImage.Write(differenceImagePath);

        return (distortion, differenceImagePath);
    }
}
