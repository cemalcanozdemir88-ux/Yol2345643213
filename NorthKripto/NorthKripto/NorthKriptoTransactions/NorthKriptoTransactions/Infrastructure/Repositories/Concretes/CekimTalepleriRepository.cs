using Dapper;
using NorthKriptoTransactions.Infrastructure.Dtos;
using NorthKriptoTransactions.Infrastructure.Entities;
using NorthKriptoTransactions.Infrastructure.Repositories.Interfaces;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Concretes
{
    public class CekimTalepleriRepository : ICekimTalepleriRepository
    {
        private readonly DapperContext _dapperContext;

        public CekimTalepleriRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<CekimTalepleri> GetByTransactionIdAsync(string webSiteTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1 
                                *
                             from CekimTalepleri WITH (NOLOCK)
                             where
                                WebSiteTransactionId = @WebSiteTransactionId and 
                                SilindiMi = 0
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@WebSiteTransactionId", webSiteTransactionId);

            return await connection.QueryFirstOrDefaultAsync<CekimTalepleri>(query, parameters);
        }

        public async Task<CekimTalepleriStatusCheckDto> GetStatusCheckDtoAsync(Guid webSiteId, string webSiteTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1 
                                WebSiteTransactionId,
                                Durum
                             from CekimTalepleri WITH (NOLOCK)
                             where
                                WebSiteId = @WebSiteId and 
                                WebSiteTransactionId = @WebSiteTransactionId and 
                                SilindiMi = 0
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@WebSiteId", webSiteId);
            parameters.Add("@WebSiteTransactionId", webSiteTransactionId);

            return await connection.QueryFirstOrDefaultAsync<CekimTalepleriStatusCheckDto>(query, parameters);
        }
    }
}
