using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<UploadVideoResponse> UploadVideoAsync(string filePath, string publicId);

        Task DeleteVideoAsync(string publicId);

        Task<string> UploadImageAsync(UploadImageRequest file);

        Task<string> UploadFileAsync(string filePath, string publicId);
    }
}
