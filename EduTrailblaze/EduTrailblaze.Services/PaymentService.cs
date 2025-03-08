using AutoMapper;
using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace EduTrailblaze.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment, int> _paymentRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMapper _mapper;

        public PaymentService(IRepository<Payment, int> paymentRepository, IMapper mapper, IBackgroundJobClient backgroundJobClient)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Payment?> GetPayment(int paymentId)
        {
            try
            {
                return await _paymentRepository.GetByIdAsync(paymentId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the payment: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Payment>> GetPayments()
        {
            try
            {
                return await _paymentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the payment: " + ex.Message);
            }
        }

        public async Task<List<PaymentDTO>> GetPaymentsByCondition(GetPaymentRequest request)
        {
            try
            {
                var dbSet = await _paymentRepository.GetDbSet();

                if (request.OrderId != null)
                {
                    dbSet = dbSet.Where(c => c.OrderId == request.OrderId);
                }

                if (request.MinAmount != null)
                {
                    dbSet = dbSet.Where(c => c.Amount >= request.MinAmount);
                }

                if (request.MaxAmount != null)
                {
                    dbSet = dbSet.Where(c => c.Amount <= request.MaxAmount);
                }

                if (!string.IsNullOrEmpty(request.PaymentMethod))
                {
                    dbSet = dbSet.Where(c => c.PaymentMethod.ToLower().Contains(request.PaymentMethod.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.PaymentStatus))
                {
                    dbSet = dbSet.Where(c => c.PaymentStatus.ToLower().Contains(request.PaymentStatus.ToLower()));
                }

                if (request.FromDate != null)
                {
                    dbSet = dbSet.Where(c => c.PaymentDate >= request.FromDate);
                }

                if (request.ToDate != null)
                {
                    dbSet = dbSet.Where(c => c.PaymentDate <= request.ToDate);
                }

                var payments = await dbSet.ToListAsync();

                return _mapper.Map<List<PaymentDTO>>(payments);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the payment: " + ex.Message);
            }
        }

        public async Task AddPayment(Payment payment)
        {
            try
            {
                await _paymentRepository.AddAsync(payment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the payment: " + ex.Message);
            }
        }

        public async Task<Payment> AddPayment(CreatePaymentRequest payment)
        {
            try
            {
                var newPayment = new Payment
                {
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod
                };
                await _paymentRepository.AddAsync(newPayment);

                _backgroundJobClient.Schedule(() => AutomaticFailedPayment(newPayment.Id), TimeSpan.FromMinutes(15));

                return newPayment;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the payment: " + ex.Message);
            }
        }

        public async Task AutomaticFailedPayment(int paymentId)
        {
            try
            {
                var payment = await GetPayment(paymentId);

                if (payment == null)
                {
                    throw new Exception("Order not found.");
                }

                if (payment.PaymentStatus == "Processing")
                {
                    payment.PaymentStatus = "Failed";
                    await UpdatePayment(payment);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while automatically failing the payment: " + ex.Message);
            }
        }

        public async Task UpdatePayment(Payment payment)
        {
            try
            {
                await _paymentRepository.UpdateAsync(payment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the payment: " + ex.Message);
            }
        }

        public async Task UpdatePayment(UpdatePaymentRequest payment)
        {
            try
            {
                var existingPayment = await _paymentRepository.GetByIdAsync(payment.PaymentId);
                if (existingPayment == null)
                {
                    throw new Exception("Payment not found.");
                }
                existingPayment.PaymentStatus = payment.PaymentStatus;
                await _paymentRepository.UpdateAsync(existingPayment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the payment: " + ex.Message);
            }
        }

        public async Task DeletePayment(Payment payment)
        {
            try
            {
                await _paymentRepository.DeleteAsync(payment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the payment: " + ex.Message);
            }
        }
    }
}
