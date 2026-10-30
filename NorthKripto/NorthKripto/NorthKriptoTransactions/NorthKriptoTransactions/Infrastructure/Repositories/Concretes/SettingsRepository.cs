using Dapper;
using NorthKriptoTransactions.Infrastructure.Entities;
using NorthKriptoTransactions.Infrastructure.Repositories.Interfaces;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Concretes
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
    }
}
