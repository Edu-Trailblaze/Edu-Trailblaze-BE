using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class CourseDiscountService : ICourseDiscountService
    {
        private readonly IRepository<CourseDiscount, int> _courseDiscountRepository;

        public CourseDiscountService(IRepository<CourseDiscount, int> courseDiscountRepository)
        {
            _courseDiscountRepository = courseDiscountRepository;
        }

        public async Task<CourseDiscount?> GetCourseDiscount(int courseDiscountId)
        {
            try
            {
                return await _courseDiscountRepository.GetByIdAsync(courseDiscountId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseDiscount: " + ex.Message);
            }
        }

        public async Task<IEnumerable<CourseDiscount>> GetCourseDiscounts()
        {
            try
            {
                return await _courseDiscountRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseDiscount: " + ex.Message);
            }
        }

        public async Task AddCourseDiscount(CourseDiscount courseDiscount)
        {
            try
            {
                await _courseDiscountRepository.AddAsync(courseDiscount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the courseDiscount: " + ex.Message);
            }
        }

        public async Task UpdateCourseDiscount(CourseDiscount courseDiscount)
        {
            try
            {
                await _courseDiscountRepository.UpdateAsync(courseDiscount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the courseDiscount: " + ex.Message);
            }
        }

        public async Task DeleteCourseDiscount(CourseDiscount courseDiscount)
        {
            try
            {
                await _courseDiscountRepository.DeleteAsync(courseDiscount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the courseDiscount: " + ex.Message);
            }
        }
    }
}
