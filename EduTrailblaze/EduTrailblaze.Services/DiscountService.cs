using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IRepository<Discount, int> _discountRepository;

        public DiscountService(IRepository<Discount, int> discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<Discount?> GetDiscount(int discountId)
        {
            try
            {
                return await _discountRepository.GetByIdAsync(discountId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the discount: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Discount>> GetDiscounts()
        {
            try
            {
                return await _discountRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the discount: " + ex.Message);
            }
        }

        public async Task AddDiscount(Discount discount)
        {
            try
            {
                await _discountRepository.AddAsync(discount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the discount: " + ex.Message);
            }
        }

        public async Task UpdateDiscount(UpdateDiscountRequest discount)
        {
            try
            {
                var discountEntity = await _discountRepository.GetByIdAsync(discount.DiscountId);
                if (discountEntity == null)
                {
                    throw new Exception("Discount not found.");
                }
                discountEntity.DiscountType = discount.DiscountType;
                discountEntity.DiscountValue = discount.DiscountValue;
                discountEntity.StartDate = discount.StartDate;
                discountEntity.EndDate = discount.EndDate;
                discountEntity.IsActive = discount.IsActive;

                await _discountRepository.UpdateAsync(discountEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the discount: " + ex.Message);
            }
        }

        public async Task AddDiscount(CreateDiscountRequest discount)
        {
            try
            {
                var discountEntity = new Discount
                {
                    DiscountType = discount.DiscountType,
                    DiscountValue = discount.DiscountValue,
                    StartDate = discount.StartDate,
                    EndDate = discount.EndDate
                };
                await _discountRepository.AddAsync(discountEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the discount: " + ex.Message);
            }
        }

        public async Task UpdateDiscount(Discount discount)
        {
            try
            {
                await _discountRepository.UpdateAsync(discount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the discount: " + ex.Message);
            }
        }

        public async Task DeleteDiscount(Discount discount)
        {
            try
            {
                await _discountRepository.DeleteAsync(discount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the discount: " + ex.Message);
            }
        }
    }
}
