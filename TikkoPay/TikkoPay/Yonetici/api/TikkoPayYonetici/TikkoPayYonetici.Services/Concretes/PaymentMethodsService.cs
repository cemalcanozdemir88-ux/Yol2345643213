using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.PaymentMethods;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class PaymentMethodsService : IPaymentMethodsService
    {
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private readonly IPaymentMethodsRepository _paymentMethodsRepository;

        public PaymentMethodsService(IMapper mapper,
                                     IUnitOfWork unitOfWork, 
                                     IPaymentMethodsRepository paymentMethodsRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _paymentMethodsRepository = paymentMethodsRepository;
        }

        public async Task<ResponseViewModel<List<PaymentMethodsDto>>> GetAllDtoAsync()
        {
            var list = await _paymentMethodsRepository.GetAllDtoAsync();

            return new ResponseViewModel<List<PaymentMethodsDto>>()
            {
                Success = true,
                Result = list.ToList()
            };
        }
    }
}
