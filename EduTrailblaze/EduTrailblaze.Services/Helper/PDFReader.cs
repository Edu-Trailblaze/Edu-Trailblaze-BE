using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Markdig;
using Microsoft.AspNetCore.Http;

namespace EduTrailblaze.Services.Helper
{
    public static class PDFReader
    {
        public static string ExtractText(IFormFile file, string format = "markdown")
        {
            if (file == null || file.Length == 0)
                return "";

            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            string extractedText = "";

            try
            {
                using var stream = file.OpenReadStream();

                if (fileExtension == ".pdf")
                {
                    extractedText = ExtractTextFromPdf(stream);
                }
                else
                {
                    throw new Exception("Unsupported file format");
                }

                // Convert to Markdown or HTML
                string formattedContent = format.ToLower() == "html"
                    ? Markdown.ToHtml(extractedText)   // Convert Markdown to HTML
                    : extractedText;                   // Keep as Markdown

                return formattedContent;
            }
            catch (Exception ex)
            {
                throw new Exception("Error extracting text from file", ex);
            }
        }

        public static string ExtractTextFromPdf(Stream pdfStream)
        {
            using var reader = new PdfReader(pdfStream);
            using var pdfDoc = new PdfDocument(reader);

            string text = "";
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                text += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i)) + "\n";
            }

            return text;
        }
    }
}
