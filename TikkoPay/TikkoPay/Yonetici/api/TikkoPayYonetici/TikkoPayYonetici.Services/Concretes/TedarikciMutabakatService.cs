using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.TedarikciMutabakatlari;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class TedarikciMutabakatService : ITedarikciMutabakatService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITedarikciMutabakatRepository _tedarikciMutabakatRepository;
        private readonly ITedarikciRepository _tedarikciRepository;
        private readonly IYatirimTalepleriRepository _yatirimTalepleriRepository;
        private readonly IKasaHaraketleriRepository _kasaHaraketleriRepository;

        public TedarikciMutabakatService(IMapper mapper,
                                         IUnitOfWork unitOfWork,
                                         ITedarikciMutabakatRepository tedarikciMutabakatRepository,
                                         ITedarikciRepository tedarikciRepository,
                                         IYatirimTalepleriRepository yatirimTalepleriRepository,
                                         IKasaHaraketleriRepository kasaHaraketleriRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tedarikciMutabakatRepository = tedarikciMutabakatRepository;
            _tedarikciRepository = tedarikciRepository;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
            _kasaHaraketleriRepository = kasaHaraketleriRepository;
        }

        public async Task<ResponseViewModel<List<TedarikciMutabakatlariListDto>>> GetAllDtoAsync(Guid? tedarikciId, DateOnly mutabakatTarihi)
        {
            var mutabakatList = await _tedarikciMutabakatRepository.GetAllDtoAsync(tedarikciId, mutabakatTarihi);

            return new ResponseViewModel<List<TedarikciMutabakatlariListDto>>
            {
                Success = true,
                Result = mutabakatList.ToList()
            };
        }

        public async Task<ResponseViewModel<TedarikciMutabakatlariListDto>> Calculate(Guid tedarikciId, decimal devir, DateOnly mutabakatTarihi)
        {
            devir = 0;
            var mutabakatTarihiPrevious = mutabakatTarihi.AddDays(-1);
            
            var previousMutabakat = await _tedarikciMutabakatRepository.GetAllDtoAsync(tedarikciId, mutabakatTarihiPrevious);
            if (previousMutabakat.Any())
            {
                devir = previousMutabakat.First().Kasa;
            }

            var bakiyeModel = GetBakiyeModel(tedarikciId, mutabakatTarihi);

            var yatirimKomisyon = bakiyeModel.ToplamYatirim - bakiyeModel.ToplamYatirimKomisyon;
            var cekimKomisyon = bakiyeModel.ToplamCekim + bakiyeModel.ToplamCekimKomisyon;
            var kasaHaraketiGiris = bakiyeModel.ToplamGiris + bakiyeModel.ToplamGirisKomisyon + bakiyeModel.ToplamGirisManuel + bakiyeModel.ToplamVirmanCikis;
            var kasaHaraketiCikis = bakiyeModel.ToplamCikis + bakiyeModel.ToplamCikisManuel + bakiyeModel.ToplamVirmanGiris;

            var kasa = devir + ((yatirimKomisyon + kasaHaraketiCikis) - (cekimKomisyon + kasaHaraketiGiris));

            // Eski kayıt(lar)ı silip yenisini ekleme işlemi tek transaction içinde yapılır;
            // aynı gün için asla iki kayıt oluşmaz.
            var kaydedildi = await _tedarikciMutabakatRepository.ReplaceForDateAsync(new TedarikciMutabakatlari
            {
                Id = Guid.NewGuid(),
                TedarikciId = tedarikciId,
                Devir = devir,
                ToplamYatirim = bakiyeModel.ToplamYatirim,
                ToplamYatirimKomisyon = bakiyeModel.ToplamYatirimKomisyon,
                ToplamCekim = bakiyeModel.ToplamCekim,
                ToplamCekimKomisyon = bakiyeModel.ToplamCekimKomisyon,
                ToplamGiris = bakiyeModel.ToplamGiris,
                ToplamGirisKomisyon = bakiyeModel.ToplamGirisKomisyon,
                ToplamCikis = bakiyeModel.ToplamCikis,
                ToplamGirisManual = bakiyeModel.ToplamGirisManuel,
                ToplamCikisManual = bakiyeModel.ToplamCikisManuel,
                ToplamVirmanGiris = bakiyeModel.ToplamVirmanGiris,
                ToplamVirmanCikis = bakiyeModel.ToplamVirmanCikis,
                Kasa = kasa,
                MutabakatTarihi = mutabakatTarihi
            });

            if (!kaydedildi)
            {
                return new ResponseViewModel<TedarikciMutabakatlariListDto>
                {
                    Success = false,
                    Message = "Bu gün için aynı anda başka bir mutabakat hesaplaması yapıldı, mevcut kayıt korundu. Gerekirse tekrar hesaplayınız."
                };
            }

            return new ResponseViewModel<TedarikciMutabakatlariListDto>
            {
                Success = true
            };
        }

        public decimal GetDevir(Guid tedarikciId, DateOnly mutabakatTarihi)
        {
            var bakiyeModel = GetBakiyeModel(tedarikciId, mutabakatTarihi.AddDays(-1));

            var yatirimKomisyon = bakiyeModel.ToplamYatirim - bakiyeModel.ToplamYatirimKomisyon;
            var cekimKomisyon = bakiyeModel.ToplamCekim + bakiyeModel.ToplamCekimKomisyon;
            var kasaHaraketiGiris = bakiyeModel.ToplamGiris + bakiyeModel.ToplamGirisKomisyon + bakiyeModel.ToplamGirisManuel + bakiyeModel.ToplamVirmanCikis;
            var kasaHaraketiCikis = bakiyeModel.ToplamCikis + bakiyeModel.ToplamCikisManuel + bakiyeModel.ToplamVirmanGiris;

            var kasa = ((yatirimKomisyon + kasaHaraketiCikis) - (cekimKomisyon + kasaHaraketiGiris));

            return kasa;
        }

        private TedarikciMutabakatBakiyeModel GetBakiyeModel(Guid tedarikciId, DateOnly mutabakatTarihi)
        {
            var start = mutabakatTarihi.ToDateTime(TimeOnly.MinValue);
            var end = mutabakatTarihi.AddDays(1).ToDateTime(TimeOnly.MinValue); 

            var toplamYatirim = _yatirimTalepleriRepository
                                .Where(y => y.OnayTarihi >= start && y.OnayTarihi < end &&
                                            y.TedarikciId == tedarikciId &&
                                            y.Durum == Core.Enums.Durum.Onaylandi &&
                                            y.IslemTipi == Core.Enums.IslemTip.Deposit &&
                                            y.SilindiMi == false)
                                .Sum(k => k.Tutar);

            var toplamYatirimKomisyon = _yatirimTalepleriRepository
                        .Where(y =>
                        y.OnayTarihi >= start && y.OnayTarihi < end &&
                        y.TedarikciId == tedarikciId &&
                                    y.Durum == Core.Enums.Durum.Onaylandi &&
                                    y.IslemTipi == Core.Enums.IslemTip.Deposit &&
                                    y.SilindiMi == false)
                        .Sum(k => k.TedarikciKomisyon);

            var toplamCekim = _yatirimTalepleriRepository
                        .Where(y =>
                         y.OnayTarihi >= start && y.OnayTarihi < end &&
                        y.TedarikciId == tedarikciId &&
                                    y.Durum == Core.Enums.Durum.Onaylandi &&
                                    y.IslemTipi == Core.Enums.IslemTip.Withdraw &&
                                    y.SilindiMi == false)
                        .Sum(k => k.Tutar);

            var toplamCekimKomisyon = _yatirimTalepleriRepository
                       .Where(y =>
                        y.OnayTarihi >= start && y.OnayTarihi < end &&
                       y.TedarikciId == tedarikciId &&
                                   y.Durum == Core.Enums.Durum.Onaylandi &&
                                   y.IslemTipi == Core.Enums.IslemTip.Withdraw &&
                                   y.SilindiMi == false)
                       .Sum(k => k.TedarikciKomisyon);

            var toplamGiris = _kasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.TedarikciId == tedarikciId &&
                                   y.Durum == Core.Enums.Durum.Onaylandi &&
                                   y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Giris &&
                                   y.TopId == null &&
                                   y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamGirisKomisyon = _kasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.TedarikciId == tedarikciId &&
                                   y.Durum == Core.Enums.Durum.Onaylandi &&
                                   y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Giris &&
                                   y.TopId != null &&
                                   y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamCikis = _kasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.TedarikciId == tedarikciId &&
                                          y.Durum == Core.Enums.Durum.Onaylandi &&
                                        y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Cikis &&
                                        y.TopId == null &&
                                          y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamGirisManuel = _kasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.TedarikciId == tedarikciId &&
                                          y.Durum == Core.Enums.Durum.Onaylandi &&
                                        y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.GirisManuel &&
                                        y.TopId == null &&
                                          y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamCikisManuel = _kasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.TedarikciId == tedarikciId &&
                                          y.Durum == Core.Enums.Durum.Onaylandi &&
                                        y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.CikisManuel &&
                                        y.TopId == null &&
                                          y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamVirmanGiris = _kasaHaraketleriRepository
                                   .Where(y =>
                                   y.KayitTarihi >= start && y.KayitTarihi < end &&
                                   y.TedarikciId == tedarikciId &&
                                          y.Durum == Core.Enums.Durum.Onaylandi &&
                                        y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.VirmanGiris &&
                                        y.TopId == null &&
                                          y.SilindiMi == false)
                                   .Sum(k => k.Tutar);

            var toplamVirmanCikis = _kasaHaraketleriRepository
                                  .Where(y =>
                                  y.KayitTarihi >= start && y.KayitTarihi < end &&
                                  y.TedarikciId == tedarikciId &&
                                         y.Durum == Core.Enums.Durum.Onaylandi &&
                                       y.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.VirmanCikis &&
                                       y.TopId == null &&
                                         y.SilindiMi == false)
                                  .Sum(k => k.Tutar);

            return new TedarikciMutabakatBakiyeModel
            {
                ToplamYatirim = toplamYatirim,
                ToplamYatirimKomisyon = toplamYatirimKomisyon,
                ToplamCekim = toplamCekim,
                ToplamCekimKomisyon = toplamCekimKomisyon,
                ToplamGiris = toplamGiris,
                ToplamGirisKomisyon = toplamGirisKomisyon,
                ToplamCikis = toplamCikis,
                ToplamGirisManuel = toplamGirisManuel,
                ToplamCikisManuel = toplamCikisManuel,
                ToplamVirmanGiris = toplamVirmanGiris,
                ToplamVirmanCikis = toplamVirmanCikis
            };
        }
    }
}
