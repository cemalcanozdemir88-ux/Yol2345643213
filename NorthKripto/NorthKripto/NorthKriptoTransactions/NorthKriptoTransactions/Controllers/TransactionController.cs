using Microsoft.AspNetCore.Mvc;
using NorthKriptoTransactions.Infrastructure.Helpers;
using NorthKriptoTransactions.Infrastructure.Repositories.Concretes;
using NorthKriptoTransactions.Infrastructure.Repositories.Interfaces;
using NorthKriptoTransactions.Models;

namespace NorthKriptoTransactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ICekimTalepleriRepository _cekimTalepleriRepository;
        private readonly IYatirimTalepleriRepository _yatirimTalepleriRepository;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;

        public TransactionController(ICekimTalepleriRepository cekimTalepleriRepository,
                                     IYatirimTalepleriRepository yatirimTalepleriRepository,
                                     IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository)
        {
            _cekimTalepleriRepository = cekimTalepleriRepository;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check(TransactionCheckRequest request)
        {
            var paymentMethod = await _webSitePaymentMethodsRepository.GetBySecretId(request.ClientId, request.ClientSecret);
            if (paymentMethod is null)
            {
                return NotFound(new TransactionCheckResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            async Task<TransactionCheckResponse?> CheckYatirimAsync()
            {
                var item = await _yatirimTalepleriRepository.GetByTransactionIdAsync(request.TransactionId);
                if (item is null) return null;

                var statusDto = await _yatirimTalepleriRepository.GetStatusCheckDtoAsync(paymentMethod.WebSiteleriId, request.TransactionId);
                return statusDto is null ? null : CreateSuccessResponse(statusDto.WebSiteTransactionId, statusDto.Durum);
            }

            async Task<TransactionCheckResponse?> CheckCekimAsync()
            {
                var item = await _cekimTalepleriRepository.GetByTransactionIdAsync(request.TransactionId);
                if (item is null) return null;

                var statusDto = await _cekimTalepleriRepository.GetStatusCheckDtoAsync(paymentMethod.WebSiteleriId, request.TransactionId);
                return statusDto is null ? null : CreateSuccessResponse(statusDto.WebSiteTransactionId, statusDto.Durum);
            }

            var response = await CheckYatirimAsync();
            if (response is not null) return Ok(response);

            response = await CheckCekimAsync();
            if (response is not null) return Ok(response);

            return NotFound(new TransactionCheckResponse
            {
                Success = false,
                Message = "Not found"
            });
        }

        private TransactionCheckResponse CreateSuccessResponse(string transactionId, int durum)
        {
            return new TransactionCheckResponse
            {
                Success = true,
                TransactionId = transactionId,
                Message = GetStatusText(durum)
            };
        }

        private string GetStatusText(int durum)
        {
            return durum switch
            {
                (int)Durum.Bekliyor => "Pending",
                (int)Durum.Onaylandi => "Approved",
                (int)Durum.Red => "Rejected",
                _ => "Status Not Found"
            };
        }
    }
}
