using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IAIService
    {
        Task<string> GetResponseAsyncUsingLocalTextGenerationAI(string userMessage);
        Task<string> GetResponseAsyncUsingLocalImageGenerationAI(GetResponseAsyncUsingLocalImageGenerationAIRequest requestBody);
        Task<ChatResponseWithConfig> GetResponseAsyncUsingLocalTextGenerationAIWithConfig(GetResponseAsyncUsingTextGenerationAIRequest request);
        Task<GoogleChatResponse> GetResponseAsyncUsingGoogleAI(GoogleChatRequest request);
        Task<GoogleChatResponse> GetResponseAsyncUsingGoogleAIAndDb(GoogleChatRequest request);
        Task<WhisperChatResponse> GenerateTranscriptUsingWhisper(WhisperChatRequest request);
    }
}
