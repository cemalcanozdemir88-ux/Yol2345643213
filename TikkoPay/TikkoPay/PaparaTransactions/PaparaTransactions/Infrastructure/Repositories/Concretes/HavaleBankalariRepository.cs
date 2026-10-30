using Dapper;
using System.Data;
using Transactions.Infrastructure.Dtos;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Repositories.Interfaces;

namespace Transactions.Infrastructure.Repositories.Concretes
{
    public class HavaleBankalariRepository : IHavaleBankalariRepository
    {
        private readonly DapperContext _dapperContext;

        public HavaleBankalariRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<HavaleBankalari> GetById(Guid id)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            SELECT
                                TOP 1
                                Id,
                                TedarikciId,
                                BankaId,
                                HesapSahibi,
                                HesapNo
                            FROM HavaleBankalari
                            WHERE
                                    Id = @Id
                                and Durum = 1
                                and SilindiMi = 0
                            ";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            return await connection.QueryFirstOrDefaultAsync<HavaleBankalari>(query, parameters);
        }

        public async Task<AvailableHavaleBankItemDto> GetAvailableHavaleBankItemDto(decimal amount)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"
                            SELECT 
                                TOP 1
                                hb.Id,
                                hb.HesapSahibi,
                                hb.HesapNo
                            FROM HavaleBankalari AS hb
                            INNER JOIN Tedarikciler AS t ON hb.TedarikciId = t.Id
                            WHERE 
                                    hb.HesapMinTutar <= @amount
                                AND hb.HesapMaxTutar >= @amount
                                AND hb.OnaylananYatirimAdedi < hb.MaxYatirimAdedi
                                AND hb.OnaylananYatirimToplami < hb.MaxYatirimToplami
                                AND t.YatirimPuani > 0
                                AND t.BankaHavale = 1
                                AND t.Durum = 1
                                AND t.SilindiMi = 0
                                AND hb.Durum = 1
                                AND hb.SilindiMi = 0
                                AND dbo.GetTedarikciBakiye(t.Id) < 0
                            ORDER BY hb.OnaylananYatirimAdediTemp, t.YatirimPuani desc, hb.OnaylananYatirimAdedi
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@amount", amount);

            return await connection.QueryFirstOrDefaultAsync<AvailableHavaleBankItemDto>(query, parameters);
        }
    }
}
