using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class CourseCouponService : ICourseCouponService
    {
        private readonly IRepository<CourseCoupon, int> _courseCouponRepository;

        public CourseCouponService(IRepository<CourseCoupon, int> courseCouponRepository)
        {
            _courseCouponRepository = courseCouponRepository;
        }

        public async Task<CourseCoupon?> GetCourseCoupon(int courseCouponId)
        {
            try
            {
                return await _courseCouponRepository.GetByIdAsync(courseCouponId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseCoupon: " + ex.Message);
            }
        }

        public async Task<CourseCoupon?> GetCourseCoupon(int courseId, int couponId)
        {
            try
            {
                var dbSet = await _courseCouponRepository.GetDbSet();
                var courseCoupon = await dbSet.FirstOrDefaultAsync(x => x.CourseId == courseId && x.CouponId == couponId);
                return courseCoupon;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseCoupon: " + ex.Message);
            }
        }

        public async Task<IEnumerable<CourseCoupon>> GetCourseCoupons()
        {
            try
            {
                return await _courseCouponRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseCoupon: " + ex.Message);
            }
        }

        public async Task AddCourseCoupon(CourseCoupon courseCoupon)
        {
            try
            {
                await _courseCouponRepository.AddAsync(courseCoupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the courseCoupon: " + ex.Message);
            }
        }

        public async Task UpdateCourseCoupon(CourseCoupon courseCoupon)
        {
            try
            {
                await _courseCouponRepository.UpdateAsync(courseCoupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the courseCoupon: " + ex.Message);
            }
        }

        public async Task DeleteCourseCoupon(CourseCoupon courseCoupon)
        {
            try
            {
                await _courseCouponRepository.DeleteAsync(courseCoupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the courseCoupon: " + ex.Message);
            }
        }
    }
}
