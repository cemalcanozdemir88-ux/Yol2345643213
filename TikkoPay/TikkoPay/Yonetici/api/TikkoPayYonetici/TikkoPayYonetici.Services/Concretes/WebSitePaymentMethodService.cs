using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSitePaymentMethods;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
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
