namespace PdfCompare;

public class PdfPageCountMismatchException(string referencePdfName, string comparisonPdfName)
    : Exception($"The number of pages in the reference PDF ({referencePdfName}) does not match the number of pages in the comparison PDF ({comparisonPdfName}).");

public class PdfContentMismatchException(
    string referencePdfName,
    string comparisonPdfName,
    List<string> differenceImagePaths)
    : Exception(
        $"The content of the reference PDF ({referencePdfName}) does not match the content of the comparison PDF ({comparisonPdfName}).\n\n" +
        string.Join("\n", differenceImagePaths.Select(path => $"Difference: {path}")) + "\n")
{
    public string[] DifferenceImages = differenceImagePaths.ToArray();
};
