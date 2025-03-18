using EduTrailblaze.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EduTrailblaze.Services
{
    public class PdfService : IPdfService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public PdfService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["pdflayer:ApiKey"];
        }

        public async Task<byte[]> ConvertHtmlToPdfAsync(string htmlContent)
        {
            try
            {
                var apiUrl = $"https://api.pdflayer.com/api/convert?access_key={_apiKey}&page_size=A4&margin_top=0&margin_bottom=0&margin_left=0&margin_right=0";

                using var content = new MultipartFormDataContent
                {
                    { new StringContent(htmlContent), "document_html" }
                };

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    throw new Exception($"Failed to convert HTML to PDF. Status: {response.StatusCode}, Message: {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while converting HTML to PDF: " + ex.Message, ex);
            }
        }
    }
}
