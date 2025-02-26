using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IVideoService
    {
        Task<Video?> GetVideo(int videoId);

        Task<IEnumerable<Video>> GetVideos();

        Task AddVideo(Video video);

        Task UpdateVideo(Video video);

        Task DeleteVideo(Video video);

        Task AddVideo(CreateVideoRequest video);

        Task UpdateVideo(UpdateVideoRequest video);

        Task DeleteVideo(int video);

        Task<UploadVideoResponse> UploadVideoAsync(UploadVideoRequest video);

        Task<int> UploadVideoWithCloudinaryAsync(UploadVideoRequest video);

        Task<List<VideoDTO>?> GetVideosByConditions(GetVideosRequest request);
    }
}
