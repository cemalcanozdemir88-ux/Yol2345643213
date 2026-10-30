using AutoMapper;
using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.PaymentMethods;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
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
