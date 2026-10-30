using Dapper;
using ParaYolCallbacks.Infrastructure.Entities;
using ParaYolCallbacks.Infrastructure.Repositories.Interfaces;

namespace ParaYolCallbacks.Infrastructure.Repositories.Concretes
{
    public class WebSiteRepository : IWebSiteRepository
    {
        private readonly DapperContext _dapperContext;

        public WebSiteRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<WebSiteleri> GetByIdAsync(Guid Id)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select 
                                top 1 
                                Id,
                                Baslik,
                                WebSiteUrl
                             from WebSiteleri 
                             where 
                                    Id = @Id
                                and SilindiMi = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id);

            return await connection.QueryFirstOrDefaultAsync<WebSiteleri>(query, parameters);
        }

        public async Task<WebSiteleri> GetByIdWithAllColumnsAsync(Guid Id)
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
            parameters.Add("@Id", Id);

            return await connection.QueryFirstOrDefaultAsync<WebSiteleri>(query, parameters);
        }

        public async Task<WebSiteleri> GetWebSiteItemAsync(string clientId, string clientSecret)
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

        public async Task<IEnumerable<WebSiteleri>> GetAllAsync()
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select 
                                *
                             from WebSiteleri 
                             where 
                                  SilindiMi = 0
                             order by Baslik";

            var parameters = new DynamicParameters();

            return await connection.QueryAsync<WebSiteleri>(query, parameters);
        }

        public async Task<decimal> GetYatirimKomisyonuAsync(Guid webSiteId, decimal tutar)
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

        public async Task<decimal> GetWebsiteBakiyeAsync(Guid websiteId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = "SELECT dbo.GetWebsiteBakiye(@websiteId)";

            var parameters = new DynamicParameters();
            parameters.Add("@websiteId", websiteId);

            var result = await connection.ExecuteScalarAsync<decimal>(query, parameters);
            return result;
        }
    }
}
