using AutoMapper;
using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.WebSitePaymentMethods;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
{
    public class WebSitePaymentMethodService : IWebSitePaymentMethodService
    {
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;

        public WebSitePaymentMethodService(IMapper mapper, 
                                           IUnitOfWork unitOfWork, 
                                           IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
        }

        public async Task<ResponseViewModel<List<WebSitePaymentMethodsDto>>> GetByWebSiteIdDtoAsync(Guid webSiteId)
        {
            var list = await _webSitePaymentMethodsRepository.GetByWebSiteIdDtoAsync(webSiteId);

            return new ResponseViewModel<List<WebSitePaymentMethodsDto>>()
            {
                Success = true,
                Result = list.ToList()
            };
        }
    }
}
