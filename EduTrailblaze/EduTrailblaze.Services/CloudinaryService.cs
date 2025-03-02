using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;

namespace EduTrailblaze.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            _cloudinary = new Cloudinary(new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
                )
            );
        }

        public async Task<UploadVideoResponse> UploadVideoAsync(string filePath, string publicId)
        {
            try
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(filePath),
                    PublicId = publicId,
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                var duration = TimeSpan.FromSeconds(uploadResult.Duration);

                return new UploadVideoResponse
                {
                    VideoUri = uploadResult.Url.ToString(),
                    Duration = duration
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading video: {ex.Message}");
            }
        }

        public async Task DeleteVideoAsync(string publicId)
        {
            try
            {
                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Video
                };

                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

                if (deletionResult.Result == "ok")
                {
                    Console.WriteLine("Video deleted successfully!");
                }
                else
                {
                    Console.WriteLine($"Failed to delete video. Status: {deletionResult.Result}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting video: {ex.Message}");
            }
        }


        public async Task<string> UploadImageAsync(UploadImageRequest file)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.File.FileName);
            using (var stream = file.File.OpenReadStream())
            {
                var task = new FirebaseStorage("court-callers.appspot.com")
                    .Child("ProfileImage")
                    .Child(fileName)
                    .PutAsync(stream);

                return await task;
            }
        }
    }
}
