using AutoMapper;
using Cart.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Features.V1.Carts.Queries.GetCarts
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, ApiResult<List<Cart.Domain.Entities.Cart>>>
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
        public async Task<ApiResult<Domain.Entities.Cart>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Begin {MethodName}");
            var orders = await _cartRepository.GetCartByUsername(request.UserName);
            return new ApiSuccessResult<Domain.Entities.Cart>(orders);
        }

        Task<ApiResult<List<Domain.Entities.Cart>>> IRequestHandler<GetCartQuery, ApiResult<List<Domain.Entities.Cart>>>.Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
