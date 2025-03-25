using AutoMapper;
using Cart.Application.Common.Interfaces;
using Cart.Application.Common.Models;
using Cart.Domain.Entities;
using Cart.Infrastructure.Helper;
using Contracts.Common.Interfaces;
using EventBus.Messages.Events;
using EventBus.Messages.Interfaces;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net.Http;
using CartItemInformations = EventBus.Messages.Events.CartItemInformation;
using Entity = Cart.Domain.Entities;
using ILogger = Serilog.ILogger;

namespace Cart.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        //that still stuff between rediscache and _database
        private readonly IDistributedCache _redisCacheService;
        private readonly IDatabase _database;
        private readonly ISerializeService _serializeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IRequestClient<EventBus.Messages.Events.GetCourseRequest> _requestCourseClient;
        private readonly IRequestClient<EventBus.Messages.Events.GetInstructorRequest> _requestInstructorClient;
        private readonly IRequestClient<EventBus.Messages.Events.GetDiscountRequest> _requestDiscountClient;
        private readonly IRequestClient<EventBus.Messages.Events.GetCouponRequest> _requestCounponClient;
        private readonly IRequestClient<EventBus.Messages.Events.GetReviewRequest> _requestReviewClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public CartRepository(IDistributedCache distributedCache, ISerializeService serializeService, ILogger logger, IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer connectionMultiplexer, IPublishEndpoint publishEndpoint, IRequestClient<EventBus.Messages.Events.GetCourseRequest> requestCourseClient, IRequestClient<EventBus.Messages.Events.GetInstructorRequest> requestInstructorClient, IRequestClient<EventBus.Messages.Events.GetDiscountRequest> requestDiscountClient, IRequestClient<GetCouponRequest> requestCounponClient, IRequestClient<EventBus.Messages.Events.GetReviewRequest> requestReviewClient, IMapper mapper)
        {
            _redisCacheService = distributedCache;
            _serializeService = serializeService;
            _logger = logger;
            _database = connectionMultiplexer.GetDatabase();
            _httpContextAccessor = httpContextAccessor;
            _publishEndpoint = publishEndpoint;
            _requestCourseClient = requestCourseClient;
            _requestDiscountClient = requestDiscountClient;
            _requestInstructorClient = requestInstructorClient;
            _requestCounponClient = requestCounponClient;
            _requestReviewClient = requestReviewClient;
            _mapper = mapper;
        }
        public async Task AddCartItemToRedis(string userId, CartItemDTO cartItem)
        {
            var cartKey = $"cart:{userId}"; // Khóa giỏ hàng
            var cartItemsKey = $"CartItems:{userId}"; // Khóa CartItems của người dùng

            // Lưu thông tin cart nếu chưa có trong Redis
            var cartJson = await _database.StringGetAsync(cartKey);
            if (!cartJson.HasValue)
            {
                var cart = new Entity.Cart
                {
                    UserId = userId,
                    IsActive = true,
                    UpdatedAt = DateTime.UtcNow
                };
                await _database.StringSetAsync(cartKey, JsonConvert.SerializeObject(cart));
            }

            // Lưu CartItem vào CartItems
            await _database.ListRightPushAsync(cartItemsKey, JsonConvert.SerializeObject(cartItem));
        }

        public async Task<List<CartItemDTO>> GetCartItemsFromRedis(string cartId)
        {
            var cartItemsKey = $"CartItems:{cartId}";  // Sử dụng cartId làm khóa

            // Lấy tất cả các CartItems từ Redis (dưới dạng JSON)
            var cartItemsJson = await _database.ListRangeAsync(cartItemsKey);

            // Deserialize các CartItems từ Redis vào CartItemDTO
            return cartItemsJson.Select(item => JsonConvert.DeserializeObject<CartItemDTO>(item)).ToList();
        }

        public async Task<Entity.Cart> GetSystemCart(string userId)
        {
            try
            {
                var cartKey = $"Cart:{userId}";
                var cartJson = await _database.StringGetAsync(cartKey);
                Entity.Cart cart;

                if (!cartJson.HasValue)
                {
                    cart = await CreateSystemCart(userId);  // Tạo giỏ hàng mới nếu chưa có
                }
                else
                {
                    cart = JsonConvert.DeserializeObject<Entity.Cart>(cartJson);  // Deserialize từ Redis
                }

                return cart;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the Cart: " + ex.Message);
            }
        }
        #region private func 
        private async Task<List<CartItemDTO>> GetSystemCartItemsFromRedis(string userId)
        {
            var cartItems = new List<CartItemDTO>();

            var redisCartKey = $"cart:{userId}"; 
            var redisCartItemKeys = await _database.ListRangeAsync(redisCartKey); 

            // Duyệt qua các CartItemId trong Redis và lấy chi tiết của từng mặt hàng
            foreach (var cartItemKey in redisCartItemKeys)
            {
                var cartItemDetailKey = $"cart_item:{cartItemKey}";
                var cartItemDetail = await _database.HashGetAllAsync(cartItemDetailKey);

                if (cartItemDetail.Any())
                {
                    var cartItem = new CartItemDTO
                    {
                        ItemId = int.Parse(cartItemDetail.FirstOrDefault(x => x.Name == "CourseId").Value),
                    };
                    cartItems.Add(cartItem);
                }
            }

            return cartItems;
        }
        public async Task SaveCartToRedis(string userId, bool isActive)
        {
            var redisCartKey = $"cart:{userId}";
            var cartHash = new Dictionary<string, string>
        {
            { "UserId", userId },
            { "IsActive", isActive.ToString() }
        };
            var cartHashEntries = cartHash.Select(kvp => new HashEntry(kvp.Key, kvp.Value)).ToArray();

            // Lưu Giỏ Hàng vào Redis
            await _database.HashSetAsync(redisCartKey, cartHashEntries);
        }
        //public async Task SaveCartItemToRedis(int cartId, int courseId, CartItem cartItem)
        //{
        //    var redisCartItemKey = $"cart_item:{cartId}:{courseId}";
        //    var cartItemHash = new Dictionary<string, string>
        //{
        //    { "CartId", cartId.ToString() },
        //    { "CourseId", courseId.ToString() }
        //};
        //    var cartItemHashEntries = cartItemHash.Select(kvp => new HashEntry(kvp.Key, kvp.Value)).ToArray();

        //    // Lưu mặt hàng vào Redis
        //    await _database.HashSetAsync(redisCartItemKey, cartItemHashEntries);
        //}
        public async Task SaveCartItemIdsToRedis(string userId, List<int> cartItemIds)
        {
            var redisCartKey = $"cart:{userId}:items"; // Key chứa danh sách CartItemIds
            var cartItemIdsJson = JsonConvert.SerializeObject(cartItemIds);
            await _database.StringSetAsync(redisCartKey, cartItemIdsJson);
        }
        private List<CartItemDTO> GetCookieCart(string? userId)
        {
            try
            {
                string savedCart = _httpContextAccessor.HttpContext.Request.Cookies[$"Cart_{userId}"];

                if (!string.IsNullOrEmpty(savedCart))
                {
                    var cart = CartUtil.GetCartFromCookie(savedCart);
                    var cartItems = cart.Values.ToList();
                    return cartItems;
                }
                return new List<CartItemDTO>();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error getting cart: {ex.Message}");
                return new List<CartItemDTO>();
            }
        }
        private async Task<List<CartItemDTO>> SyncCarts(string userId,
        List<CartItemDTO> redisItems,
        List<CartItemDTO> cookieItems)
        {
            var newItems = cookieItems
                .Where(ci => !redisItems.Any(ri => ri.ItemId == ci.ItemId))
                .ToList();

            if (newItems.Any())
            {
               
                var tran = _database.CreateTransaction();

                // Add new items
                tran.SetAddAsync(GetRedisCartKey(userId),
                    newItems.Select(n => (RedisValue)n.ItemId).ToArray());

                // Update metadata
                tran.HashSetAsync(GetRedisMetadataKey(userId), new[] {
            new HashEntry("lastModified", DateTime.UtcNow.Ticks),
            new HashEntry("version", Guid.NewGuid().ToString())
        });

                if (!await tran.ExecuteAsync())
                    throw new Exception("Failed to merge carts");
            }

            return await GetRedisCartItems(userId);
        }

        private string GetRedisCartKey(string userId) => $"cart:{userId}:items";
        private string GetRedisMetadataKey(string userId) => $"cart:{userId}:meta";
        private async Task<List<CartItemDTO>> GetRedisCartItems(string userId)
        {
            
            var items = await _database.SetMembersAsync(GetRedisCartKey(userId));

            return items.Select(i => new CartItemDTO
            {
                ItemId = int.Parse(i),
                // Các field khác nếu cần
            }).ToList();
        }
        //private async Task UpdateCookieCart(int courseId, HttpContext httpContext)
        //{
        //    var cookieCart = GetCookieCart(userId);

        //    // Kiểm tra trùng lặp trong memory
        //    if (cookieCart.Any(ci => ci.ItemId == courseId))
        //    {
        //        return;
        //    }

        //    // Giới hạn số lượng item trong cookie
        //    if (cookieCart.Count >= 20)
        //    {
        //        throw new Exception("Cart limit exceeded");
        //    }

        //    var newItem = new CartItemDTO { ItemId = courseId };
        //    cookieCart.Add(newItem);

        //    // Lưu cookie với HttpOnly và Secure flags
        //    var cookieOptions = new CookieOptions
        //    {
        //        Expires = DateTime.Now.AddDays(7),
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.Lax
        //    };

        //    var cookieValue = CartUtil.ConvertCartToString(cookieCart);
        //    httpContext.Response.Cookies.Append("EdTechCart", cookieValue, cookieOptions);
        //}
        //private async Task MergeCookieToRedis(string userId, string cookieValue)
        //{
            
        //    var cartKey = $"cart:{userId}:items";
        //    var metaKey = $"cart:{userId}:meta";

        //    var cookieItems = CartUtil.ParseCartItems(cookieValue);
        //    var redisItems = await _database.SetMembersAsync(cartKey);

        //    var newItems = cookieItems
        //        .Where(ci => !redisItems.Any(ri => ri.ToString() == ci.ItemId))
        //        .Select(ci => ci.ItemId)
        //        .ToArray();

        //    if (newItems.Length > 0)
        //    {
        //        var tran = _database.CreateTransaction();
        //        tran.SetAddAsync(cartKey, newItems.Select(n => (RedisValue)n).ToArray());
        //        tran.HashSetAsync(metaKey, "lastModified", DateTime.UtcNow.Ticks);

        //        if (await tran.ExecuteAsync())
        //        {
        //            // Xóa cookie sau khi merge thành công
        //            _httpContextAccessor.HttpContext.Response.Cookies.Delete("EdTechCart");
        //        }
        //    }
        //}

        // Helper method để validate key type
        private async Task ValidateKeyType(string key, RedisType expectedType)
        {
            var actualType = await _database.KeyTypeAsync(key);
            if (actualType != RedisType.None && actualType != expectedType)
            {
                _logger.Warning($"Invalid key type for {key}. Expected {expectedType}, found {actualType}");
                throw new Exception();
            }
        }
        #endregion
        public async Task<List<CartItemDTO>> GetCart(string userId)
        {
            try
            {
                var cookieCart = GetCookieCart(userId);
                var cartItems = new List<CartItemDTO>() ;
                if (cookieCart != null && cookieCart.Count > 0)
                {
                    cartItems.AddRange(cookieCart);
                }
                // 2. Xử lý logic merge
                if (!string.IsNullOrEmpty(userId))
                {
                    //await _database.SetAddAsync(GetRedisCartKey(userId), "2");
                    // 2a. Lấy giỏ hàng từ Redis
                    var redisCartItems = await GetRedisCartItems(userId);

                    // 2b. Merge và đồng bộ dữ liệu
                    var mergedItems = await SyncCarts(userId, redisCartItems, cookieCart);

                    // 2c. Xóa cookie sau khi merge thành công
                   // ClearCookieCart(httpContext, CART_COOKIE_NAME);

                    return mergedItems;
                }
                return cookieCart;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while viewing the cart: " + ex.Message);
            }
        }
        

        public async Task<Entity.Cart> CreateSystemCart(string userId)
        {
            try
            {
                var cart = new Entity.Cart
                {
                    UserId = userId,
                    IsActive = true,
                    UpdatedAt = DateTime.UtcNow
                };

                var cartKey = $"Cart:{userId}";
                await _database.StringSetAsync(cartKey, JsonConvert.SerializeObject(cart));

                // Lưu các mặt hàng (CartItems) vào Redis (bạn có thể lưu theo từng khóa như CartItems:{cartId})
                var cartItemsKey = $"CartItems:{userId}";
               // await _database.StringSetAsync(cartItemsKey, JsonConvert.SerializeObject(cart.CartItems));

                return cart;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the Cart: " + ex.Message);
            }
        }



        public async Task<bool> DeleteCartByUsername(string userName)
        {
            string key = $"Cart:{userName}";
            try
            {
                await _redisCacheService.RemoveAsync(key);
                return true;
            }
            catch
            {
                _logger.Error("Error deleting basket for username: {username}", userName);
                return false;
            }
        }

        public async Task<Entity.Cart?> GetCartByUsername(string userName)
        {
            string key = $"Cart:{userName}";
            var basket = await _redisCacheService.GetStringAsync(key);
            return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Entity.Cart>(basket);
        }

        public async Task<Entity.Cart> UpdateCart(Entity.Cart basket, DistributedCacheEntryOptions entryOptions)
        {
            //string key = $"Cart:{basket.UserName}";
            //if (entryOptions != null)
            //{
            //    await _redisCacheService.SetStringAsync(key, _serializeService.Serialize(basket), entryOptions);
            //}
            //else await _redisCacheService.SetStringAsync(key, _serializeService.Serialize(basket));
            //return await GetCartByUsername(basket.UserName);
            throw new NotImplementedException();
        }

        public async Task<CartInformation?> ViewCart(string? userId)
        {
            var cartItems = await GetCart(userId);
            var cartInformation = new CartInformation
            {
                CartItems = new List<Cart.Application.Common.Models.CartItemInformation>()
            };
            decimal totalPrice = 0;
            foreach (var item in cartItems)
            {

                var responseGetCourse = await _requestCourseClient.GetResponse<EventBus.Messages.Events.CartCourseInformation>(new EventBus.Messages.Events.GetCourseRequest { CourseId = item.ItemId });

                var courseInformation = responseGetCourse.Message; 
                var coursePrice = responseGetCourse.Message.Price;

                var responseGetInstructor = await _requestInstructorClient.GetResponse<EventBus.Messages.Events.GetInstructorResponse>(
      new EventBus.Messages.Events.GetInstructorRequest { CourseId = item.ItemId }
  );
                var instructors = responseGetInstructor.Message.Instructors;


                var responseGetDiscount = await _requestDiscountClient.GetResponse<EventBus.Messages.Events.GetDiscountResponse>(
     new EventBus.Messages.Events.GetDiscountRequest { CourseId = item.ItemId });
                var discount = responseGetDiscount.Message.Discount;


                if (discount.DiscountType != "None")
                {
                    discount.CalculateDiscountAndPrice(coursePrice);
                    coursePrice = discount.CalculatedPrice;
                }
                var responseGetCoupon = await _requestCounponClient.GetResponse<EventBus.Messages.Events.GetCouponResponse>(
     new EventBus.Messages.Events.GetCouponRequest { CourseId = item.ItemId, UserId = userId });
                var coupon = responseGetCoupon.Message.Coupon;
                if (coupon.DiscountType != "None")
                {
        coupon.CalculateDiscountAndPrice(coursePrice);
                    coursePrice = coupon.CalculatedPrice;
                }
                var responseGetAvarageReview = await _requestReviewClient.GetResponse<EventBus.Messages.Events.GetReviewResponse>(
    new EventBus.Messages.Events.GetReviewRequest { CourseId = item.ItemId });
                var averageRatingandNumofRate = responseGetAvarageReview.Message.Review;
                totalPrice += coursePrice;
                #region auto mapper
                var cartCourseModel = _mapper.Map<Cart.Application.Common.Models.CartCourseInformation>(courseInformation);
                var discountModel = _mapper.Map<Cart.Application.Common.Models.DiscountInformation>(discount);
                var couponModel = _mapper.Map<Cart.Application.Common.Models.CouponInformation>(coupon); 
                var instructorModel = _mapper.Map<List<Cart.Application.Common.Models.InstructorInformation>>(instructors);
                var averageRatingandNumofRateModel = _mapper.Map<Cart.Application.Common.Models.ReviewInformation>(averageRatingandNumofRate);
                #endregion
                cartInformation.CartItems.Add(
                      new Cart.Application.Common.Models.CartItemInformation
                      {
                          CartCourseInformation = cartCourseModel,
                          InstructorInformation = instructorModel,
                          CouponInformation = couponModel,
                          DiscountInformation = discountModel,
                          CourseReviewInformation = averageRatingandNumofRateModel,
                          TotalCoursePrice = coursePrice
                      }
                  );
            }
            cartInformation.TotalPrice = totalPrice;
            return cartInformation;
        }
        public async Task AddItemToSystemCart(int courseId, string userId)
        {
            var cart = await GetSystemCart(userId);
            // add cart to redis by this template cart:{userId}:items
            await _database.SetAddAsync($"cart:{userId}:items", courseId);
        }
        public async Task<CartInformation> AddToCartV2(string? userId, int courseId)
        {
            //1. get order from orderservice and check if it is already paid
            //2. check if course is already in cart
            var cart = await GetCart(userId);

            //3. save cart to cookie
            //4. addItem to cart redis
            if (userId != null)
            {
                await AddItemToSystemCart(courseId, userId);
            }
            return await ViewCart(userId);
        }

        public Task RemoveFromCart(string? userId, int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
