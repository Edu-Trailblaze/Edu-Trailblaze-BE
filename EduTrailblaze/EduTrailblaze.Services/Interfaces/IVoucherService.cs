using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<Voucher?> GetVoucher(int voucherId);

        Task<IEnumerable<Voucher>> GetVouchers();

        Task AddVoucher(Voucher voucher);

        Task UpdateVoucher(Voucher voucher);

        Task AddVoucher(CreateVoucherRequest voucher);

        Task UpdateVoucher(UpdateVoucherRequest voucher);

        Task DeleteVoucher(Voucher voucher);

        Task<Voucher?> CheckVoucherValidity(string voucherCode, decimal orderPrice);

        Task<List<VoucherDTO>?> GetVouchersByConditions(GetVouchersRequest request);

        Task<PaginatedList<VoucherDTO>> GetVoucherInformation(GetVouchersRequest request, Paging paging);
    }
}
