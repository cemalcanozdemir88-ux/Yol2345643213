using AutoMapper;
using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using ParaYolYonetici.Core.Enums;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
{
    public class WebSiteKasaHaraketleriService : IWebSiteKasaHaraketleriService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebSiteKasaHaraketleriRepository _webSiteKasaHaraketleriRepository;
        private readonly IWebSiteleriRepository _webSiteleriRepository;

        public WebSiteKasaHaraketleriService(IMapper mapper, 
                                             IUnitOfWork unitOfWork, 
                                             IWebSiteKasaHaraketleriRepository webSiteKasaHaraketleriRepository, 
                                             IWebSiteleriRepository webSiteleriRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webSiteKasaHaraketleriRepository = webSiteKasaHaraketleriRepository;
            _webSiteleriRepository = webSiteleriRepository;
        }

        public async Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> GetById(Guid id)
        {
            var item = await _webSiteKasaHaraketleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<WebSiteKasaHaraketleriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModelPagination<List<WebSiteKasaHaraketleriListDto>>> GetAll(Guid? webSiteId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _webSiteKasaHaraketleriRepository.GetCountAsync(webSiteId, durum, islemTipi, createdDateStart, createdDateEnd);
            var items = await _webSiteKasaHaraketleriRepository.GetAllDtoAsync(webSiteId, durum, islemTipi, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<WebSiteKasaHaraketleriListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> CreateVirman(WebSiteKasaHaraketleriVirmanCreateDto item)
        {
            if (item.FromWebSitesiId != Guid.Empty)
            {
                WebSiteKasaHaraketleri from = new WebSiteKasaHaraketleri();
                from.WebSitesiId = item.FromWebSitesiId;
                from.Aciklama = item.Aciklama;
                from.Tutar = item.Tutar;
                from.DovizKuru = item.DovizKuru;
                from.UsdtMiktar = item.UsdtMiktar;
                from.UsdtAdres = item.UsdtAdres;
                from.UsdtTxtNo = item.UsdtTxtNo;
                from.ResimUrl = item.ResimUrl;
                from.IslemTipi = item.FromVirmanIslemTipi == 1 ? KasaHaraketleriIslemTipi.VirmanGiris : KasaHaraketleriIslemTipi.VirmanCikis;
                from.Durum = Durum.Onaylandi;

                await _webSiteKasaHaraketleriRepository.AddAsync(from);
            }

            if (item.ToWebSitesiId != Guid.Empty)
            {
                WebSiteKasaHaraketleri to = new WebSiteKasaHaraketleri();
                to.WebSitesiId = item.ToWebSitesiId;
                to.Aciklama = item.Aciklama;
                to.Tutar = item.Tutar;
                to.DovizKuru = item.DovizKuru;
                to.UsdtMiktar = item.UsdtMiktar;
                to.UsdtAdres = item.UsdtAdres;
                to.UsdtTxtNo = item.UsdtTxtNo;
                to.ResimUrl = item.ResimUrl;
                to.IslemTipi = item.ToVirmanIslemTipi == 1 ? KasaHaraketleriIslemTipi.VirmanGiris : KasaHaraketleriIslemTipi.VirmanCikis;
                to.Durum = Durum.Onaylandi;

                await _webSiteKasaHaraketleriRepository.AddAsync(to);
            }

            var result = await _unitOfWork.SaveAsync();
            if (result > 0)
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Create(WebSiteKasaHaraketleriCreateDto item)
        {
            WebSiteKasaHaraketleri webSiteKasaHaraketleri = _mapper.Map<WebSiteKasaHaraketleri>(item);
            await _webSiteKasaHaraketleriRepository.AddAsync(webSiteKasaHaraketleri);

            var result = await _unitOfWork.SaveAsync();

            await AddSubItem(webSiteKasaHaraketleri);

            if (result > 0)
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        private async Task AddSubItem(WebSiteKasaHaraketleri webSiteKasaHaraketleri)
        {
            await _webSiteKasaHaraketleriRepository.DeleteByTopId(webSiteKasaHaraketleri.Id);

            if (webSiteKasaHaraketleri.IslemTipi == KasaHaraketleriIslemTipi.Cikis)
            {
                var webSiteItem = await _webSiteleriRepository.GetByIdAsync(webSiteKasaHaraketleri.WebSitesiId);
                if (webSiteItem is not null && webSiteItem.OdemeKomisyonOrani > 0)
                {
                    var subItem = new WebSiteKasaHaraketleri();
                    subItem.TopId = webSiteKasaHaraketleri.Id;
                    subItem.WebSitesiId = webSiteKasaHaraketleri.WebSitesiId;
                    subItem.Aciklama = webSiteKasaHaraketleri.Aciklama;
                    subItem.Tutar = webSiteKasaHaraketleri.Tutar * (decimal)webSiteItem.OdemeKomisyonOrani / 100m;
                    subItem.DovizKuru = webSiteKasaHaraketleri.DovizKuru;
                    subItem.UsdtMiktar = webSiteKasaHaraketleri.UsdtMiktar * (decimal)webSiteItem.OdemeKomisyonOrani / 100m;
                    subItem.UsdtAdres = webSiteKasaHaraketleri.UsdtAdres;
                    subItem.UsdtTxtNo = webSiteKasaHaraketleri.UsdtTxtNo;
                    subItem.ResimUrl = webSiteKasaHaraketleri.ResimUrl;
                    subItem.IslemTipi = webSiteKasaHaraketleri.IslemTipi;
                    subItem.Durum = webSiteKasaHaraketleri.Durum;

                    await _webSiteKasaHaraketleriRepository.AddAsync(subItem);
                }
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Update(WebSiteKasaHaraketleriUpdateDto item)
        {
            var dbItem = await _webSiteKasaHaraketleriRepository.GetByIdAsync(item.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _webSiteKasaHaraketleriRepository.Update(dbItem, item);
            var result = await _unitOfWork.SaveAsync();

            await AddSubItem(dbItem);

            return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Delete(Guid id)
        {
            var item = await _webSiteKasaHaraketleriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _webSiteKasaHaraketleriRepository.DeleteByTopId(item.Id);

            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

    }
}
