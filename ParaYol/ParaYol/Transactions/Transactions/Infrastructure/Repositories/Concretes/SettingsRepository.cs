using Dapper;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Repositories.Interfaces;

namespace Transactions.Infrastructure.Repositories.Concretes
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly DapperContext _dapperContext;

        public SettingsRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<Settings> GetFirst()
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                               SELECT
                                    top 1 *
                                FROM Settings
                            ";

            return await connection.QueryFirstOrDefaultAsync<Settings>(query);
        }

        public async Task<decimal> GetMaxFastTutar()
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                               SELECT
                                    TOP 1 ISNULL(MaxFastTutar, 0) AS MaxFastTutar
                                FROM Settings
                            ";

            return await connection.QueryFirstOrDefaultAsync<decimal>(query);
        }

        public async Task<Guid> GetAsPayDefaultFastHavaleBankaIdAsync()
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                               SELECT
                                    TOP 1 AsPayDefaultFastHavaleBankaId
                                FROM Settings
                            ";

            return await connection.QueryFirstOrDefaultAsync<Guid>(query);
        }

        public async Task SaveAsPayTokenAsync(Guid id, string token, DateTime expireDate)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"Update Settings Set AsPayToken=@Token, AsPayTokenExpiresAt=@ExpiresAt Where Id=@id";

            var parameters = new DynamicParameters();
            parameters.Add("@Token", token);
            parameters.Add("@ExpiresAt", expireDate);
            parameters.Add("@id", id);

            await connection.ExecuteAsync(query, parameters);
        }
    }
}
