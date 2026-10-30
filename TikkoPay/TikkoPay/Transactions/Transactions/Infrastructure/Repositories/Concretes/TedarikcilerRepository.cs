using Dapper;
using Transactions.Infrastructure.Repositories.Interfaces;

namespace Transactions.Infrastructure.Repositories.Concretes
{
    public class TedarikcilerRepository : ITedarikcilerRepository
    {
        private readonly DapperContext _dapperContext;

        public TedarikcilerRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<decimal> GetYatirimKomisyonu(Guid tedarikciId, decimal tutar)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            SELECT
                                TOP 1
                                YaririmKomisyonOrani
                            FROM Tedarikciler
                            WHERE
                                Id = @Id
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", tedarikciId);

            var yatirimKomisyonOrani = await connection.ExecuteScalarAsync<decimal>(query, parameters);
            var yatirimKomisyonu = yatirimKomisyonOrani / 100;

            return tutar * (decimal)yatirimKomisyonu;
        }

        public async Task ResetHesapNoGosterimSayisi()
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"UPDATE Tedarikciler SET HesapNoGosterimSayisi = 0";

            await connection.ExecuteAsync(query);
        }
    }
}
