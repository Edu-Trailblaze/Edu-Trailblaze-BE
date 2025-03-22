using AutoMapper;
using Cart.Application.Common.Interfaces;
using Cart.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SeedWork;


namespace Cart.Application.Features.V1.Carts.Queries.GetCarts
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, List<CartItemDTO>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCartQueryHandler> _logger;
        public GetCartQueryHandler(ICartRepository cartRepository, IMapper mapper, ILogger<GetCartQueryHandler> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
        }
        private const string MethodName = "GetCartQueryHandler";
        public async Task<List<CartItemDTO>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Begin {MethodName}");
            var orders = await _cartRepository.GetCart(request.UserId);
            return new List<CartItemDTO>(orders);
        }


    }
}
