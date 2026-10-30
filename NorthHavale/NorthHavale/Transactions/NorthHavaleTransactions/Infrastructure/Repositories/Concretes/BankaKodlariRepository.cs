using Dapper;
using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Infrastructure.Repositories.Interfaces;

namespace NorthHavaleTransactions.Infrastructure.Repositories.Concretes
{
    public class BankaKodlariRepository : IBankaKodlariRepository
    {
        private readonly DapperContext _dapperContext;

        public BankaKodlariRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<BankaKodlari> GetByKodAsync(string kod)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select 
                                top 1 
                                b.*
                             from BankaKodlari b
                             where 
                                b.Kod = @kod";

            var parameters = new DynamicParameters();
            parameters.Add("@kod", kod);

            return await connection.QueryFirstOrDefaultAsync<BankaKodlari>(query, parameters);
        }
    }
}
