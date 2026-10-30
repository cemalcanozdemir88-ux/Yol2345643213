using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.CallBacks;
using PaparaTedarikciHizmet.Core.DTOs.YatirimTalepleri;
using PaparaTedarikciHizmet.Core.Enums;
using PaparaTedarikciHizmet.Core.Interfaces;
using PaparaTedarikciHizmet.Services.Interfaces;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using PaparaTedarikciHizmet.Core.Models;

namespace PaparaTedarikciHizmet.Services.Concretes
{
    public class CekimTalepleriService : ICekimTalepleriService
    {
        private readonly IYatirimCekimTalepleriRepository _yatirimTalepleriRepository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;
        private readonly IWebSiteleriRepository _webSiteleriRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;

        public CekimTalepleriService(IYatirimCekimTalepleriRepository yatirimTalepleriRepository,
                                     IWithdrawTransactionRepository withdrawTransactionRepository,
                                     IWebSiteleriRepository webSiteleriRepository,
                                     IUnitOfWork unitOfWork,
                                     HttpClient httpClient)
        {
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
            _webSiteleriRepository = webSiteleriRepository;
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> ApproveAsync(Guid id, Guid tedarikciId, string onayAciklama)
        {
            var cekimTalepleriDbItem = GetCekimTalepleri(id, tedarikciId);
            if (cekimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            WithdrawTransaction withDrawTransactionDbItem = new WithdrawTransaction();
            WebSiteleri webSiteItem = new WebSiteleri();

            if (!cekimTalepleriDbItem.YontemTipi.Equals("Manuel"))
            {
                withDrawTransactionDbItem = await GetWithdrawTransactionAsync(cekimTalepleriDbItem.WebSiteTransactionId);
                if (withDrawTransactionDbItem == null)
                {
                    return new ResponseViewModel<YatirimTalepleriListDto>
                    {
                        Success = false,
                        Message = "Withdraw Transaction kaydı bulunamadı."
                    };
                }

                webSiteItem = await GetWebSiteAsync(withDrawTransactionDbItem.WebSiteId);
                if (webSiteItem == null)
                {
                    return new ResponseViewModel<YatirimTalepleriListDto>
                    {
                        Success = false,
                        Message = "WebSite kaydı bulunamadı."
                    };
                }
            }

            cekimTalepleriDbItem.IptalAciklama = onayAciklama;
            cekimTalepleriDbItem.Durum = Durum.Onaylandi;
            cekimTalepleriDbItem.OnayTarihi = DateTime.Now;

            _ = await _unitOfWork.SaveAsync();

            var responseViewModel = new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true,
                Message = "Çekim talebi onaylandı"
            };

            if(!cekimTalepleriDbItem.YontemTipi.Equals("Manuel"))
            {
                try
                {
                    var withdrawalPostData = GenerateWithdrawalCallBackDto(withDrawTransactionDbItem, cekimTalepleriDbItem, Durum.Onaylandi, webSiteItem.Type);
                    var content = await PostWithdrawalCallbackAsync(withdrawalPostData, webSiteItem.WithdrawCallbackUrl);

                    cekimTalepleriDbItem.IsSendToWebSite = true;
                    cekimTalepleriDbItem.WebSiteCallBackResult = content;

                    await _unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {
                    cekimTalepleriDbItem.IsSendToWebSite = true;
                    cekimTalepleriDbItem.WebSiteCallBackResult = ex.Message;

                    await _unitOfWork.SaveAsync();

                    responseViewModel.Success = false;
                    responseViewModel.Message = $"İşlem sırasında hata oluştu: {ex.Message}";
                }
            }

            return responseViewModel;

        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> CancelAsync(Guid id, Guid tedarikciId, string iptalAciklama)
        {
            var cekimTalepleriDbItem = GetCekimTalepleri(id, tedarikciId);
            if (cekimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            WithdrawTransaction withDrawTransactionDbItem = new WithdrawTransaction();
            WebSiteleri webSiteItem = new WebSiteleri();

            if (!cekimTalepleriDbItem.YontemTipi.Equals("Manuel"))
            {
                withDrawTransactionDbItem = await GetWithdrawTransactionAsync(cekimTalepleriDbItem.WebSiteTransactionId);
                if (withDrawTransactionDbItem == null)
                {
                    return new ResponseViewModel<YatirimTalepleriListDto>
                    {
                        Success = false,
                        Message = "Withdraw Transaction kaydı bulunamadı."
                    };
                }

                webSiteItem = await GetWebSiteAsync(withDrawTransactionDbItem.WebSiteId);
                if (webSiteItem == null)
                {
                    return new ResponseViewModel<YatirimTalepleriListDto>
                    {
                        Success = false,
                        Message = "WebSite kaydı bulunamadı."
                    };
                }
            }

                

            cekimTalepleriDbItem.IptalAciklama = iptalAciklama;
            cekimTalepleriDbItem.Durum = Durum.Red;
            cekimTalepleriDbItem.OnayTarihi = DateTime.Now;

            _ = await _unitOfWork.SaveAsync();

            var responseViewModel = new ResponseViewModel<YatirimTalepleriListDto>
            {
                Success = true,
                Message = "Çekim talebi iptal edildi"
            };

            if (!cekimTalepleriDbItem.YontemTipi.Equals("Manuel"))
            {
                try
                {
                    var withdrawalPostData = GenerateWithdrawalCallBackDto(withDrawTransactionDbItem, cekimTalepleriDbItem, Durum.Red, webSiteItem.Type);
                    var content = await PostWithdrawalCallbackAsync(withdrawalPostData, webSiteItem.WithdrawCallbackUrl);

                    cekimTalepleriDbItem.IsSendToWebSite = true;
                    cekimTalepleriDbItem.WebSiteCallBackResult = content;

                    await _unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {
                    cekimTalepleriDbItem.IsSendToWebSite = true;
                    cekimTalepleriDbItem.WebSiteCallBackResult = ex.Message;

                    await _unitOfWork.SaveAsync();

                    responseViewModel.Success = false;
                    responseViewModel.Message = $"İşlem sırasında hata oluştu: {ex.Message}";
                }
            }

            return responseViewModel;
        }

        public async Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _yatirimTalepleriRepository.GetAllAsync(tedarikciId, bankaId, searchText, IslemTip.Withdrawal, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            var count = await _yatirimTalepleriRepository.GetCountAsync(tedarikciId, bankaId, searchText, IslemTip.Withdrawal, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriRepository.GetAllAsync(tedarikciId, bankaId, searchText, IslemTip.Withdrawal, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        private YatirimCekimTalepleri GetCekimTalepleri(Guid id, Guid tedarikciId)
        {
            return _yatirimTalepleriRepository.Find(k => k.Id == id &&
                                                         k.TedarikciId == tedarikciId &&
                                                         k.IslemTipi == IslemTip.Withdrawal &&
                                                         k.Durum == Durum.Bekliyor &&
                                                         !k.SilindiMi).FirstOrDefault();
        }

        private async Task<WithdrawTransaction> GetWithdrawTransactionAsync(string webSiteTransactionId)
        {
            return await _withdrawTransactionRepository.GetByTransactionIdAsync(webSiteTransactionId);
        }

        private async Task<WebSiteleri> GetWebSiteAsync(Guid webSiteId)
        {
            return await _webSiteleriRepository.GetByIdAsync(webSiteId);
        }

        private WithdrawalCallBackDto GenerateWithdrawalCallBackDto(WithdrawTransaction withdrawTransaction, YatirimCekimTalepleri cekimTalepleri, Durum durum, int websiteType)
        {
            var withdrawalPostData = new WithdrawalCallBackDto
            {
                userID = withdrawTransaction.UserId,
                username = withdrawTransaction.UserName,
                name = withdrawTransaction.FullName,
                amount = cekimTalepleri.Tutar,
                verifiedAmount = cekimTalepleri.Tutar,
                transaction_id = cekimTalepleri.WebSiteTransactionId,
                status = (int)durum, //Transaction status (e.g., 1: Pending 2: Approved 3: Rejected)
                type = nameof(IslemTip.Withdrawal), //Transaction type (e.g., 1: Deposit, 0: Withdrawal)
                payment_method_id = cekimTalepleri.YontemTipi
            };

            if (websiteType == (int)WebsiteType.Pronet)
            {
                withdrawalPostData.hash = GenerateHashForPronet(withdrawalPostData.transaction_id, 
                                                                withdrawalPostData.userID, 
                                                                withdrawalPostData.status, 
                                                                withdrawalPostData.type,
                                                                withdrawTransaction.ClientId,
                                                                withdrawTransaction.ClientSecret);
            }
            else
            {
                withdrawalPostData.hash = GenerateHash(withdrawalPostData.transaction_id, withdrawalPostData.userID, withdrawalPostData.status, withdrawalPostData.type);
            }

            return withdrawalPostData;
        }

        private async Task<string> PostWithdrawalCallbackAsync(WithdrawalCallBackDto withdrawalPostData, string WithdrawCallbackUrl)
        {
            var serializerdItem = JsonSerializer.Serialize(withdrawalPostData);
            StringContent stringContent = new(serializerdItem, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(WithdrawCallbackUrl, stringContent);
            var statusCode = response.IsSuccessStatusCode;
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        private static string GenerateHash(string TransactionId, string UserId, int Status, string Type)
        {
            using var sha1 = SHA1.Create();
            var hashInput = $"{TransactionId}{UserId}{Status}{Type}";
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private static string GenerateHashForPronet(string TransactionId, string UserId, int Status, string Type, string ClientId, string ClientSecret)
        {
            using var sha1 = SHA1.Create();
            var hashInput = $"{TransactionId}{UserId}{Status}{Type}{ClientId}{ClientSecret}";
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
