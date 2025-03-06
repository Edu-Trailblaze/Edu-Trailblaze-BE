using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class CouponService : ICouponService
    {
        private readonly IRepository<Coupon, int> _couponRepository;
        private readonly IRepository<CourseCoupon, int> _courseCouponRepository;
        private readonly IRepository<UserCourseCoupon, int> _userCourseCouponRepository;

        public CouponService(IRepository<Coupon, int> couponRepository, IRepository<CourseCoupon, int> courseCouponRepository, IRepository<UserCourseCoupon, int> userCourseCouponRepository)
        {
            _couponRepository = couponRepository;
            _courseCouponRepository = courseCouponRepository;
            _userCourseCouponRepository = userCourseCouponRepository;
        }

        public async Task<Coupon?> GetCoupon(int couponId)
        {
            try
            {
                return await _couponRepository.GetByIdAsync(couponId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the coupon: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Coupon>> GetCoupons()
        {
            try
            {
                return await _couponRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the coupon: " + ex.Message);
            }
        }

        public async Task AddCoupon(Coupon coupon)
        {
            try
            {
                await _couponRepository.AddAsync(coupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the coupon: " + ex.Message);
            }
        }

        public async Task UpdateCoupon(Coupon coupon)
        {
            try
            {
                await _couponRepository.UpdateAsync(coupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the coupon: " + ex.Message);
            }
        }

        public async Task AddCoupon(CreateCouponRequest coupon)
        {
            try
            {
                var newCoupon = new Coupon
                {
                    Code = coupon.Code,
                    DiscountType = coupon.DiscountType,
                    DiscountValue = coupon.DiscountValue,
                    ExpiryDate = coupon.ExpiryDate,
                };
                await _couponRepository.AddAsync(newCoupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the coupon: " + ex.Message);
            }
        }

        public async Task UpdateCoupon(UpdateCouponRequest coupon)
        {
            try
            {
                var existingCoupon = await _couponRepository.GetByIdAsync(coupon.CouponId);
                if (existingCoupon == null)
                {
                    throw new Exception("Coupon not found.");
                }
                existingCoupon.Code = coupon.Code;
                existingCoupon.DiscountType = coupon.DiscountType;
                existingCoupon.DiscountValue = coupon.DiscountValue;
                existingCoupon.ExpiryDate = coupon.ExpiryDate;
                existingCoupon.IsActive = coupon.IsActive;

                await _couponRepository.UpdateAsync(existingCoupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the coupon: " + ex.Message);
            }
        }

        public async Task DeleteCoupon(Coupon coupon)
        {
            try
            {
                await _couponRepository.DeleteAsync(coupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the coupon: " + ex.Message);
            }
        }

        public async Task<List<Coupon>> GetValidCoupons()
        {
            try
            {
                var dbSet = await _couponRepository.GetDbSet();
                var currentDate = DateTimeHelper.GetVietnamTime();

                var list = await dbSet
                    .Where(c => c.IsActive
                                && (c.ExpiryDate == null || c.ExpiryDate >= currentDate)
                                && (c.StartDate == null || c.StartDate <= currentDate)
                                && (c.MaxUsage == null || c.MaxUsage > c.UsageCount))
                    .ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the coupon: " + ex.Message);
            }
        }

        public async Task<Coupon?> GetCouponByCouponCode(string couponCode)
        {
            try
            {
                var validCoupons = await GetValidCoupons();
                return validCoupons.FirstOrDefault(c => c.Code == couponCode);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the coupon: " + ex.Message);
            }
        }

        public async Task ApplyCoupon(string couponCode, string userId, int courseId)
        {
            try
            {
                var userCourseCoupon = await _userCourseCouponRepository.GetDbSet();
                var userCourseCouponExist = await userCourseCoupon.AnyAsync(x => x.UserId == userId && x.CourseCoupon.CourseId == courseId && x.CourseCoupon.Coupon.Code == couponCode);

                if (userCourseCouponExist)
                {
                    throw new Exception("Coupon already applied.");
                }

                var coupon = await GetCouponByCouponCode(couponCode);

                if (coupon == null)
                {
                    throw new Exception("Invalid coupon code.");
                }

                var courseCouponDbSet = await _courseCouponRepository.GetDbSet();

                var courseCoupon = await courseCouponDbSet.FirstOrDefaultAsync(x => x.CourseId == courseId && x.CouponId == coupon.Id);

                if (courseCoupon == null)
                {
                    throw new Exception("Coupon is not valid for this course.");
                }

                await _userCourseCouponRepository.AddAsync(new UserCourseCoupon
                {
                    CourseCouponId = courseCoupon.Id,
                    UserId = userId,
                });

                coupon.UsageCount++;
                await UpdateCoupon(coupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while applying the coupon: " + ex.Message);
            }
        }

        public async Task RemoveCoupon(string couponCode, string userId, int courseId)
        {
            try
            {
                var userCourseCoupon = await _userCourseCouponRepository.GetDbSet();
                var userCourseCouponExist = await userCourseCoupon.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseCoupon.CourseId == courseId && x.CourseCoupon.Coupon.Code == couponCode);

                if (userCourseCouponExist == null)
                {
                    throw new Exception("Coupon not applied.");
                }
                await _userCourseCouponRepository.DeleteAsync(userCourseCouponExist);
                var coupon = await GetCouponByCouponCode(couponCode);
                if (coupon != null)
                {
                    coupon.UsageCount--;
                    await UpdateCoupon(coupon);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while removing the coupon: " + ex.Message);
            }
        }
    }
}
