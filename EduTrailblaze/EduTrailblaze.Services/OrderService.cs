using AutoMapper;
using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IMailService _mailService;
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IMoMoService _moMoService;
        private readonly IVNPAYService _vNPAYService;
        private readonly IMapper _mapper;
        private readonly IPayPalService _payPalService;

        public OrderService(IRepository<Order, int> orderRepository, IPaymentService paymentService, ICartService cartService, IOrderDetailService orderDetailService, IMoMoService moMoService, IVNPAYService vNPAYService, IMapper mapper, IPayPalService payPalService)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _cartService = cartService;
            _orderDetailService = orderDetailService;
            _moMoService = moMoService;
            _vNPAYService = vNPAYService;
            _mapper = mapper;
            _payPalService = payPalService;
        }

        public async Task<Order?> GetOrder(int orderId)
        {
            try
            {
                return await _orderRepository.GetByIdAsync(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the order: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            try
            {
                return await _orderRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the order: " + ex.Message);
            }
        }

        public async Task AddOrder(Order order)
        {
            try
            {
                await _orderRepository.AddAsync(order);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the order: " + ex.Message);
            }
        }

        public async Task AddOrder(CreateOrderRequest order)
        {
            try
            {
                var newOrder = new Order
                {
                    UserId = order.UserId,
                    OrderAmount = order.OrderAmount,
                };
                await _orderRepository.AddAsync(newOrder);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the order: " + ex.Message);
            }
        }

        public async Task UpdateOrder(Order order)
        {
            try
            {
                await _orderRepository.UpdateAsync(order);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order: " + ex.Message);
            }
        }

        public async Task UpdateOrder(UpdateOrderRequest order)
        {
            try
            {
                var existingCourse = await _orderRepository.GetByIdAsync(order.OrderId);
                if (existingCourse == null)
                {
                    throw new Exception("Course not found.");
                }
                existingCourse.OrderStatus = order.OrderStatus;
                await _orderRepository.UpdateAsync(existingCourse);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order: " + ex.Message);
            }
        }

        public async Task DeleteOrder(Order order)
        {
            try
            {
                await _orderRepository.DeleteAsync(order);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the order: " + ex.Message);
            }
        }

        public async Task<Order> OrderProcessing(string userId)
        {
            try
            {
                var cart = await _cartService.ViewCart(userId);

                if (cart == null || cart.CartItems == null || cart.CartItems.Count == 0)
                {
                    throw new Exception("Cart is empty.");
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderAmount = cart.TotalPrice,
                };

                await AddOrder(order);

                foreach (var cartItem in cart.CartItems)
                {
                    var orderDetail = new OrderDetailRequest
                    {
                        OrderId = order.Id,
                        CourseId = cartItem.CartCourseInformation.Id,
                        Price = cartItem.TotalCoursePrice,
                    };

                    await _orderDetailService.AddOrderDetail(orderDetail);
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order: " + ex.Message);
            }
        }

        public async Task<string> PaymentProcessing(int orderId, string paymentMethod)
        {
            try
            {
                var order = await GetOrder(orderId);
                if (order == null)
                {
                    throw new Exception("Order not found.");
                }

                CreatePaymentRequest createPaymentRequest = new CreatePaymentRequest
                {
                    OrderId = orderId,
                    PaymentMethod = paymentMethod,
                    Amount = order.OrderAmount,
                };

                var payment = await _paymentService.AddPayment(createPaymentRequest);

                string paymentUrl = "";

                if (paymentMethod == "MoMo")
                {
                    paymentUrl = await _moMoService.CreatePaymentUrl(order.OrderAmount, orderId, payment.Id);
                }
                else if (paymentMethod == "VnPay")
                {
                    paymentUrl = _vNPAYService.CreatePaymentUrl(order.OrderAmount, orderId, payment.Id);
                }
                else if (paymentMethod == "PayPal")
                {
                    paymentUrl = await _payPalService.CreatePaymentUrl(order.OrderAmount, orderId, payment.Id);
                }
                return paymentUrl;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the payment: " + ex.Message);
            }
        }

        public async Task<string> PlaceOrder(PlaceOrderRequest placeOrderRequest)
        {
            try
            {
                var order = await OrderProcessing(placeOrderRequest.UserId);

                if (order == null)
                {
                    throw new Exception("Order not found.");
                }

                var paymentResponse = await PaymentProcessing(order.Id, placeOrderRequest.PaymentMethod);

                return paymentResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while placing the order: " + ex.Message);
            }
        }

        public async Task AutomaticFailedOrder(int orderId)
        {
            try
            {
                var order = await GetOrder(orderId);

                if (order == null)
                {
                    throw new Exception("Order not found.");
                }

                if (order.OrderStatus == "Pending")
                {
                    order.OrderStatus = "Failed";
                    await UpdateOrder(order);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while automatically failing the order: " + ex.Message);
            }
        }

        public async Task CancelOrder(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);

                if (order == null)
                {
                    throw new Exception("Order not found.");
                }
                order.OrderStatus = "Cancelled";

                await UpdateOrder(order);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while canceling the order: " + ex.Message);
            }
        }

        public async Task<IQueryable<Order>> GetByCondition(GetOrdersRequest req)
        {
            try
            {
                var orderDbSet = await _orderRepository.GetDbSet();

                if (!string.IsNullOrEmpty(req.UserId))
                {
                    orderDbSet = orderDbSet.Where(x => x.UserId == req.UserId);
                }

                if (!string.IsNullOrEmpty(req.OrderStatus))
                {
                    orderDbSet = orderDbSet.Where(x => x.OrderStatus == req.OrderStatus);
                }

                if (req.MinOrderAmount.HasValue)
                {
                    orderDbSet = orderDbSet.Where(x => x.OrderAmount >= req.MinOrderAmount);
                }

                if (req.MaxOrderAmount.HasValue)
                {
                    orderDbSet = orderDbSet.Where(x => x.OrderAmount <= req.MaxOrderAmount);
                }

                if (req.OrderDateFrom.HasValue)
                {
                    orderDbSet = orderDbSet.Where(x => x.OrderDate >= req.OrderDateFrom);
                }

                if (req.OrderDateTo.HasValue)
                {
                    orderDbSet = orderDbSet.Where(x => x.OrderDate <= req.OrderDateTo);
                }

                return orderDbSet;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the orders: " + ex.Message);
            }
        }

        public async Task<List<Order>> GetOrdersByCondition(GetOrdersRequest req)
        {
            try
            {
                var orders = await GetByCondition(req);
                return await orders.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the orders: " + ex.Message);
            }
        }

        public async Task<PaginatedList<OrderDTO>> GetPagingOrders(GetOrdersRequest req, Paging paging)
        {
            try
            {
                var orders = await GetByCondition(req);
                if (!paging.PageSize.HasValue || paging.PageSize <= 0)
                {
                    paging.PageSize = 10;
                }

                if (!paging.PageIndex.HasValue || paging.PageIndex <= 0)
                {
                    paging.PageIndex = 1;
                }

                var totalCount = orders.Count();
                var skip = (paging.PageIndex.Value - 1) * paging.PageSize.Value;
                var take = paging.PageSize.Value;

                var validSortOptions = new[] { "status", "highest_price", "newest" };
                if (string.IsNullOrEmpty(paging.Sort) || !validSortOptions.Contains(paging.Sort))
                {
                    paging.Sort = "newest";
                }

                orders = paging.Sort switch
                {
                    "status" => orders.OrderByDescending(p => p.OrderStatus),
                    "highest_price" => orders.OrderByDescending(p => p.OrderAmount),
                    "newest" => orders.OrderByDescending(p => p.OrderDate),
                    _ => orders
                };

                var orderList = orders.Skip(skip).Take(take).ToList();
                var orderDTOList = _mapper.Map<List<OrderDTO>>(orderList);

                return new PaginatedList<OrderDTO>(orderDTOList, totalCount, paging.PageIndex.Value, paging.PageSize.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the orders: " + ex.Message);
            }
        }
    }
}
