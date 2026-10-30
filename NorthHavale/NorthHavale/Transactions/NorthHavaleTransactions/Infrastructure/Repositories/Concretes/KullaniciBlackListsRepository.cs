using Dapper;
using NorthHavaleTransactions.Infrastructure.Repositories.Interfaces;

namespace NorthHavaleTransactions.Infrastructure.Repositories.Concretes
{
    public class KullaniciBlackListsRepository : IKullaniciBlackListsRepository
    {
        private readonly DapperContext _dapperContext;

        public KullaniciBlackListsRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<bool> IsUserInBlacklistAsync(Guid websiteId, string userId)
        {
            _ = long.TryParse(userId, out long userIdBigIntValue);

            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            SELECT 
                                COUNT(*) 
                            FROM KullaniciBlackLists 
                            WHERE 
                                SilindiMi = 0 and 
                                WebSiteId = @WebSiteId and 
                                UserId = @UserId
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@WebSiteId", websiteId);
            parameters.Add("@UserId", userIdBigIntValue);

            var result = await connection.ExecuteScalarAsync<int>(query, parameters);

            return result > 0;
        }
    }
}
