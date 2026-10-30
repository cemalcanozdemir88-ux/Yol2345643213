using Microsoft.AspNetCore.Mvc;
using PayCoTransactions.Infrastructure.Entities;
using PayCoTransactions.Infrastructure.Helpers;
using PayCoTransactions.Infrastructure.Repositories.Interfaces;
using PayCoTransactions.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PayCoTransactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private readonly IGeneralHelper _generalHelper;
        private readonly IConfiguration _configuration;
        private readonly ICallbackRepository _callbackRepository;
        private readonly IDepositTransactionRepository _depositTransactionRepository;
        private readonly IWebSiteRepository _webSiteRepository;
        private readonly IYatirimTalepleriRepository _yatirimTalepleriRepository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;
        private readonly HttpClient _httpClient;
        private readonly ICekimTalepleriRepository _cekimTalepleriRepository;

        public CallbackController(IGeneralHelper generalHelper,
                                  IConfiguration configuration,
                                  ICallbackRepository callbackRepository,
                                  IDepositTransactionRepository depositTransactionRepository,
                                  IWebSiteRepository webSiteRepository,
                                  IYatirimTalepleriRepository yatirimTalepleriRepository,
                                  IWithdrawTransactionRepository withdrawTransactionRepository,
                                  HttpClient httpClient,
                                  ICekimTalepleriRepository cekimTalepleriRepository)
        {
            _generalHelper = generalHelper;
            _configuration = configuration;
            _callbackRepository = callbackRepository;
            _depositTransactionRepository = depositTransactionRepository;
            _webSiteRepository = webSiteRepository;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
            _httpClient = httpClient;
            _cekimTalepleriRepository = cekimTalepleriRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaycoCallbackRequest callbackRequest)
        {
            if (!CheckToken(callbackRequest))
            {
                return BadRequest(new CallbackCreateResponse
                {
                    Success = false,
                    Message = "Invalid Token",
                    Status = 500
                });
            }

            await _callbackRepository.Insert(callbackRequest);

            if (callbackRequest.type.Equals("deposit"))
            {
                var depositTransactionItem = await _depositTransactionRepository.GetByTransactionId(callbackRequest.your_tx_id);
                if (depositTransactionItem is null)
                {
                    return BadRequest(new CallbackCreateResponse
                    {
                        Success = false,
                        Message = "Deposit Transaction not found",
                        Status = 500
                    });
                }

                var webSiteItem = await _webSiteRepository.GetById(depositTransactionItem.WebSiteId);

                var yatirimTalepItem = new YatirimTalepleri
                {
                    Id = Guid.NewGuid(),
                    TalepNo = DateTime.Now.ToString("yyyyMMdd"),
                    WebSiteId = webSiteItem.Id,
                    WebSiteTransactionId = depositTransactionItem.TransactionId,
                    AdSoyad = depositTransactionItem.FullName,
                    UserName = depositTransactionItem.UserName,
                    UserId = depositTransactionItem.UserId,
                };

                _ = decimal.TryParse(depositTransactionItem.Amount, out decimal amount);
                yatirimTalepItem.Tutar = amount;

                yatirimTalepItem.WebSiteKomisyon = GetWebSiteYatirimKomisyon(webSiteItem.YaririmKomisyonOrani, yatirimTalepItem.Tutar);
                yatirimTalepItem.Toplam = yatirimTalepItem.Tutar - yatirimTalepItem.WebSiteKomisyon;
                yatirimTalepItem.OnayTarihi = DateTime.Now;
                yatirimTalepItem.SilindiMi = false;
                yatirimTalepItem.KayitTarihi = DateTime.Now;
                yatirimTalepItem.IsSendToWebSite = false;

                var insertedId = _yatirimTalepleriRepository.Insert(yatirimTalepItem);

                var output = new CallbackCreateResponse();

                try
                {
                    var depositPostData = GenerateDepositCallBackDto(depositTransactionItem, yatirimTalepItem);
                    var content = await PostDepositCallbackAsync(depositPostData, webSiteItem.DepositCallbackUrl);

                    _yatirimTalepleriRepository.UpdateWebSiteCallBackResult(insertedId, content);
                    _depositTransactionRepository.UpdateIsCompleted(depositTransactionItem.Id);

                    output.Success = true;
                    output.Status = 200;
                }
                catch (Exception ex)
                {
                    _yatirimTalepleriRepository.UpdateWebSiteCallBackResult(insertedId, ex.Message);

                    output.Success = false;
                    output.Message = $"İşlem sırasında hata oluştu: {ex.Message}";
                    output.Status = 500;
                }

                return Ok(output);
            }
            else if (callbackRequest.type.Equals("withdraw"))
            {
                var withdrawTransactionItem = await _withdrawTransactionRepository.GetByTransactionId(callbackRequest.your_tx_id);
                if (withdrawTransactionItem is null)
                {
                    return BadRequest(new CallbackCreateResponse
                    {
                        Success = false,
                        Message = "Withdraw Transaction not found",
                        Status = 500
                    });
                }

                var webSiteItem = await _webSiteRepository.GetById(withdrawTransactionItem.WebSiteId);

                var cekimTalepItem = new CekimTalepleri
                {
                    Id = Guid.NewGuid(),
                    TalepNo = DateTime.Now.ToString("yyyyMMdd"),
                    WebSiteId = webSiteItem.Id,
                    WebSiteTransactionId = withdrawTransactionItem.TransactionId,
                    AdSoyad = withdrawTransactionItem.FullName,
                    UserName = withdrawTransactionItem.UserName,
                    UserId = withdrawTransactionItem.UserId,
                };

                _ = decimal.TryParse(withdrawTransactionItem.Amount, out decimal amount);
                cekimTalepItem.Tutar = amount;

                cekimTalepItem.WebSiteKomisyon = GetWebSiteYatirimKomisyon(webSiteItem.CekimKomisyonOrani, cekimTalepItem.Tutar);
                cekimTalepItem.Toplam = cekimTalepItem.Tutar - cekimTalepItem.WebSiteKomisyon;
                cekimTalepItem.Durum = callbackRequest.status.Equals("completed");
                cekimTalepItem.IptalAciklama = callbackRequest.description;
                cekimTalepItem.OnayTarihi = DateTime.Now;
                cekimTalepItem.SilindiMi = false;
                cekimTalepItem.KayitTarihi = DateTime.Now;
                cekimTalepItem.IsSendToWebSite = false;

                var insertedId = _cekimTalepleriRepository.Insert(cekimTalepItem);

                var output = new CallbackCreateResponse();

                try
                {
                    var withdrawalPostData = GenerateWithdrawalCallBackDto(withdrawTransactionItem, cekimTalepItem);
                    var content = await PostWithdrawalCallbackAsync(withdrawalPostData, webSiteItem.WithdrawCallbackUrl);

                    _cekimTalepleriRepository.UpdateWebSiteCallBackResult(insertedId, content);

                    output.Message = string.Empty;
                    output.Success = true;
                    output.Status = 200;
                }
                catch (Exception ex)
                {
                    _cekimTalepleriRepository.UpdateWebSiteCallBackResult(insertedId, ex.Message);

                    output.Success = false;
                    output.Message = $"İşlem sırasında hata oluştu: {ex.Message}";
                    output.Status = 500;
                }

                return Ok(output);
            }
            else
            {
                return BadRequest(new CallbackCreateResponse
                {
                    Success = false,
                    Message = "Invalid Operation",
                    Status = 500
                });
            }
        }

        private bool CheckToken(PaycoCallbackRequest request)
        {
            var secret = _configuration.GetValue<string>("SecretKey");
            return _generalHelper.PaycoValidateToken(request.token, request.cp_tx_id.ToString(), request.amount.ToString(), secret);
        }

        private static string GenerateHash(string TransactionId, string UserId, int Status, string Type)
        {
            using var sha1 = SHA1.Create();
            var hashInput = $"{TransactionId}{UserId}{Status}{Type}";
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private decimal GetWebSiteYatirimKomisyon(double yatirimKomisyonOrani, decimal tutar)
        {
            var yatirimKomisyonu = yatirimKomisyonOrani / 100;

            return tutar * (decimal)yatirimKomisyonu;
        }

        private DepositCallBackDto GenerateDepositCallBackDto(DepositTransaction depositTransaction, YatirimTalepleri yatirimTalepleri)
        {
            var depositPostData = new DepositCallBackDto
            {
                userID = depositTransaction.UserId,
                username = depositTransaction.UserName,
                name = depositTransaction.FullName,
                amount = yatirimTalepleri.Tutar,
                verifiedAmount = yatirimTalepleri.Tutar,
                transaction_id = yatirimTalepleri.WebSiteTransactionId,
                status = 2, //Transaction status (e.g., 1: Pending 2: Approved 3: Rejected)
                type = "Deposit", //Transaction type (e.g., 1: Deposit, 0: Withdrawal)
                payment_method_id = "payco"
            };

            depositPostData.hash = GenerateHash(depositPostData.transaction_id, depositPostData.userID, depositPostData.status, depositPostData.type);

            return depositPostData;
        }

        private async Task<string> PostDepositCallbackAsync(DepositCallBackDto depositPostData, string DepositCallbackUrl)
        {
            var serializerdItem = JsonSerializer.Serialize(depositPostData);
            StringContent stringContent = new(serializerdItem, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(DepositCallbackUrl, stringContent);
            var statusCode = response.IsSuccessStatusCode;
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        private WithdrawalCallBackDto GenerateWithdrawalCallBackDto(WithdrawTransaction withdrawTransaction, CekimTalepleri cekimTalepleri)
        {
            var withdrawalPostData = new WithdrawalCallBackDto
            {
                userID = withdrawTransaction.UserId,
                username = withdrawTransaction.UserName,
                name = withdrawTransaction.FullName,
                amount = cekimTalepleri.Tutar,
                verifiedAmount = cekimTalepleri.Tutar,
                transaction_id = cekimTalepleri.WebSiteTransactionId,
                status = cekimTalepleri.Durum ? 2 : 3, //Transaction status (e.g., 1: Pending 2: Approved 3: Rejected)
                type = "Withdrawal", //Transaction type (e.g., 1: Deposit, 0: Withdrawal)
                payment_method_id = "payco"
            };

            withdrawalPostData.hash = GenerateHash(withdrawalPostData.transaction_id, withdrawalPostData.userID, withdrawalPostData.status, withdrawalPostData.type);

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
    }
}
