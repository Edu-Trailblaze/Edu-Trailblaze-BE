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
        private readonly IRequestClient<EventBus.Messages.Events.GetCourseRequest> _requestClient;
        private readonly IPublishEndpoint _publishEndpoint;

        public CartRepository(IDistributedCache distributedCache, ISerializeService serializeService, ILogger logger, IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer connectionMultiplexer, IPublishEndpoint publishEndpoint, IRequestClient<EventBus.Messages.Events.GetCourseRequest> requestClient )
        {
            _redisCacheService = distributedCache;
            _serializeService = serializeService;
            _logger = logger;
            _database = connectionMultiplexer.GetDatabase();
            _httpContextAccessor = httpContextAccessor;
            _publishEndpoint = publishEndpoint;
            _requestClient = requestClient; 
        }
        public async Task AddCartItemToRedis(string userId, CartItemDTO cartItem)
        {
            var cartKey = $"Cart:{userId}"; // Khóa giỏ hàng
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
        public async Task SaveCartItemToRedis(int cartId, int courseId, CartItem cartItem)
        {
            var redisCartItemKey = $"cart_item:{cartId}:{courseId}";
            var cartItemHash = new Dictionary<string, string>
        {
            { "CartId", cartId.ToString() },
            { "CourseId", courseId.ToString() }
        };
            var cartItemHashEntries = cartItemHash.Select(kvp => new HashEntry(kvp.Key, kvp.Value)).ToArray();

            // Lưu mặt hàng vào Redis
            await _database.HashSetAsync(redisCartItemKey, cartItemHashEntries);
        }
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
        //public async Task<CartInformation> AddToCartV2(string? userId, int courseId)
        //{
        //    var cartKey = userId != null ? $"cart:{userId}:items" : null;
        //    var httpContext = _httpContextAccessor.HttpContext;

        //    try
        //    {
        //        // 1. Kiểm tra khóa học đã mua
        //        //var hasBoughtCourse = await _orderRepository.GetDbSet()
        //        //    .Include(o => o.OrderDetails)
        //        //    .AnyAsync(o => o.UserId == userId && o.OrderStatus == "Completed"
        //        //        && o.OrderDetails.Any(od => od.CourseId == courseId));

        //        //if (hasBoughtCourse)
        //        //{
        //        //    throw new Exception("Course has already been bought.");
        //        //}

        //        // 2. Kiểm tra trong Redis (nếu đã login)
        //        if (userId != null)
        //        {
        //            // Validate key type trước khi thao tác
        //            await ValidateKeyType(cartKey, RedisType.Set);

        //            var existsInRedis = await _database.SetContainsAsync(cartKey, courseId);
        //            if (existsInRedis)
        //            {
        //                throw new Exception("Course already exists in the cart.");
        //            }
        //        }

        //        // 3. Kiểm tra trong cookie (cho user chưa login)
        //        var cookieItems = GetCookieCart(userId);
        //        if (cookieItems.Any(ci => ci.ItemId == courseId))
        //        {
        //            throw new Exception("Course already exists in the cart.");
        //        }

        //        // 4. Thêm vào hệ thống
        //        if (userId != null)
        //        {
        //            // Atomic transaction với Redis
        //            var tran = _database.CreateTransaction();

        //            // Thêm vào Redis Set
        //            tran.SetAddAsync(cartKey, courseId);

        //            // Cập nhật metadata
        //            tran.HashSetAsync($"cart:{userId}:meta", new[] {
        //        new HashEntry("lastModified", DateTime.UtcNow.Ticks),
        //        new HashEntry("version", Guid.NewGuid().ToString())
        //    });

        //            if (!await tran.ExecuteAsync())
        //            {
        //                throw new Exception("Failed to add item to Redis cart");
        //            }
        //        }
        //        else
        //        {
        //            // Xử lý cookie với optimistic concurrency
        //            await UpdateCookieCart(courseId, httpContext);
        //        }

        //        // 5. Merge carts nếu cần
        //        if (userId != null && httpContext.Request.Cookies.TryGetValue("EdTechCart", out var cookieValue))
        //        {
        //            await MergeCookieToRedis(userId, cookieValue);
        //        }

        //        return await ViewCart(userId);
        //    }
            //catch (RedisConnectionException ex)
            //{
            //    _logger.LogError(ex, "Redis connection error");

            //    // Fallback to cookie storage
            //    if (userId == null)
            //    {
            //        await UpdateCookieCart(courseId, httpContext);
            //        return await ViewCart(null);
            //    }
            //    throw new CartException("System is busy, please try again later", CartStatus.TemporaryUnavailable);
            //}
            
        //    catch (Exception ex)
        //    {
              
        //        throw new Exception("Failed to add item to cart", ex);
        //    }
        //}


        //public async Task SaveCartToCookie(int courseId, string? userId)
        //{
        //    try
        //    {
        //        Dictionary<int, CartItemDTO> cartItems = new Dictionary<int, CartItemDTO>();
        //        CartItemDTO item = null;

        //        var savedCart = _httpContextAccessor.HttpContext?.Request.Cookies[$"Cart_{userId}"] ?? string.Empty;

        //        if (!string.IsNullOrEmpty(savedCart))
        //        {
        //            cartItems = CartUtil.GetCartFromCookie(savedCart);
        //        }

        //        // Check if the item exists in the cart, add or update accordingly
        //        if (cartItems.TryGetValue(courseId, out item))
        //        {
        //            //throw new Exception("Course already exists in the cart.");
        //            return;
        //        }

        //        item = new CartItemDTO
        //        {
        //            ItemId = courseId,
        //        };
        //        cartItems[courseId] = item;

        //        // Convert the updated cart to string and save it back to the cookie
        //        var strItemsInCart = CartUtil.ConvertCartToString(cartItems.Values.ToList());
        //        CartUtil.SaveCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, strItemsInCart, userId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while saving the cart to cookie: " + ex.Message);
        //    }
        //}

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
                CartItems = new List<CartItemInformation>()
            };
            decimal totalPrice = 0;
            foreach (var item in cartItems)
            {

                var response = await _requestClient.GetResponse<EventBus.Messages.Events.CartCourseInformation>(new EventBus.Messages.Events.GetCourseRequest { CourseId = item.ItemId });
                var course = response.Message;
               
            }
                return cartInformation;
        }
    }
}
