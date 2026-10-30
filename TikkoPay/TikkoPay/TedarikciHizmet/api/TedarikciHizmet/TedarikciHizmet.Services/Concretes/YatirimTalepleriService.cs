using AutoMapper;
using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.CallBacks;
using TedarikciHizmet.Core.DTOs.YatirimTalepleri;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using TedarikciHizmet.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TedarikciHizmet.Services.Concretes
{
    public class YatirimTalepleriService : IYatirimTalepleriService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IYatirimCekimTalepleriRepository _yatirimTalepleriRepository;
        private readonly ITedarikciRepository _tedarikciRepository;
        private readonly IWebSiteleriRepository _webSiteleriRepository;
        private readonly IDepositTransactionRepository _depositTransactionRepository;
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;
        private readonly HttpClient _httpClient;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;

        public YatirimTalepleriService(IMapper mapper,
            IUnitOfWork unitOfWork,
            IYatirimCekimTalepleriRepository yatirimTalepleriRepository,
            ITedarikciRepository tedarikciRepository,
            IWebSiteleriRepository webSiteleriRepository,
            IDepositTransactionRepository depositTransactionRepository,
            HttpClient httpClient,
            IHavaleBankalariRepository havaleBankalariRepository,
            IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
            _tedarikciRepository = tedarikciRepository;
            _webSiteleriRepository = webSiteleriRepository;
            _depositTransactionRepository = depositTransactionRepository;
            _httpClient = httpClient;
            _havaleBankalariRepository = havaleBankalariRepository;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> GetByIdAsync(Guid id)
        {
            var item = await _yatirimTalepleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<YatirimTalepleriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> CreateAsync(YatirimTalepleriCreateDto item)
        {
            YatirimCekimTalepleri model = _mapper.Map<YatirimCekimTalepleri>(item);

            if (model.IslemTipi == IslemTip.Deposit)
            {
                if (model.TedarikciId.HasValue)
                {
                    model.TedarikciKomisyon = await GetTedarikciYatirimKomisyonAsync(model.TedarikciId.Value, model.Tutar);
                }

                if (model.WebSiteId.HasValue)
                {
                    model.WebSiteKomisyon = await GetWebSiteYatirimKomisyonAsync(model.WebSiteId.Value, model.Tutar);
                }
            }

            if (model.IslemTipi == IslemTip.Withdrawal)
            {
                if (model.TedarikciId.HasValue)
                {
                    model.TedarikciKomisyon = await GetTedarikciCekimKomisyonAsync(model.TedarikciId.Value, model.Tutar);
                }

                if (model.WebSiteId.HasValue)
                {
                    model.WebSiteKomisyon = await GetWebSiteCekimKomisyonAsync(model.WebSiteId.Value, model.Tutar);
                }
            }

            await _yatirimTalepleriRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> UpdateAsync(YatirimTalepleriUpdateDto itemDto)
        {
            var dbItem = await _yatirimTalepleriRepository.GetByIdAsync(itemDto.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _yatirimTalepleriRepository.Update(dbItem, itemDto);
            _ = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> TedarikciyeYonlendirAsync(Guid id, Guid tedarikciId)
        {
            var dbItem = await _yatirimTalepleriRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if (dbItem.Durum != Durum.Bekliyor || dbItem.IslemTipi != IslemTip.Withdrawal)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Bekliyor durumundaki çekim talepleri yönlendirilebilir."
                };
            }

            dbItem.TedarikciKomisyon = await GetTedarikciCekimKomisyonAsync(tedarikciId, dbItem.Tutar);
            dbItem.TedarikciId = tedarikciId;

            _ = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Message = "Çekim talebi tedarikçiye yönlendirildi",
                Success = true
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> UpdatePriceAsync(Guid id, decimal price)
        {
            var dbItem = await _yatirimTalepleriRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if(dbItem.TedarikciId.HasValue)
            {
                dbItem.TedarikciKomisyon = await GetTedarikciYatirimKomisyonAsync(dbItem.TedarikciId.Value, price);
            }

            if (dbItem.WebSiteId.HasValue)
            {
                dbItem.WebSiteKomisyon = await GetWebSiteYatirimKomisyonAsync(dbItem.WebSiteId.Value, price);
            }

            dbItem.Tutar = price;
            dbItem.Toplam = dbItem.Tutar - dbItem.TedarikciKomisyon;

            _ = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Message = "Tutar güncellendi",
                Success = true
            };
        }

        private static string GenerateHash(string TransactionId, string UserId, int Status, string Type)
        {
            using var sha1 = SHA1.Create();
            var hashInput = $"{TransactionId}{UserId}{Status}{Type}";
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private static string GenerateHashForPronet(string TransactionId, string UserId, int Status, string Type, decimal Amount, string SecretKey)
        {
            using var sha1 = SHA1.Create();
            
            var amountString = Amount.ToString("0");
            var hashInput = $"{TransactionId}{UserId}{Status}{Type}{amountString}{SecretKey}";
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private YatirimCekimTalepleri GetYatirimTalepleri(Guid id, Guid tedarikciId)
        {
            return _yatirimTalepleriRepository.Find(k => k.Id == id &&
                                                         k.TedarikciId == tedarikciId &&
                                                         k.IslemTipi == IslemTip.Deposit &&
                                                         k.Durum == Durum.Bekliyor &&
                                                         !k.SilindiMi).FirstOrDefault();
        }

        private async Task<DepositTransaction> GetDepositTransactionAsync(string webSiteTransactionId)
        {
            return await _depositTransactionRepository.GetByTransactionIdAsync(webSiteTransactionId);
        }

        private async Task<WebSiteleri> GetWebSiteAsync(Guid webSiteId)
        {
            return await _webSiteleriRepository.GetByIdAsync(webSiteId);
        }

        private DepositCallBackDto GenerateDepositCallBackDto(DepositTransaction depositTransaction, YatirimCekimTalepleri yatirimCekimTalepleri, Durum durum, int websiteType, string secretKey)
        {
            var depositPostData = new DepositCallBackDto
            {
                userID = depositTransaction.UserId,
                username = depositTransaction.UserName,
                name = depositTransaction.FullName,
                amount = yatirimCekimTalepleri.Tutar,
                verifiedAmount = yatirimCekimTalepleri.Tutar,
                transaction_id = yatirimCekimTalepleri.WebSiteTransactionId,
                status = (int)durum, //Transaction status (e.g., 1: Pending 2: Approved 3: Rejected)
                type = nameof(IslemTip.Deposit), //Transaction type (e.g., 1: Deposit, 0: Withdrawal)
                payment_method_id = yatirimCekimTalepleri.YontemTipi
            };

            if (websiteType == (int)WebsiteType.Pronet)
            {
                depositPostData.hash = GenerateHashForPronet(depositPostData.transaction_id, 
                                                             depositPostData.userID, 
                                                             depositPostData.status, 
                                                             depositPostData.type,
                                                             yatirimCekimTalepleri.Tutar,
                                                             secretKey);
            }
            else
            {
                depositPostData.hash = GenerateHash(depositPostData.transaction_id, depositPostData.userID, depositPostData.status, depositPostData.type);
            }


            return depositPostData;
        }

        private async Task<string> PostDepositCallbackAsync(DepositCallBackDto depositPostData, string depositCallbackUrl)
        {
            var serializerdItem = JsonSerializer.Serialize(depositPostData);
            StringContent stringContent = new(serializerdItem, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(depositCallbackUrl, stringContent);
            var statusCode = response.IsSuccessStatusCode;
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> ApproveAsync(Guid id, Guid tedarikciId)
        {
            var yatirimTalepleriDbItem = GetYatirimTalepleri(id, tedarikciId);
            if (yatirimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var depositTransactionDbItem = await GetDepositTransactionAsync(yatirimTalepleriDbItem.WebSiteTransactionId);
            if (depositTransactionDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Deposit Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await GetWebSiteAsync(depositTransactionDbItem.WebSiteId);
            if (webSiteItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "WebSite kaydı bulunamadı."
                };
            }

            yatirimTalepleriDbItem.Durum = Durum.Onaylandi;
            yatirimTalepleriDbItem.OnayTarihi = DateTime.Now;

            _ = await _unitOfWork.SaveAsync();

            var responseViewModel = new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true,
                Message = "Yatırım talebi onaylandı"
            };

            string webSitePaymentMethodSecretKey = string.Empty;
            string depositCallbackUrl = webSiteItem.DepositCallbackUrl;
            if (yatirimTalepleriDbItem.WebSitePaymentMethodId.HasValue)
            {
                var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetByIdAsync(yatirimTalepleriDbItem.WebSitePaymentMethodId.Value);
                if (webSitePaymentMethodItem != null)
                {
                    webSitePaymentMethodSecretKey = webSitePaymentMethodItem.ApiClientSecret;
                    depositCallbackUrl = webSitePaymentMethodItem.DepositCallbackUrl;
                }
            }

            try
            {
                var depositPostData = GenerateDepositCallBackDto(depositTransactionDbItem, yatirimTalepleriDbItem, Durum.Onaylandi, webSiteItem.Type, webSitePaymentMethodSecretKey);
                var content = await PostDepositCallbackAsync(depositPostData, depositCallbackUrl);
                
                yatirimTalepleriDbItem.IsSendToWebSite = true;
                yatirimTalepleriDbItem.WebSiteCallBackResult = content;

                depositTransactionDbItem.IsCompleted = true;

                if (yatirimTalepleriDbItem.HavaleBankaId.HasValue)
                {
                    var havaleBankalariItem = await _havaleBankalariRepository.GetByIdAsync(yatirimTalepleriDbItem.HavaleBankaId.Value);
                    havaleBankalariItem.OnaylananYatirimAdedi += 1;
                    havaleBankalariItem.OnaylananYatirimToplami += yatirimTalepleriDbItem.Tutar;
                }

                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                yatirimTalepleriDbItem.IsSendToWebSite = true;
                yatirimTalepleriDbItem.WebSiteCallBackResult = ex.Message;

                await _unitOfWork.SaveAsync();

                responseViewModel.Success = false;
                responseViewModel.Message = $"İşlem sırasında hata oluştu: {ex.Message}";
            }

            return responseViewModel;
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> CancelAsync(Guid id, Guid tedarikciId, string iptalAciklama)
        {
            var yatirimTalepleriDbItem = GetYatirimTalepleri(id, tedarikciId);
            if (yatirimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var depositTransactionDbItem = await GetDepositTransactionAsync(yatirimTalepleriDbItem.WebSiteTransactionId);
            if (depositTransactionDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Deposit Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await GetWebSiteAsync(depositTransactionDbItem.WebSiteId);
            if (webSiteItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "WebSite kaydı bulunamadı."
                };
            }

            yatirimTalepleriDbItem.IptalAciklama = iptalAciklama;
            yatirimTalepleriDbItem.Durum = Durum.Red;
            yatirimTalepleriDbItem.OnayTarihi = DateTime.Now;

            depositTransactionDbItem.IsCompleted = true;

            _ = await _unitOfWork.SaveAsync();

            var responseViewModel = new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true,
                Message = "Yatırım talebi iptal edildi",
            };

            string webSitePaymentMethodSecretKey = string.Empty;
            string depositCallbackUrl = webSiteItem.DepositCallbackUrl;
            if (yatirimTalepleriDbItem.WebSitePaymentMethodId.HasValue)
            {
                var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetByIdAsync(yatirimTalepleriDbItem.WebSitePaymentMethodId.Value);
                if (webSitePaymentMethodItem != null)
                {
                    webSitePaymentMethodSecretKey = webSitePaymentMethodItem.ApiClientSecret;
                    depositCallbackUrl = webSitePaymentMethodItem.DepositCallbackUrl;
                }
            }

            if (webSiteItem.Type == (int)WebsiteType.Pronet || webSiteItem.Id.Equals(Guid.Parse("809A5E8C-8861-4283-99E2-EDF652AFA3B4")))
            {
                try
                {
                    var depositPostData = GenerateDepositCallBackDto(depositTransactionDbItem, yatirimTalepleriDbItem, Durum.Red, webSiteItem.Type, webSitePaymentMethodSecretKey);
                    var content = await PostDepositCallbackAsync(depositPostData, depositCallbackUrl);

                    yatirimTalepleriDbItem.IsSendToWebSite = true;
                    yatirimTalepleriDbItem.WebSiteCallBackResult = content;

                    await _unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {
                    yatirimTalepleriDbItem.IsSendToWebSite = true;
                    yatirimTalepleriDbItem.WebSiteCallBackResult = ex.Message;

                    await _unitOfWork.SaveAsync();

                    responseViewModel.Success = false;
                    responseViewModel.Message = $"İşlem sırasında hata oluştu: {ex.Message}";
                }
            }

            return responseViewModel;
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> DeleteAsync(Guid id)
        {
            var item = await _yatirimTalepleriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            _yatirimTalepleriRepository.Delete(item);
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

        public async Task<int> GetToplamBekleyenYatirimlar(Guid? tedarikciId)
        {
            var count = await _yatirimTalepleriRepository.GetCountAsync(tedarikciId, IslemTip.Deposit, Durum.Bekliyor);
            return count;
        }

        public async Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _yatirimTalepleriRepository.GetAllAsync(tedarikciId, bankaId, searchText, IslemTip.Deposit, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _yatirimTalepleriRepository.GetCountAsync(tedarikciId, bankaId, searchText, IslemTip.Deposit, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriRepository.GetAllAsync(tedarikciId, bankaId, searchText, IslemTip.Deposit, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<ToplamYatirimTutariDto>> GetToplamYatirimTutari(Guid tedarikciId, Guid? bankaId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var result = await _yatirimTalepleriRepository.GetToplamYatirimTutari(tedarikciId, bankaId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<ToplamYatirimTutariDto> { Success = true, Result = result };
        }

        public async Task<ResponseViewModel<ToplamCekimTutariDto>> GetToplamCekimTutari(Guid tedarikciId, Guid? bankaId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var result = await _yatirimTalepleriRepository.GetToplamCekimTutari(tedarikciId, bankaId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<ToplamCekimTutariDto> { Success = true, Result = result };
        }

        private async Task<decimal> GetTedarikciCekimKomisyonAsync(Guid tedarikciId, decimal tutar)
        {
            var tedariciItem = await _tedarikciRepository.GetByIdAsync(tedarikciId);
            var cekimKomisyonOrani = tedariciItem.CekimKomisyonOrani / 100;

            return tutar * (decimal)cekimKomisyonOrani;
        }

        private async Task<decimal> GetWebSiteCekimKomisyonAsync(Guid webSiteId, decimal tutar)
        {
            var webSiteItem = await _webSiteleriRepository.GetByIdAsync(webSiteId);
            var cekimKomisyonOrani = webSiteItem.CekimKomisyonOrani / 100;

            return tutar * (decimal)cekimKomisyonOrani;
        }

        private async Task<decimal> GetTedarikciYatirimKomisyonAsync(Guid tedarikciId, decimal tutar)
        {
            var tedarikciItem = await _tedarikciRepository.GetByIdAsync(tedarikciId);
            var yatirimKomisyonOrani = tedarikciItem.YaririmKomisyonOrani / 100;

            return tutar * (decimal)yatirimKomisyonOrani;
        }

        private async Task<decimal> GetWebSiteYatirimKomisyonAsync(Guid webSiteId, decimal tutar)
        {
            var webSiteItem = await _webSiteleriRepository.GetByIdAsync(webSiteId);
            var yatirimKomisyonOrani = webSiteItem.YaririmKomisyonOrani / 100;

            return tutar * (decimal)yatirimKomisyonOrani;
        }
    }
}
