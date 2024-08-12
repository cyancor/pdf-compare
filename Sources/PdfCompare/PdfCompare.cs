using ImageMagick;

namespace PdfCompare;

/// <summary>
/// Image to mask the reference and comparison images.
/// </summary>
/// <param name="Path">The file path to the mask.png</param>
/// <param name="PageNumber">The page number of the PDF (starts at 0)</param>
public record MaskImage(string Path, int PageNumber);

public class PdfCompare
{
    public string OutputDirectory { get; init; } = "ComparisonOutput";

    public required string Reference { get; init; }
    public required string Comparison { get; init; }
    public MaskImage[] Masks { get; init; } = [];

    private List<string> ConvertPdfToImages(string path)
    {
        var settings = new MagickReadSettings
        {
            Density = new Density(300, 300)
        };

        using var images = new MagickImageCollection();
        images.Read(path, settings);

        var page = 1;
        var fileName = Path.GetFileNameWithoutExtension(path);
        List<string> files = [];

        foreach (var image in images)
        {
            var fileNameWithPaging = Path.Combine(OutputDirectory, "tmp", $"{fileName}.page-{page}.png");
            image.Write(fileNameWithPaging);
            files.Add(fileNameWithPaging);
            page++;
        }

        return files;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PdfPageCountMismatchException"></exception>
    /// <exception cref="PdfContentMismatchException"></exception>
    public bool Compare()
    {
        var tmpDirectoryPath = Path.Combine(OutputDirectory, "tmp");

        try
        {
            if (Directory.Exists(tmpDirectoryPath))
            {
                Directory.Delete(tmpDirectoryPath, true);
            }

            Directory.CreateDirectory(tmpDirectoryPath);

            var referencePdfImages = ConvertPdfToImages(Reference);
            var comparisonPdfImages = ConvertPdfToImages(Comparison);

            if (referencePdfImages.Count != comparisonPdfImages.Count)
            {
                throw new PdfPageCountMismatchException(Reference, Comparison);
            }

            var failedDifferentImagePaths = new List<string>();

            for (var index = 0; index < referencePdfImages.Count; index++)
            {
                var imageCompare = new ImageCompare
                {
                    Reference = referencePdfImages[index],
                    Comparison = comparisonPdfImages[index]
                };

                var mask = Masks.FirstOrDefault(mask => mask.PageNumber == index);

                if (mask != null)
                {
                    imageCompare.Mask = mask.Path;
                }

                var (distortion, differenceImagePath) = imageCompare.Compare();

                if (distortion != 0)
                {
                    failedDifferentImagePaths.Add(differenceImagePath);
                }
            }

            if (failedDifferentImagePaths.Count > 0)
            {
                throw new PdfContentMismatchException(Reference, Comparison, failedDifferentImagePaths);
            }

            return true;
        }
        finally
        {
            Directory.Delete(tmpDirectoryPath, true);
        }
    }
}
