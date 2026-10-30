using Dapper;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Repositories.Interfaces;

namespace Transactions.Infrastructure.Repositories.Concretes
{
    public class WebSiteRepository : IWebSiteRepository
    {
        private readonly DapperContext _dapperContext;

        public WebSiteRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<WebSiteleri> GetById(Guid Id)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select 
                                top 1 
                                Id,
                                WebSiteUrl
                             from WebSiteleri 
                             where 
                                    Id = @Id
                                and SilindiMi = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id);

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

        public async Task<decimal> GetYatirimKomisyonu(Guid webSiteId, decimal tutar)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            SELECT
                                TOP 1
                                YaririmKomisyonOrani
                            FROM WebSiteleri
                            WHERE
                                Id = @Id
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", webSiteId);

            var yatirimKomisyonOrani = await connection.ExecuteScalarAsync<decimal>(query, parameters);
            var yatirimKomisyonu = yatirimKomisyonOrani / 100;

            return tutar * (decimal)yatirimKomisyonu;
        }
    }
}
