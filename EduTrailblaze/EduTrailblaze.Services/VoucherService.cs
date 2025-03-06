using AutoMapper;
using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IRepository<Voucher, int> _voucherRepository;
        private readonly IMapper _mapper;

        public VoucherService(IRepository<Voucher, int> voucherRepository, IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<Voucher?> GetVoucher(int voucherId)
        {
            try
            {
                return await _voucherRepository.GetByIdAsync(voucherId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the voucher: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Voucher>> GetVouchers()
        {
            try
            {
                return await _voucherRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the voucher: " + ex.Message);
            }
        }

        public async Task AddVoucher(Voucher voucher)
        {
            try
            {
                await _voucherRepository.AddAsync(voucher);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the voucher: " + ex.Message);
            }
        }

        public async Task UpdateVoucher(Voucher voucher)
        {
            try
            {
                await _voucherRepository.UpdateAsync(voucher);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the voucher: " + ex.Message);
            }
        }

        public async Task AddVoucher(CreateVoucherRequest voucher)
        {
            try
            {
                var newVoucher = new Voucher
                {
                    VoucherCode = voucher.VoucherCode,
                    DiscountType = voucher.DiscountType,
                    DiscountValue = voucher.DiscountValue,
                    ExpiryDate = voucher.ExpiryDate
                };
                await _voucherRepository.AddAsync(newVoucher);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the voucher: " + ex.Message);
            }
        }

        public async Task UpdateVoucher(UpdateVoucherRequest voucher)
        {
            try
            {
                var voucherToUpdate = await _voucherRepository.GetByIdAsync(voucher.VoucherId);
                if (voucherToUpdate == null)
                {
                    throw new Exception("Voucher not found.");
                }
                voucherToUpdate.VoucherCode = voucher.VoucherCode;
                voucherToUpdate.DiscountType = voucher.DiscountType;
                voucherToUpdate.DiscountValue = voucher.DiscountValue;
                voucherToUpdate.ExpiryDate = voucher.ExpiryDate;
                voucherToUpdate.IsUsed = voucher.IsUsed;

                await _voucherRepository.UpdateAsync(voucherToUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the voucher: " + ex.Message);
            }
        }

        public async Task DeleteVoucher(Voucher voucher)
        {
            try
            {
                await _voucherRepository.DeleteAsync(voucher);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the voucher: " + ex.Message);
            }
        }

        public async Task<Voucher?> CheckVoucherValidity(string voucherCode, decimal orderPrice)
        {
            try
            {
                var voucherDbSet = await _voucherRepository.GetDbSet();
                var voucher = await voucherDbSet.FirstOrDefaultAsync(v => v.ExpiryDate < DateTimeHelper.GetVietnamTime() && v.VoucherCode == voucherCode && v.IsUsed);

                if (voucher == null)
                {
                    return null;
                }
                if (voucher.MinimumOrderValue.HasValue && orderPrice < voucher.MinimumOrderValue)
                {
                    return null;
                }
                if (voucher.OrderId.HasValue)
                {
                    return null;
                }
                return voucher;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking the voucher validity: " + ex.Message);
            }
        }

        public async Task<List<VoucherDTO>?> GetVouchersByConditions(GetVouchersRequest request)
        {
            try
            {
                var dbSet = await _voucherRepository.GetDbSet();

                if (request.IsUsed != null)
                {
                    dbSet = dbSet.Where(c => c.IsUsed == request.IsUsed);
                }

                if (request.IsValid != null)
                {
                    if (request.IsValid == true)
                    {
                        dbSet = dbSet.Where(c => c.ExpiryDate.HasValue && c.ExpiryDate >= DateTimeHelper.GetVietnamTime() && c.StartDate.HasValue && c.StartDate <= DateTimeHelper.GetVietnamTime() && !c.IsUsed);
                    }
                    else
                    {
                        dbSet = dbSet.Where(c => !c.ExpiryDate.HasValue || c.ExpiryDate < DateTimeHelper.GetVietnamTime() || !c.StartDate.HasValue || c.StartDate > DateTimeHelper.GetVietnamTime());
                    }
                }

                if (request.DiscountType != null)
                {
                    dbSet = dbSet.Where(c => c.DiscountType == request.DiscountType);
                }

                if (request.MinimumOrderValue != null)
                {
                    dbSet = dbSet.Where(c => c.MinimumOrderValue <= request.MinimumOrderValue);
                }

                if (request.DiscountValueMin != null)
                {
                    dbSet = dbSet.Where(c => c.DiscountValue >= request.DiscountValueMin);
                }

                if (request.DiscountValueMax != null)
                {
                    dbSet = dbSet.Where(c => c.DiscountValue <= request.DiscountValueMax);
                }

                if (request.StartDate != null)
                {
                    dbSet = dbSet.Where(c => c.StartDate >= request.StartDate);
                }

                if (request.ExpiryDate != null)
                {
                    dbSet = dbSet.Where(c => c.ExpiryDate <= request.ExpiryDate);
                }

                var items = await dbSet.ToListAsync();

                var voucherDTO = _mapper.Map<List<VoucherDTO>>(items);

                return voucherDTO;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courses: " + ex.Message);
            }
        }

        public async Task<PaginatedList<VoucherDTO>> GetVoucherInformation(GetVouchersRequest request, Paging paging)
        {
            try
            {
                var vouchers = await GetVouchersByConditions(request);

                if (vouchers == null)
                {
                    return new PaginatedList<VoucherDTO>(new List<VoucherDTO>(), 0, 1, 10);
                }

                if (!paging.PageSize.HasValue || paging.PageSize <= 0)
                {
                    paging.PageSize = 10;
                }

                if (!paging.PageIndex.HasValue || paging.PageIndex <= 0)
                {
                    paging.PageIndex = 1;
                }

                var totalCount = vouchers.Count;
                var skip = (paging.PageIndex.Value - 1) * paging.PageSize.Value;
                var take = paging.PageSize.Value;

                var validSortOptions = new[] { "highest_value", "order_value" };
                if (string.IsNullOrEmpty(paging.Sort) || !validSortOptions.Contains(paging.Sort))
                {
                    paging.Sort = "highest_value";
                }

                vouchers = paging.Sort switch
                {
                    "highest_value" => vouchers.OrderBy(p => p.DiscountType).ThenByDescending(p => p.DiscountValue).ToList(),
                    "order_value" => vouchers.OrderByDescending(p => p.MinimumOrderValue).ToList(),
                    _ => vouchers
                };

                var paginatedCourseCards = vouchers.Skip(skip).Take(take).ToList();

                return new PaginatedList<VoucherDTO>(paginatedCourseCards, totalCount, paging.PageIndex.Value, paging.PageSize.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courses: " + ex.Message);
            }
        }
    }
}
