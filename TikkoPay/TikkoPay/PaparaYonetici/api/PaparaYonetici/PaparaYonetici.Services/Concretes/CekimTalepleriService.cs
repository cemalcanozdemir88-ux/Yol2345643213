using PaparaYonetici.Core.DTOs;
using PaparaYonetici.Core.DTOs.CallBacks;
using PaparaYonetici.Core.DTOs.YatirimTalepleri;
using PaparaYonetici.Core.Enums;
using PaparaYonetici.Core.Interfaces;
using PaparaYonetici.Services.Interfaces;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;

namespace PaparaYonetici.Services.Concretes
{
    public class CekimTalepleriService : ICekimTalepleriService
    {
        private readonly IYatirimTalepleriRepository _yatirimTalepleriRepository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;
        private readonly IWebSiteleriRepository _webSiteleriRepository;
        private IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;

        public CekimTalepleriService(IYatirimTalepleriRepository yatirimTalepleriRepository,
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

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> Approve(Guid id)
        {
            var yatirimTalepleriDbItem = await _yatirimTalepleriRepository.GetByIdAsync(id);
            if (yatirimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            var withDrawTransactionDbItem = await _withdrawTransactionRepository.GetByTransactionIdAsync(yatirimTalepleriDbItem.WebSiteTransactionId);
            if (withDrawTransactionDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Withdraw Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await _webSiteleriRepository.GetByIdAsync(withDrawTransactionDbItem.WebSiteId);
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

            var result = await _unitOfWork.SaveAsync();

            try
            {
                var depositPostData = new DepositCallBackDto();
                depositPostData.userID = withDrawTransactionDbItem.UserId;
                depositPostData.username = withDrawTransactionDbItem.UserName;
                depositPostData.name = withDrawTransactionDbItem.FullName;
                depositPostData.amount = yatirimTalepleriDbItem.Tutar;
                depositPostData.verifiedAmount = yatirimTalepleriDbItem.Tutar;
                depositPostData.transaction_id = yatirimTalepleriDbItem.WebSiteTransactionId;
                depositPostData.status = (int)Durum.Onaylandi; //Transaction status (e.g., 1: Pending 2: Approved 3: Rejected)
                depositPostData.type = "Withdrawal"; //Transaction type (e.g., 1: Deposit, 0: Withdrawal)
                depositPostData.payment_method_id = yatirimTalepleriDbItem.YontemTipi;
                depositPostData.hash = GenerateHash(depositPostData.transaction_id, depositPostData.userID, depositPostData.status, depositPostData.type);

                var serializerdItem = JsonSerializer.Serialize(depositPostData);
                StringContent stringContent = new StringContent(serializerdItem, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(webSiteItem.DepositCallbackUrl, stringContent);
                var statusCode = response.IsSuccessStatusCode;
                var content = await response.Content.ReadAsStringAsync();

                yatirimTalepleriDbItem.IsSendToWebSite = true;
                yatirimTalepleriDbItem.WebSiteCallBackResult = content;

                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                yatirimTalepleriDbItem.IsSendToWebSite = true;
                yatirimTalepleriDbItem.WebSiteCallBackResult = ex.Message;

                await _unitOfWork.SaveAsync();

                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = $"İşlem sırasında hata oluştu: {ex.Message}"
                };
            }

            return new ResponseViewModel<YatirimTalepleriListDto>
            {
                Message = "Çekim talebi onaylandı",
                Success = true
            };

        }

        private string GenerateHash(string TransactionId, string UserId, int Status, string Type)
        {
            using (var sha1 = SHA1.Create())
            {
                var hashInput = $"{TransactionId}{UserId}{Status}{Type}";
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
