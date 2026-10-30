using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Infrastructure.Helpers;
using TikkoPayCallbacks.Infrastructure.Repositories.Interfaces;
using TikkoPayCallbacks.Infrastructure.Services.Interfaces;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Infrastructure.Services.Concretes
{
    public class ViPayWithdrawCallbackService : IViPayWithdrawCallbackService
    {
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IWithdrawRepository _withdrawRepository;
        private readonly IWebSiteRepository _webSiteRepository;
        private readonly HttpClient _httpClient;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;
        private readonly ITelegramBotService _telegramBotService;

        public ViPayWithdrawCallbackService(IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository, 
                                            IWithdrawRepository withdrawRepository, 
                                            IWebSiteRepository webSiteRepository, 
                                            HttpClient httpClient, 
                                            IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository, 
                                            ITelegramBotService telegramBotService)
        {
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _withdrawRepository = withdrawRepository;
            _webSiteRepository = webSiteRepository;
            _httpClient = httpClient;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
            _telegramBotService = telegramBotService;
        }

        public async Task<ProcessResultModel> ProcessWithdraw(ViPayCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings)
        {
            if (cekimTalebi.TedarikciId != settings.ViPayTedarikciId)
            {
                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "Çekim talebi bulunamadı"
                };
            }

            if (cekimTalebi.Durum == (int)Durum.Onaylandi)
            {
                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "Çekim talebi daha önce onaylanmış"
                };
            }

            var withDrawTransactionDbItem = await _withdrawRepository.GetByTransactionIdAsync(cekimTalebi.WebSiteTransactionId);
            if (withDrawTransactionDbItem is null)
            {
                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "Transaction kaydı bulunamadı."
                };
            }

            var webSiteItem = await _webSiteRepository.GetByIdWithAllColumnsAsync(withDrawTransactionDbItem.WebSiteId);
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

            await _yatirimCekimTalepleriRepository.UpdateDurumByIdAsync(cekimTalebi.Id, status);

            if (status == (int)Durum.Red)
            {
                var telegramMessage = $"{cekimTalebi.TalepNo}{cekimTalebi.IdNumber} talep numaralı çekim talebi vipay tarafından reddedilmiştir.";
                var telegramResponse = await _telegramBotService.SendMessageAsync(TelegramBotConstants.NorthNotificationsChannelId, telegramMessage);

                return new ProcessResultModel
                {
                    IsSuccess = true,
                    Status = "success",
                    Message = "Withdrawal status updated"
                };
            }

            var amount = decimal.Parse(request.Amount.Trim().Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

            string webSitePaymentMethodSecretKey = string.Empty;
            string withdrawCallbackUrl = webSiteItem.WithdrawCallbackUrl;
            if (cekimTalebi.WebSitePaymentMethodId.HasValue)
            {
                var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetByIdAsync(cekimTalebi.WebSitePaymentMethodId.Value);
                if (webSitePaymentMethodItem != null)
                {
                    webSitePaymentMethodSecretKey = webSitePaymentMethodItem.ApiClientSecret;
                    withdrawCallbackUrl = webSitePaymentMethodItem.WithdrawCallbackUrl;
                }
            }

            var amkWebSiteId = Guid.Parse("A73517A1-3B24-4BEE-98C7-BC3A2F3FDD30");
            if (status == (int)Durum.Red)
            {
                if (webSiteItem.Id.Equals(amkWebSiteId))
                {
                    return await ProcessWithdrawalStatusUpdate(withDrawTransactionDbItem, webSiteItem, cekimTalebi, amount, status, withdrawCallbackUrl, webSitePaymentMethodSecretKey);
                }
                else
                {
                    return new ProcessResultModel
                    {
                        IsSuccess = true,
                        Status = "success",
                        Message = "Withdrawal status updated"
                    };
                }
            }
            else
            {
                return await ProcessWithdrawalStatusUpdate(withDrawTransactionDbItem, webSiteItem, cekimTalebi, amount, status, withdrawCallbackUrl, webSitePaymentMethodSecretKey);
            }
        }

        private async Task<ProcessResultModel> ProcessWithdrawalStatusUpdate(WithdrawTransaction withdrawTransaction, WebSiteleri webSite, YatirimCekimTalepleri withdrawRequest, decimal amount, int status, string withdrawCallbackUrl, string secretKey)
        {
            if (!withdrawRequest.TopId.HasValue)
            {
                var withdrawalPostData = CreateWithdrawalPostData(withdrawTransaction, amount, status, webSite.Type, secretKey);

                return await PostWithdrawalCallback(withdrawalPostData, withdrawCallbackUrl, withdrawRequest.Id.ToString());
            }

            return await HandleSubItems(withdrawTransaction, webSite, withdrawRequest.TopId.Value, status, withdrawCallbackUrl, secretKey);
        }

        private StringContent CreateWithdrawalPostData(WithdrawTransaction withdrawTransaction, decimal amount, int status, int websiteType, string secretKey)
        {
            var withdrawPostData = new BetcoWithdrawalCallBackDto
            {
                userID = withdrawTransaction.UserId,
                username = withdrawTransaction.UserName,
                name = withdrawTransaction.FullName,
                amount = amount,
                verifiedAmount = amount,
                transaction_id = withdrawTransaction.TransactionId,
                status = status,
                type = "Withdrawal",
                payment_method_id = "Banka Havale",
                hash = GenerateHashForBetco(withdrawTransaction.TransactionId, withdrawTransaction.UserId, status, "Withdrawal")
            };

            if (websiteType == (int)WebsiteType.Pronet)
            {
                withdrawPostData.hash = GenerateHashForPronet(withdrawPostData.transaction_id,
                                                                withdrawPostData.userID,
                                                                withdrawPostData.status,
                                                                withdrawPostData.type,
                                                                amount,
                                                                secretKey);
            }
            else
            {
                withdrawPostData.hash = GenerateHashForBetco(withdrawTransaction.TransactionId, withdrawTransaction.UserId, status, "Withdrawal");
            }

            var serializedData = JsonSerializer.Serialize(withdrawPostData);
            return new StringContent(serializedData, Encoding.UTF8, "application/json");
        }

        private async Task<ProcessResultModel> PostWithdrawalCallback(StringContent withdrawalData, string callbackUrl, string requestId)
        {
            try
            {
                var response = await _httpClient.PostAsync(callbackUrl, withdrawalData);
                var responseContent = await response.Content.ReadAsStringAsync();

                await _yatirimCekimTalepleriRepository.UpdateWebsiteCallBackResultByIdAsync(requestId, responseContent);

                return new ProcessResultModel
                {
                    IsSuccess = true,
                    Status = "success",
                    Message = "Withdrawal successfully posted"
                };
            }
            catch (Exception ex)
            {
                await LogRequestAsync(ex.Message);

                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "An error occurred while sending the withdrawal"
                };
            }
        }

        private async Task LogRequestAsync(string content)
        {
            var logDirectory = Path.Combine("logs", "request", DateTime.Now.ToString("yyyy-MM-dd"), "error");
            var logFilePath = Path.Combine(logDirectory, $"log-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.txt");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            using (var writer = new StreamWriter(logFilePath, append: false, Encoding.UTF8))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("/******** Content ********/");
                sb.AppendLine(content);
                sb.AppendLine(" ");

                await writer.WriteAsync(sb.ToString());
            }
        }

        private async Task<ProcessResultModel> HandleSubItems(WithdrawTransaction withdrawTransaction, WebSiteleri webSite, Guid topId, int status, string withdrawCallbackUrl, string secretKey)
        {
            var subItems = await _yatirimCekimTalepleriRepository.GetCekimTalepleriByTopId(topId);
            bool allApproved = subItems.All(item => item.Durum == (int)Durum.Onaylandi);

            if (!allApproved)
            {
                return new ProcessResultModel
                {
                    IsSuccess = true,
                    Status = "success",
                    Message = "Withdrawal status updated"
                };
            }

            var topWithdrawRequest = await _yatirimCekimTalepleriRepository.GetById(topId);
            await _yatirimCekimTalepleriRepository.UpdateDurumByIdAsync(topWithdrawRequest.Id, status);

            var withdrawalData = CreateWithdrawalPostData(withdrawTransaction, topWithdrawRequest.Tutar, status, webSite.Type, secretKey);

            return await PostWithdrawalCallback(withdrawalData, withdrawCallbackUrl, topWithdrawRequest.Id.ToString());
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
