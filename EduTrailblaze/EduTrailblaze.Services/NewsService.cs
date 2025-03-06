using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class NewsService : INewsService
    {
        private readonly IRepository<News, int> _newsRepository;

        public NewsService(IRepository<News, int> newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<News?> GetNews(int newsId)
        {
            try
            {
                return await _newsRepository.GetByIdAsync(newsId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the news: " + ex.Message);
            }
        }

        public async Task<IEnumerable<News>> GetNews()
        {
            try
            {
                return await _newsRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the news: " + ex.Message);
            }
        }

        public async Task AddNews(News news)
        {
            try
            {
                await _newsRepository.AddAsync(news);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the news: " + ex.Message);
            }
        }

        public async Task UpdateNews(News news)
        {
            try
            {
                await _newsRepository.UpdateAsync(news);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the news: " + ex.Message);
            }
        }

        public async Task DeleteNews(News news)
        {
            try
            {
                await _newsRepository.DeleteAsync(news);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the news: " + ex.Message);
            }
        }

        public async Task AddNews(CreateNewsRequest news)
        {
            try
            {
                var newsEntity = new News
                {
                    Title = news.Title,
                    Content = news.Content,
                    ImageUrl = news.ImageUrl,
                };
                await _newsRepository.AddAsync(newsEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the news: " + ex.Message);
            }
        }

        public async Task UpdateNews(UpdateNewsRequest news)
        {
            try
            {
                var newsEntity = await _newsRepository.GetByIdAsync(news.NewsId);
                if (newsEntity == null)
                {
                    throw new Exception("News not found.");
                }
                newsEntity.Title = news.Title;
                newsEntity.Content = news.Content;
                newsEntity.ImageUrl = news.ImageUrl;
                newsEntity.UpdatedAt = DateTimeHelper.GetVietnamTime();
                await _newsRepository.UpdateAsync(newsEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the news: " + ex.Message);
            }
        }

        public async Task DeleteNews(int newsId)
        {
            try
            {
                var news = await _newsRepository.GetByIdAsync(newsId);
                if (news == null)
                {
                    throw new Exception("News not found.");
                }

                news.IsDeleted = true;
                await _newsRepository.UpdateAsync(news);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the news: " + ex.Message);
            }
        }
    }
}
