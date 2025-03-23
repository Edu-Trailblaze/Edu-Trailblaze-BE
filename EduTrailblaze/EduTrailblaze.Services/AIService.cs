using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace EduTrailblaze.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _localAiTextUrl;
        private readonly string _localAiTextWithConfigUrl;
        private readonly string _localAiImageUrl;
        private readonly string _googleAiUrl;
        private readonly string _googleAiAndDbUrl;
        private readonly string _whisperUrl;

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _localAiTextUrl = configuration["LocalAI:Text"];
            _localAiTextWithConfigUrl = configuration["LocalAI:TextWithConfig"];
            _localAiImageUrl = configuration["LocalAI:Image"];
            _googleAiUrl = configuration["LocalAI:Google"];
            _googleAiAndDbUrl = configuration["LocalAI:GoogleDb"];
            _whisperUrl = configuration["LocalAI:Whisper"];
        }

        public async Task<string> GetResponseAsyncUsingLocalImageGenerationAI(GetResponseAsyncUsingLocalImageGenerationAIRequest requestBody)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_localAiImageUrl, requestBody);
                response.EnsureSuccessStatusCode();

                // Read the response as JSON and extract the base64 image string
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();

                // Check if the image_base64 field exists
                if (result.TryGetProperty("image_base64", out var base64Property))
                {
                    return base64Property.GetString() ?? "No image received";
                }
                else
                {
                    throw new Exception("No image base64 string found in the response.");
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> GetResponseAsyncUsingLocalTextGenerationAI(string userMessage)
        {
            try
            {
                var messages = new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        role = "system",
                        content = "You're a helpful chat bot. Answer short and concise in 150 tokens only."
                    },
                    new ChatMessage
                    {
                        role = "user",
                        content = userMessage
                    }
                };

                var request = new ChatRequest
                {
                    messages = messages,
                    temperature = 0.9,
                    max_tokens = 150
                };

                var response = await _httpClient.PostAsJsonAsync(_localAiTextUrl, request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
                return result?.choices?.FirstOrDefault()?.message?.content ?? "No response received";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<ChatResponseWithConfig> GetResponseAsyncUsingLocalTextGenerationAIWithConfig(GetResponseAsyncUsingTextGenerationAIRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_localAiTextWithConfigUrl, request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ChatResponseWithConfig>();
                return result ?? throw new Exception("No GetResponseAsyncUsingLocalTextGenerationAIWithConfig received");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<GoogleChatResponse> GetResponseAsyncUsingGoogleAI(GoogleChatRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_googleAiUrl, request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<GoogleChatResponse>();
                return result ?? throw new Exception("No GetResponseAsyncUsingGooleAI received");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<GoogleChatResponse> GetResponseAsyncUsingGoogleAIAndDb(GoogleChatRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_googleAiAndDbUrl, request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<GoogleChatResponse>();
                return result ?? throw new Exception("No GetResponseAsyncUsingGoogleAIAndDb received");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<WhisperChatResponse> GenerateTranscriptUsingWhisper(WhisperChatRequest request)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = request.file.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.file.ContentType);
                content.Add(streamContent, "file", request.file.FileName);

                var response = await _httpClient.PostAsync(_whisperUrl, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<WhisperChatResponse>();
                return result ?? throw new Exception("No GenerateTranscriptUsingWhisper received");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}
