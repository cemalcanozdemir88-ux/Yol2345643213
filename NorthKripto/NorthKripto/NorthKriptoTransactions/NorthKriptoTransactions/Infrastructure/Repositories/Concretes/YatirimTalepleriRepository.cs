using Dapper;
using NorthKriptoTransactions.Infrastructure.Dtos;
using NorthKriptoTransactions.Infrastructure.Entities;
using NorthKriptoTransactions.Infrastructure.Repositories.Interfaces;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Concretes
{
    public class YatirimTalepleriRepository : IYatirimTalepleriRepository
    {
        private readonly DapperContext _dapperContext;

        public YatirimTalepleriRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<YatirimTalepleri> GetByTransactionIdAsync(string webSiteTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1 
                                *
                             from YatirimTalepleri WITH (NOLOCK)
                             where
                                WebSiteTransactionId = @WebSiteTransactionId and 
                                SilindiMi = 0
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@WebSiteTransactionId", webSiteTransactionId);

            return await connection.QueryFirstOrDefaultAsync<YatirimTalepleri>(query, parameters);
        }

        public async Task<YatirimTalepleriStatusCheckDto> GetStatusCheckDtoAsync(Guid webSiteId, string webSiteTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1 
                                WebSiteTransactionId,
                                Durum
                             from YatirimTalepleri WITH (NOLOCK)
                             where
                                WebSiteId = @WebSiteId and 
                                WebSiteTransactionId = @WebSiteTransactionId and 
                                SilindiMi = 0
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@WebSiteId", webSiteId);
            parameters.Add("@WebSiteTransactionId", webSiteTransactionId);

            return await connection.QueryFirstOrDefaultAsync<YatirimTalepleriStatusCheckDto>(query, parameters);
        }
    }
}
