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

        public async Task<IEnumerable<Bankalar>> GetBankListForAmountOverFast(decimal amount)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"
                            SELECT
                                DISTINCT
                                banka.Id,
                                banka.Baslik,
                                banka.Resim
                            FROM HavaleBankalari AS hb
                            INNER JOIN Tedarikciler AS t ON hb.TedarikciId = t.Id
                            INNER JOIN Bankalar as banka on banka.Id = hb.BankaId
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
                                AND dbo.GetTedarikciBakiye(t.Id) < t.BakiyeLimitor
                                AND banka.Id not in ('7B45E338-626E-47A0-A705-ACC93B24EEAB', '439BA9A5-4CEE-4744-9032-B6F03EE34000') --fast banka görünmesin
                            ORDER BY banka.Baslik
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@amount", amount);

            return await connection.QueryAsync<Bankalar>(query, parameters);
        }

        public async Task<AvailableHavaleBankItemDto> GetAvailableHavaleBankItemDto(decimal amount, Guid webSiteId, string bankaId)
        {
            using var connection = _dapperContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@amount", amount);
            parameters.Add("@webSiteId", webSiteId);
            parameters.Add("@bankaId", string.IsNullOrWhiteSpace(bankaId) ? null : bankaId);

            return await connection.QueryFirstOrDefaultAsync<AvailableHavaleBankItemDto>(
                "GetAvailableHavaleBankItemDtoWithWebSite",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
