using Dapper;
using ParaYolCallbacks.Infrastructure.Dtos;
using ParaYolCallbacks.Infrastructure.Repositories.Interfaces;

namespace ParaYolCallbacks.Infrastructure.Repositories.Concretes
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

        public async Task<IEnumerable<TedarikciHavaleBankalariDto>> GetTedarikciHavaleBankalari()
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            select
	                            h.Id,
	                            t.Unvan as Tedarikci,
	                            t.BakiyeLimitor as TedarikciBakiyeLimitor,
	                            b.Baslik as BankaAdi,
	                            h.HesapMinTutar,
	                            h.HesapMaxTutar,
	                            h.HesapSahibi,
	                            h.HesapNo
                            from HavaleBankalari h
                            left join Bankalar b on b.Id = h.BankaId
                            inner join Tedarikciler t on t.Id = h.TedarikciId
                            where
	                            h.Durum = 1
                            and h.SilindiMi = 0
                            and t.Durum = 1
                            and t.SilindiMi = 0
                            and h.BankaId in ('7B45E338-626E-47A0-A705-ACC93B24EEAB', '439BA9A5-4CEE-4744-9032-B6F03EE34000')
                            order by t.Unvan, h.HesapMinTutar                             
                            ";

            var parameters = new DynamicParameters();

            return await connection.QueryAsync<TedarikciHavaleBankalariDto>(query, parameters);
        }

        public async Task<IEnumerable<TedarikciDto>> GetTedarikciler()
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            select
	                            t.Id,
	                            t.Unvan
                            from Tedarikciler t
                            where
	                            t.Durum = 1
                            and t.SilindiMi = 0
                            order by t.Unvan                      
                            ";

            var parameters = new DynamicParameters();
            return await connection.QueryAsync<TedarikciDto>(query, parameters);
        }

        public async Task<decimal> GetTedarikciBakiye(Guid tedarikciId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = "SELECT dbo.GetTedarikciBakiye(@Id)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", tedarikciId);

            return await connection.QuerySingleAsync<decimal>(query, parameters);
        }
    }
}
