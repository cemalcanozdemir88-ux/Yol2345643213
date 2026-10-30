using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ParaYolCallbacks.Infrastructure.Dtos;
using ParaYolCallbacks.Infrastructure.Entities;
using ParaYolCallbacks.Infrastructure.Helpers;
using ParaYolCallbacks.Infrastructure.Repositories.Interfaces;
using ParaYolCallbacks.Infrastructure.Services.Interfaces;
using ParaYolCallbacks.Models;

namespace ParaYolCallbacks.Infrastructure.Services.Concretes
{
    public class ViPayDepositCallbackService : IViPayDepositCallbackService
    {
        private readonly HttpClient _httpClient;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IWebSiteRepository _webSiteRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly ITedarikcilerRepository _tedarikcilerRepository;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;

        public ViPayDepositCallbackService(HttpClient httpClient,
                                           IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                                           IWebSiteRepository webSiteRepository,
                                           IDepositRepository depositRepository,
                                           ITedarikcilerRepository tedarikcilerRepository,
                                           IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository)
        {
            _httpClient = httpClient;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _webSiteRepository = webSiteRepository;
            _depositRepository = depositRepository;
            _tedarikcilerRepository = tedarikcilerRepository;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
        }

        public async Task<ProcessResultModel> ProcessDeposit(ViPayCallbackRequest request, YatirimCekimTalepleri yatirimTalebi, Settings settings)
        {
            if (yatirimTalebi.TedarikciId != settings.ViPayTedarikciId)
            {
                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "Yatirim talebi bulunamadı"
                };
            }

            if (yatirimTalebi.Durum == (int)Durum.Onaylandi)
            {
                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "Yatirim talebi daha önce onaylanmış"
                };
            }

            var depositTransactionDbItem = await _depositRepository.GetByTransactionIdAsync(yatirimTalebi.WebSiteTransactionId);
            if (depositTransactionDbItem is null)
            {
                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await _webSiteRepository.GetByIdWithAllColumnsAsync(depositTransactionDbItem.WebSiteId);
            if (webSiteItem == null)
            {
                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "WebSite kaydı bulunamadı."
                };
            }

            int status = request.Status.Equals("approved") ? (int)Durum.Onaylandi : (int)Durum.Red;

            await _yatirimCekimTalepleriRepository.UpdateDurumByIdAsync(yatirimTalebi.Id, status);
            if (status == (int)Durum.Red)
            {
                await _depositRepository.UpdateIsCompletedAsync(depositTransactionDbItem.Id, true);

                return new ProcessResultModel
                {
                    IsSuccess = true,
                    Status = "success",
                    Message = "Transaction completed successfully"
                };
            }

            string webSitePaymentMethodSecretKey = string.Empty;
            string depositCallbackUrl = webSiteItem.DepositCallbackUrl;
            if (yatirimTalebi.WebSitePaymentMethodId.HasValue)
            {
                var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetByIdAsync(yatirimTalebi.WebSitePaymentMethodId.Value);
                if (webSitePaymentMethodItem != null)
                {
                    webSitePaymentMethodSecretKey = webSitePaymentMethodItem.ApiClientSecret;
                    depositCallbackUrl = webSitePaymentMethodItem.DepositCallbackUrl;
                }
            }


            var amount = decimal.Parse(request.Amount.Trim().Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

            var depositPostData = CreateDepositPostData(yatirimTalebi, amount, status, webSiteItem.Type, webSitePaymentMethodSecretKey);

            try
            {
                var response = await _httpClient.PostAsync(depositCallbackUrl, depositPostData);
                var statusCode = response.IsSuccessStatusCode;
                var content = await response.Content.ReadAsStringAsync();

                if (yatirimTalebi.Tutar != amount)
                {
                    var tutar = amount;
                    var tedarikciKomisyon = await GetTedarikciYatirimKomisyon(yatirimTalebi.TedarikciId.Value, tutar);
                    var webSiteKomisyon = await GetWebSiteYatirimKomisyon(yatirimTalebi.WebSiteId.Value, tutar);

                    await _yatirimCekimTalepleriRepository.UpdateWebsiteCallBackResultByTransactionIdWithTutarAsync(yatirimTalebi.WebSiteTransactionIdBigIntValue.Value, content, tutar, tedarikciKomisyon, webSiteKomisyon);
                }
                else
                {
                    await _yatirimCekimTalepleriRepository.UpdateWebsiteCallBackResultByTransactionIdAsync(yatirimTalebi.WebSiteTransactionIdBigIntValue.Value, content);
                }

                await _depositRepository.UpdateIsCompletedAsync(depositTransactionDbItem.Id, true);

                return new ProcessResultModel
                {
                    IsSuccess = true,
                    Status = "success",
                    Message = "Transaction completed successfully"
                };
            }
            catch
            {
                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "An error occurred while sending the deposit"
                };
            }
        }

        private StringContent CreateDepositPostData(YatirimCekimTalepleri yatirimTalebi, decimal amount, int status, int websiteType, string secretKey)
        {
            var depositPostData = new BetcoDepositCallBackDto();
            depositPostData.userID = yatirimTalebi.UserId;
            depositPostData.username = yatirimTalebi.UserName;
            depositPostData.name = yatirimTalebi.AdSoyad;
            depositPostData.amount = amount;
            depositPostData.verifiedAmount = amount;
            depositPostData.transaction_id = yatirimTalebi.WebSiteTransactionId;
            depositPostData.status = status;
            depositPostData.type = "Deposit";
            depositPostData.payment_method_id = "Banka Havale";

            if (websiteType == (int)WebsiteType.Pronet)
            {
                depositPostData.hash = GenerateHashForPronet(depositPostData.transaction_id,
                                                             depositPostData.userID,
                                                             depositPostData.status,
                                                             depositPostData.type,
                                                             amount,
                                                             secretKey);
            }
            else
            {
                depositPostData.hash = GenerateHashForBetco(depositPostData.transaction_id, depositPostData.userID, depositPostData.status, depositPostData.type);
            }

            var serializerdItem = JsonSerializer.Serialize(depositPostData);
            return new StringContent(serializerdItem, Encoding.UTF8, "application/json");
        }

        private async Task<decimal> GetTedarikciYatirimKomisyon(Guid tedarikciId, decimal tutar)
        {
            return await _tedarikcilerRepository.GetYatirimKomisyonu(tedarikciId, tutar);
        }

        private async Task<decimal> GetWebSiteYatirimKomisyon(Guid webSiteId, decimal tutar)
        {
            return await _webSiteRepository.GetYatirimKomisyonuAsync(webSiteId, tutar);
        }

        private static string GenerateHashForPronet(string TransactionId, string UserId, int Status, string Type, decimal Amount, string SecretKey)
        {
            using var sha1 = SHA1.Create();

            var amountString = Amount.ToString("0");
            var hashInput = $"{TransactionId}{UserId}{Status}{Type}{amountString}{SecretKey}";
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string GenerateHashForBetco(string TransactionId, string UserId, int Status, string Type)
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
