using EduTrailblaze.Services.DTOs;
using Firebase.Storage;

namespace EduTrailblaze.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<UploadVideoResponse> UploadVideoAsync(string filePath, string publicId);

        Task DeleteVideoAsync(string publicId);
        Task<string> UploadImageAsync(UploadImageRequest file);
        
    }
}
