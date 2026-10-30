using Dapper;
using NorthKriptoTransactions.Infrastructure.Dtos;
using NorthKriptoTransactions.Infrastructure.Repositories.Interfaces;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Concretes
{
    public class WebSitePaymentMethodsRepository : IWebSitePaymentMethodsRepository
    {
        private readonly DapperContext _dapperContext;

        public WebSitePaymentMethodsRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<WebSitePaymentMethodsDto> GetBySecretId(string clientId, string clientSecret)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select 
                                top 1 
                                wpm.Id,
                                wpm.WebSiteleriId,
                                wpm.PaymentMethodId,
                                w.YaririmKomisyonOrani,
	                            w.CekimKomisyonOrani,
                                w.BakiyeLimitor,
                                w.MinYatirimTutari
                             from WebSitePaymentMethods wpm
                             inner join WebSiteleri w on w.Id = wpm.WebSiteleriId
                             where 
                                    wpm.ApiClientId = @ApiClientId 
                                and wpm.ApiClientSecret = @ApiClientSecret 
                                and wpm.PaymentMethodId = @PaymentMethodId 
                                and wpm.SilindiMi = 0
                                and w.SilindiMi = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@ApiClientId", clientId);
            parameters.Add("@ApiClientSecret", clientSecret);
            parameters.Add("@PaymentMethodId", Constants.NorthKriptoPaymentMethodId);

            return await connection.QueryFirstOrDefaultAsync<WebSitePaymentMethodsDto>(query, parameters);
        }
    }
}
