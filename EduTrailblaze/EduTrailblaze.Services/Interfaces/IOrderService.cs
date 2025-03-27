using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Models;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order?> GetOrder(int orderId);

        Task<IEnumerable<Order>> GetOrders();
        Task<ApiResponse> TotalRevenueByMonth(int month, int year);
        Task AddOrder(Order order);

        Task UpdateOrder(Order order);

        Task DeleteOrder(Order order);

        Task AutomaticFailedOrder(int orderId);

        Task<string> PlaceOrder(PlaceOrderRequest orderProcessingRequest);

        Task<string> PaymentProcessing(int orderId, string paymentMethod);

        Task CancelOrder(int orderId);

        Task<IQueryable<Order>> GetByCondition(GetOrdersRequest req);

        Task<List<Order>> GetOrdersByCondition(GetOrdersRequest req);

        Task<PaginatedList<OrderDTO>> GetPagingOrders(GetOrdersRequest req, Paging paging);
    }
}
