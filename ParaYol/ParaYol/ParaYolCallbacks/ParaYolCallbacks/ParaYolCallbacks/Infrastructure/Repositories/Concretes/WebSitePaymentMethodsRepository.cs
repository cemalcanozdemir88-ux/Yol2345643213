using Dapper;
using ParaYolCallbacks.Infrastructure.Entities;
using ParaYolCallbacks.Infrastructure.Repositories.Interfaces;

namespace ParaYolCallbacks.Infrastructure.Repositories.Concretes
{
    public class WebSitePaymentMethodsRepository : IWebSitePaymentMethodsRepository
    {
        private readonly DapperContext _dapperContext;

        public WebSitePaymentMethodsRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<WebSitePaymentMethods> GetByIdAsync(Guid id)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1
                                *
                             from WebSitePaymentMethods
                             where
                                Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            return await connection.QueryFirstOrDefaultAsync<WebSitePaymentMethods>(query, parameters);
        }
    }
}
