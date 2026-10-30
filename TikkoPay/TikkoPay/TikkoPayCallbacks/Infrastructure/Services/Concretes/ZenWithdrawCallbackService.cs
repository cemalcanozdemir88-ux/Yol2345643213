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
    public class ZenWithdrawCallbackService : IZenWithdrawCallbackService
    {
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IWithdrawRepository _withdrawRepository;
        private readonly IWebSiteRepository _webSiteRepository;
        private readonly HttpClient _httpClient;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;
        private readonly ITelegramBotService _telegramBotService;

        public ZenWithdrawCallbackService(IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
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

        public async Task<ProcessResultModel> ProcessWithdraw(ZenWithdrawCallbackRequest request, YatirimCekimTalepleri cekimTalebi, Settings settings)
        {
            if (cekimTalebi.TedarikciId != settings.ZenEntegreTedarikciId)
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

            if (request.Transaction.Status == 0)
            {
                return new ProcessResultModel
                {
                    IsSuccess = false,
                    Status = "error",
                    Message = "Transaction Status Is Pending"
                };
            }

            int status = request.Transaction.Status == 1 ? (int)Durum.Onaylandi : (int)Durum.Red;

            await _yatirimCekimTalepleriRepository.UpdateDurumByIdAsync(cekimTalebi.Id, status);

            if (status == (int)Durum.Red)
            {
                var telegramMessage = $"{cekimTalebi.TalepNo}{cekimTalebi.IdNumber} talep numaralı çekim talebi zen tarafından reddedilmiştir.";
                var telegramResponse = await _telegramBotService.SendMessageAsync(TelegramBotConstants.NorthNotificationsChannelId, telegramMessage);

                return new ProcessResultModel
                {
                    IsSuccess = true,
                    Status = "success",
                    Message = "Withdrawal status updated"
                };
            }

            var amount = decimal.Parse(request.Transaction.ConfirmedAmount.Trim().Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

            string withdrawCallbackUrl = webSiteItem.WithdrawCallbackUrl;
            if (cekimTalebi.WebSitePaymentMethodId.HasValue)
            {
                var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetByIdAsync(cekimTalebi.WebSitePaymentMethodId.Value);
                if (webSitePaymentMethodItem != null)
                {
                    withdrawCallbackUrl = webSitePaymentMethodItem.WithdrawCallbackUrl;
                }
            }

            return await ProcessWithdrawalStatusUpdate(withDrawTransactionDbItem, webSiteItem, cekimTalebi, amount, status, withdrawCallbackUrl);
        }

        private async Task<ProcessResultModel> ProcessWithdrawalStatusUpdate(WithdrawTransaction withdrawTransaction, WebSiteleri webSite, YatirimCekimTalepleri withdrawRequest, decimal amount, int status, string withdrawCallbackUrl)
        {
            if (!withdrawRequest.TopId.HasValue)
            {
                var withdrawalPostData = CreateWithdrawalPostData(withdrawTransaction, amount, status);

                return await PostWithdrawalCallback(withdrawalPostData, withdrawCallbackUrl, withdrawRequest.Id.ToString());
            }

            return await HandleSubItems(withdrawTransaction, webSite, withdrawRequest.TopId.Value, status, withdrawCallbackUrl);
        }

        private StringContent CreateWithdrawalPostData(WithdrawTransaction withdrawTransaction, decimal amount, int status)
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

        private async Task<ProcessResultModel> HandleSubItems(WithdrawTransaction withdrawTransaction, WebSiteleri webSite, Guid topId, int status, string withdrawCallbackUrl)
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

            var withdrawalData = CreateWithdrawalPostData(withdrawTransaction, topWithdrawRequest.Tutar, status);

            return await PostWithdrawalCallback(withdrawalData, withdrawCallbackUrl, topWithdrawRequest.Id.ToString());
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
