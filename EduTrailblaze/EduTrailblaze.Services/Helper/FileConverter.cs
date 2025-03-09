using PuppeteerSharp;

namespace EduTrailblaze.Services.Helper
{
    public class FileConverter
    {
        public static async Task<byte[]> ConvertHtmlToImage(string htmlContent)
        {
            // Ensure PuppeteerSharp is downloaded
            await new BrowserFetcher().DownloadAsync();

            // Launch a headless browser
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();

            // Set content
            await page.SetContentAsync(htmlContent);

            // Capture screenshot as a PNG image
            var imageBytes = await page.ScreenshotDataAsync(new ScreenshotOptions
            {
                Type = ScreenshotType.Png,
                FullPage = true
            });

            return imageBytes;
        }
    }
}
