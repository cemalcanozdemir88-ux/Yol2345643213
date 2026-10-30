using Dapper;
using PayCoTransactions.Infrastructure.Entities;
using PayCoTransactions.Infrastructure.Repositories.Interfaces;

namespace PayCoTransactions.Infrastructure.Repositories.Concretes
{
    public class WebSiteRepository : IWebSiteRepository
    {
        private readonly DapperContext _dapperContext;

        public WebSiteRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<WebSiteleri> GetById(Guid webSiteId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select 
                                top 1 
                                *
                             from WebSiteleri 
                             where 
                                    Id = @Id
                                and SilindiMi = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", webSiteId);

            return await connection.QueryFirstOrDefaultAsync<WebSiteleri>(query, parameters);
        }

        public async Task<WebSiteleri> GetWebSiteItem(string clientId, string clientSecret)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select 
                                top 1 
                                Id,
                                YaririmKomisyonOrani,
                                CekimKomisyonOrani
                             from WebSiteleri 
                             where 
                                    ApiClientId = @ApiClientId 
                                and ApiClientSecret = @ApiClientSecret 
                                and SilindiMi = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@ApiClientId", clientId);
            parameters.Add("@ApiClientSecret", clientSecret);

            return await connection.QueryFirstOrDefaultAsync<WebSiteleri>(query, parameters);
        }
    }
}
