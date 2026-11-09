using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.WebSiteMutabakatlari;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class WebSiteMutabakatService : IWebSiteMutabakatService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebSiteMutabakatRepository _webSiteMutabakatRepository;
        private readonly IWebSiteleriRepository _webSiteleriRepository;
        private readonly IYatirimTalepleriRepository _yatirimTalepleriRepository;
        private readonly IWebSiteKasaHaraketleriRepository _webSiteKasaHaraketleriRepository;

        public WebSiteMutabakatService(IMapper mapper, 
                                       IUnitOfWork unitOfWork, 
                                       IWebSiteMutabakatRepository webSiteMutabakatRepository, 
                                       IWebSiteleriRepository webSiteleriRepository, 
                                       IYatirimTalepleriRepository yatirimTalepleriRepository, 
                                       IWebSiteKasaHaraketleriRepository webSiteKasaHaraketleriRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webSiteMutabakatRepository = webSiteMutabakatRepository;
            _webSiteleriRepository = webSiteleriRepository;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
            _webSiteKasaHaraketleriRepository = webSiteKasaHaraketleriRepository;
        }

        public async Task<ResponseViewModel<List<WebSiteMutabakatlariListDto>>> GetAllDtoAsync(Guid? webSiteId, DateOnly mutabakatTarihi)
        {
            var mutabakatList = await _webSiteMutabakatRepository.GetAllDtoAsync(webSiteId, mutabakatTarihi);

            return new ResponseViewModel<List<WebSiteMutabakatlariListDto>>
            {
                Success = true,
                Result = mutabakatList.ToList()
            };
        }

        public async Task<ResponseViewModel<WebSiteMutabakatlariListDto>> Calculate(Guid webSiteId, decimal devir, DateOnly mutabakatTarihi)
        {
            devir = 0;
            var mutabakatTarihiPrevious = mutabakatTarihi.AddDays(-1);
            
            var previousMutabakat = await _webSiteMutabakatRepository.GetAllDtoAsync(webSiteId, mutabakatTarihiPrevious);
            if (previousMutabakat.Any())
            {
                devir = previousMutabakat.First().Kasa;
            }

            var bakiyeModel = GetBakiyeModel(webSiteId, mutabakatTarihi);

            var yatirim = bakiyeModel.ToplamYatirim - bakiyeModel.ToplamYatirimKomisyon;
            var cekim = bakiyeModel.ToplamCekim + bakiyeModel.ToplamCekimKomisyon;
            var kasaHaraketiGiris = bakiyeModel.ToplamGiris + + bakiyeModel.ToplamGirisManuel + bakiyeModel.ToplamVirmanGiris;
            var kasaHaraketiCikis = bakiyeModel.ToplamCikis + bakiyeModel.ToplamCikisKomisyon + bakiyeModel.ToplamCikisManuel + bakiyeModel.ToplamVirmanCikis;

            var kasa = devir + (yatirim - cekim + kasaHaraketiGiris - kasaHaraketiCikis);

            // Eski kayıt(lar)ı silip yenisini ekleme işlemi tek transaction içinde yapılır;
            // aynı gün için asla iki kayıt oluşmaz.
            var kaydedildi = await _webSiteMutabakatRepository.ReplaceForDateAsync(new WebSiteMutabakatlari
            {
                Id = Guid.NewGuid(),
                WebSiteId = webSiteId,
                Devir = devir,
                ToplamYatirim = bakiyeModel.ToplamYatirim,
                ToplamYatirimKomisyon = bakiyeModel.ToplamYatirimKomisyon,
                ToplamCekim = bakiyeModel.ToplamCekim,
                ToplamCekimKomisyon = bakiyeModel.ToplamCekimKomisyon,
                ToplamGiris = bakiyeModel.ToplamGiris,
                ToplamCikis = bakiyeModel.ToplamCikis,
                ToplamCikisKomisyon = bakiyeModel.ToplamCikisKomisyon,
                ToplamGirisManual = bakiyeModel.ToplamGirisManuel,
                ToplamCikisManual = bakiyeModel.ToplamCikisManuel,
                ToplamVirmanGiris = bakiyeModel.ToplamVirmanGiris,
                ToplamVirmanCikis = bakiyeModel.ToplamVirmanCikis,
                Kasa = kasa,
                MutabakatTarihi = mutabakatTarihi
            });

            if (!kaydedildi)
            {
                return new ResponseViewModel<WebSiteMutabakatlariListDto>
                {
                    Success = false,
                    Message = "Bu gün için aynı anda başka bir mutabakat hesaplaması yapıldı, mevcut kayıt korundu. Gerekirse tekrar hesaplayınız."
                };
            }

            return new ResponseViewModel<WebSiteMutabakatlariListDto>
            {
                Success = true
            };
        }

        private WebSiteMutabakatBakiyeModel GetBakiyeModel(Guid webSiteId, DateOnly mutabakatTarihi)
        {
            var start = mutabakatTarihi.ToDateTime(TimeOnly.MinValue);
            var end = mutabakatTarihi.AddDays(1).ToDateTime(TimeOnly.MinValue);

            var toplamYatirim = _yatirimTalepleriRepository
                                .Where(y => y.OnayTarihi >= start && y.OnayTarihi < end &&
                                            y.WebSiteId == webSiteId &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Deposit &&
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar);

            var toplamYatirimKomisyon = _yatirimTalepleriRepository
                        .Where(y =>
                        y.OnayTarihi >= start && y.OnayTarihi < end &&
                        y.WebSiteId == webSiteId &&
                        y.Durum == Core.Enums.Durum.Onaylandi &&
                        y.IslemTipi == Core.Enums.IslemTip.Deposit &&
                        y.SilindiMi == false)
                        .Sum(k => k.WebSiteKomisyon);

            var toplamCekim = _yatirimTalepleriRepository
                        .Where(y =>
                         y.TopId == null &&
                         y.OnayTarihi >= start && y.OnayTarihi < end &&
                         y.WebSiteId == webSiteId &&
                         y.Durum == Core.Enums.Durum.Onaylandi &&
                         y.IslemTipi == Core.Enums.IslemTip.Withdraw &&
                         y.SilindiMi == false)
                        .Sum(k => k.Tutar);

            var toplamCekimKomisyon = _yatirimTalepleriRepository
                       .Where(y =>
                        y.TopId == null &&
                        y.OnayTarihi >= start && y.OnayTarihi < end &&
                        y.WebSiteId == webSiteId &&
                        y.Durum == Core.Enums.Durum.Onaylandi &&
                        y.IslemTipi == Core.Enums.IslemTip.Withdraw &&
                        y.SilindiMi == false)
                       .Sum(k => k.WebSiteKomisyon);

            var toplamGiris = _webSiteKasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.WebSitesiId == webSiteId &&
                                   y.Durum == Core.Enums.Durum.Onaylandi &&
                                   y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Giris &&
                                   y.TopId == null &&
                                   y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamCikis = _webSiteKasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.WebSitesiId == webSiteId &&
                                   y.Durum == Core.Enums.Durum.Onaylandi &&
                                   y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Cikis &&
                                   y.TopId == null &&
                                   y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamCikisKomisyon = _webSiteKasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.WebSitesiId == webSiteId &&
                                   y.Durum == Core.Enums.Durum.Onaylandi &&
                                   y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Cikis &&
                                   y.TopId != null &&
                                   y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamGirisManuel = _webSiteKasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.WebSitesiId == webSiteId &&
                                   y.Durum == Core.Enums.Durum.Onaylandi &&
                                   y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.GirisManuel &&
                                   y.TopId == null &&
                                   y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamCikisManuel = _webSiteKasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.WebSitesiId == webSiteId &&
                                   y.Durum == Core.Enums.Durum.Onaylandi &&
                                   y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.CikisManuel &&
                                   y.TopId == null &&
                                   y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamVirmanGiris = _webSiteKasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.WebSitesiId == webSiteId &&
                                   y.Durum == Core.Enums.Durum.Onaylandi &&
                                   y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.VirmanGiris &&
                                   y.TopId == null &&
                                   y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamVirmanCikis = _webSiteKasaHaraketleriRepository
                                  .Where(y =>
                                  y.KayitTarihi >= start && y.KayitTarihi < end &&
                                  y.WebSitesiId == webSiteId &&
                                  y.Durum == Core.Enums.Durum.Onaylandi &&
                                  y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.VirmanCikis &&
                                  y.TopId == null &&
                                  y.SilindiMi == false)
                                  .Sum(k => k.Tutar);

            return new WebSiteMutabakatBakiyeModel
            {
                ToplamYatirim = toplamYatirim,
                ToplamYatirimKomisyon = toplamYatirimKomisyon,
                ToplamCekim = toplamCekim,
                ToplamCekimKomisyon = toplamCekimKomisyon,
                ToplamGiris = toplamGiris,
                ToplamCikis = toplamCikis,
                ToplamCikisKomisyon = toplamCikisKomisyon,
                ToplamGirisManuel = toplamGirisManuel,
                ToplamCikisManuel = toplamCikisManuel,
                ToplamVirmanGiris = toplamVirmanGiris,
                ToplamVirmanCikis = toplamVirmanCikis
            };
        }
    }
}
