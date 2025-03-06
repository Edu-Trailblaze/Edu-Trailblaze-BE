using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IRepository<OrderDetail, int> _orderDetailRepository;

        public OrderDetailService(IRepository<OrderDetail, int> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<OrderDetail?> GetOrderDetail(int orderDetailId)
        {
            try
            {
                return await _orderDetailRepository.GetByIdAsync(orderDetailId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the orderDetail: " + ex.Message);
            }
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetails()
        {
            try
            {
                return await _orderDetailRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the orderDetail: " + ex.Message);
            }
        }

        public async Task AddOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                await _orderDetailRepository.AddAsync(orderDetail);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the orderDetail: " + ex.Message);
            }
        }

        public async Task AddOrderDetail(OrderDetailRequest orderDetail)
        {
            try
            {
                var newOrderDetail = new OrderDetail
                {
                    OrderId = orderDetail.OrderId,
                    CourseId = orderDetail.CourseId,
                    Price = orderDetail.Price
                };
                await _orderDetailRepository.AddAsync(newOrderDetail);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the orderDetail: " + ex.Message);
            }
        }

        public async Task UpdateOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                await _orderDetailRepository.UpdateAsync(orderDetail);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the orderDetail: " + ex.Message);
            }
        }

        public async Task DeleteOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                await _orderDetailRepository.DeleteAsync(orderDetail);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the orderDetail: " + ex.Message);
            }
        }
    }
}
