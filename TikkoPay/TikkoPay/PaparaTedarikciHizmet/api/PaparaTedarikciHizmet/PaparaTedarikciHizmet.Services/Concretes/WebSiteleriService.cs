using AutoMapper;
using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.WebSiteleri;
using PaparaTedarikciHizmet.Core.Interfaces;
using PaparaTedarikciHizmet.Services.Interfaces;

namespace PaparaTedarikciHizmet.Services.Concretes
{
    public class WebSiteleriService : IWebSiteleriService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebSiteleriRepository _webSiteleriRepository;

        public WebSiteleriService(IMapper mapper, IUnitOfWork unitOfWork, IWebSiteleriRepository webSiteleriRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webSiteleriRepository = webSiteleriRepository;
        }

        public ResponseViewModel<WebSiteleriDto> GetById(Guid id)
        {
           var item = _webSiteleriRepository.Where(k => k.Id == id && k.SilindiMi == false).FirstOrDefault();
            if (item == null)
            {
                return new ResponseViewModel<WebSiteleriDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                var result = _mapper.Map<WebSiteleriDto>(item);

                return new ResponseViewModel<WebSiteleriDto>
                {
                    Success = true,
                    Result = result
                };
            }
        }

        public ResponseViewModel<List<WebSiteleriListDto>> GetAll(Guid? webSiteId)
        {
            var items = _webSiteleriRepository.Find(k => (k.Id == webSiteId || webSiteId == null) && k.SilindiMi == false).OrderBy(k => k.WebSiteUrl).ToList();
            return new ResponseViewModel<List<WebSiteleriListDto>>
            {
                Success = true,
                Result = _mapper.Map<List<WebSiteleriListDto>>(items)
            };
        }

        public async Task<ResponseViewModel<List<WebSiteBakiyeListDto>>> GetBakiyeList(Guid? webSiteId)
        {
            var bakiyeList = await _webSiteleriRepository.GetBakiyeList(webSiteId);
            return new ResponseViewModel<List<WebSiteBakiyeListDto>>
            {
                Success = true,
                Result = bakiyeList
            };
        }
    }
}
