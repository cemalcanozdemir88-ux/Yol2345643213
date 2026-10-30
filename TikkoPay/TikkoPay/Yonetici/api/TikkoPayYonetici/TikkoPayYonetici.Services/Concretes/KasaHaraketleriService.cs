using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.KasaHaraketleri;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class KasaHaraketleriService : IKasaHaraketleriService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKasaHaraketleriRepository _kasaHaraketleriRepository;
        private readonly ITedarikciRepository _tedarikciRepository;

        public KasaHaraketleriService(IMapper mapper,
                                      IUnitOfWork unitOfWork,
                                      IKasaHaraketleriRepository kasaHaraketleriRepository,
                                      ITedarikciRepository tedarikciRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _kasaHaraketleriRepository = kasaHaraketleriRepository;
            _tedarikciRepository = tedarikciRepository;
        }

        public async Task<ResponseViewModel<KasaHaraketleriListDto>> GetById(Guid id)
        {
            var item = await _kasaHaraketleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<KasaHaraketleriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _kasaHaraketleriRepository.GetAllDtoAsync(tedarikciId, durum, islemTipi, createdDateStart, createdDateEnd);

            return new ResponseViewModel<List<KasaHaraketleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, int? islemTipi, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _kasaHaraketleriRepository.GetCountAsync(tedarikciId, durum, islemTipi, createdDateStart, createdDateEnd);
            var items = await _kasaHaraketleriRepository.GetAllDtoAsync(tedarikciId, durum, islemTipi, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<KasaHaraketleriListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<KasaHaraketleriListDto>> CreateVirman(KasaHaraketleriVirmanCreateDto item)
        {
            if (item.FromTedarikciId != Guid.Empty)
            {
                KasaHaraketleri from = new KasaHaraketleri();
                from.TedarikciId = item.FromTedarikciId;
                from.Aciklama = item.Aciklama;
                from.Tutar = item.Tutar;
                from.DovizKuru = item.DovizKuru;
                from.UsdtMiktar = item.UsdtMiktar;
                from.UsdtAdres = item.UsdtAdres;
                from.UsdtTxtNo = item.UsdtTxtNo;
                from.ResimUrl = item.ResimUrl;
                from.IslemTipi = item.FromVirmanIslemTipi == 1 ? KasaHaraketleriIslemTipi.VirmanGiris : KasaHaraketleriIslemTipi.VirmanCikis;
                from.Durum = Durum.Onaylandi;

                await _kasaHaraketleriRepository.AddAsync(from);
            }

            if (item.ToTedarikciId != Guid.Empty)
            {
                KasaHaraketleri to = new KasaHaraketleri();
                to.TedarikciId = item.ToTedarikciId;
                to.Aciklama = item.Aciklama;
                to.Tutar = item.Tutar;
                to.DovizKuru = item.DovizKuru;
                to.UsdtMiktar = item.UsdtMiktar;
                to.UsdtAdres = item.UsdtAdres;
                to.UsdtTxtNo = item.UsdtTxtNo;
                to.ResimUrl = item.ResimUrl;
                to.IslemTipi = item.ToVirmanIslemTipi == 1 ? KasaHaraketleriIslemTipi.VirmanGiris : KasaHaraketleriIslemTipi.VirmanCikis;
                to.Durum = Durum.Onaylandi;

                await _kasaHaraketleriRepository.AddAsync(to);
            }

            var result = await _unitOfWork.SaveAsync();
            if (result > 0)
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<KasaHaraketleriListDto>> Create(KasaHaraketleriCreateDto item)
        {
            KasaHaraketleri kasaHaraketleri = _mapper.Map<KasaHaraketleri>(item);
            await _kasaHaraketleriRepository.AddAsync(kasaHaraketleri);
            var result = await _unitOfWork.SaveAsync();

            await AddSubItem(kasaHaraketleri);

            if (result > 0)
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        private async Task AddSubItem(KasaHaraketleri kasaHaraketleri)
        {
            await _kasaHaraketleriRepository.DeleteByTopId(kasaHaraketleri.Id);

            if (kasaHaraketleri.IslemTipi == Core.Enums.KasaHaraketleriIslemTipi.Giris)
            {
                var tedarikciItem = await _tedarikciRepository.GetByIdAsync(kasaHaraketleri.TedarikciId);
                if (tedarikciItem is not null && tedarikciItem.TedarikcidenAlinanOdemeKomisyonOrani > 0)
                {
                    var subItem = new KasaHaraketleri
                    {
                        TopId = kasaHaraketleri.Id,
                        TedarikciId = kasaHaraketleri.TedarikciId,
                        Aciklama = kasaHaraketleri.Aciklama,
                        Tutar = kasaHaraketleri.Tutar * (decimal)tedarikciItem.TedarikcidenAlinanOdemeKomisyonOrani / 100m,
                        DovizKuru = kasaHaraketleri.DovizKuru,
                        UsdtMiktar = kasaHaraketleri.UsdtMiktar * (decimal)tedarikciItem.TedarikcidenAlinanOdemeKomisyonOrani / 100m,
                        UsdtAdres = kasaHaraketleri.UsdtAdres,
                        UsdtTxtNo = kasaHaraketleri.UsdtTxtNo,
                        ResimUrl = kasaHaraketleri.ResimUrl,
                        IslemTipi = kasaHaraketleri.IslemTipi,
                        Durum = kasaHaraketleri.Durum
                    };

                    await _kasaHaraketleriRepository.AddAsync(subItem);
                }
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<ResponseViewModel<KasaHaraketleriListDto>> Update(KasaHaraketleriUpdateDto item)
        {
            var dbItem = await _kasaHaraketleriRepository.GetByIdAsync(item.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _kasaHaraketleriRepository.Update(dbItem, item);

            var result = await _unitOfWork.SaveAsync();

            await AddSubItem(dbItem);

            return new ResponseViewModel<KasaHaraketleriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<KasaHaraketleriListDto>> Delete(Guid id)
        {
            var item = await _kasaHaraketleriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _kasaHaraketleriRepository.DeleteByTopId(item.Id);

            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KasaHaraketleriListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }
    }
}
