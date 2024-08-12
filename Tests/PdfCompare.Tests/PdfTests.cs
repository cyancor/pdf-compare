namespace PdfCompare.Tests;

public class PdfTests
{
    [Test]
    public void ValidComparison()
    {
        var pdfCompare = new PdfCompare
        {
            Reference = "./Assets/Reference.pdf",
            Comparison = "./Assets/ValidComparison.pdf"
        };

        pdfCompare.Compare();
    }

    [Test]
    public void InvalidComparison()
    {
        var pdfCompare = new PdfCompare
        {
            Reference = "./Assets/Reference.pdf",
            Comparison = "./Assets/InvalidComparison.pdf"
        };

        try
        {
            pdfCompare.Compare();
        }
        catch (PdfContentMismatchException exception)
        {
            Assert.That(
                exception.DifferenceImages[0] == "ComparisonOutput/Reference.page-1.difference.png"
                && exception.DifferenceImages[1] == "ComparisonOutput/Reference.page-2.difference.png",
                Is.True
            );

            return;
        }

        Assert.Fail();
    }

    [Test]
    public void InvalidComparisonWithMask()
    {
        var pdfCompare = new PdfCompare
        {
            Reference = "./Assets/Reference.pdf",
            Comparison = "./Assets/InvalidComparison.pdf",
            Masks = [ new MaskImage(Path: "./Assets/Reference.page-2.mask.png", PageNumber: 1) ]
        };

        try
        {
            pdfCompare.Compare();
        }
        catch (PdfContentMismatchException exception)
        {
            Assert.That(
                exception.DifferenceImages[0] == "ComparisonOutput/Reference.page-1.difference.png",
                Is.True
            );

            return;
        }

        Assert.Fail();
    }
}
