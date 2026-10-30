using Dapper;
using Transactions.Infrastructure.Dtos;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Interfaces;

namespace Transactions.Infrastructure.Repositories.Concretes
{
    public class YatirimCekimTalepleriRepository : IYatirimCekimTalepleriRepository
    {
        private readonly DapperContext _dapperContext;

        public YatirimCekimTalepleriRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<int> GetCountByWebSiteTransactionId(string webSiteTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            SELECT 
                                COUNT(*)
                            FROM YatirimCekimTalepleri
                            WHERE 
                                    WebSiteTransactionId = @WebSiteTransactionId
                                AND Durum != @DurumBekliyor
                                AND SilindiMi = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@WebSiteTransactionId", webSiteTransactionId);
            parameters.Add("@DurumBekliyor", (int)Durum.Bekliyor);

            return await connection.ExecuteScalarAsync<int>(query, parameters);
        }

        public async Task<YatirimCekimTalepleriStatusCheckDto> GetStatusCheckDto(Guid webSiteId, string webSiteTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1 
                                WebSiteTransactionId,
                                Durum
                             from YatirimCekimTalepleri
                             where
                                WebSiteId = @WebSiteId and 
                                WebSiteTransactionId = @WebSiteTransactionId and 
                                SilindiMi = 0
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@WebSiteId", webSiteId);
            parameters.Add("@WebSiteTransactionId", webSiteTransactionId);

            return await connection.QueryFirstOrDefaultAsync<YatirimCekimTalepleriStatusCheckDto>(query, parameters);
        }

        public void Insert(YatirimCekimTalepleri yatirimCekimTalepleri)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
insert into YatirimCekimTalepleri
(Id, TalepNo, TedarikciId, HavaleBankaId, BankaId, WebSiteId, WebSiteTransactionId, AdSoyad, UserName, UserId, YontemTipi, HesapSahibi, HesapNo, Tutar, TedarikciKomisyon, WebSiteKomisyon, Toplam, IslemTipi, Durum, SilindiMi, KayitTarihi)
values
(@Id, @TalepNo, @TedarikciId, @HavaleBankaId, @BankaId, @WebSiteId, @WebSiteTransactionId, @AdSoyad, @UserName, @UserId, @YontemTipi, @HesapSahibi, @HesapNo, @Tutar, @TedarikciKomisyon, @WebSiteKomisyon, @Toplam, @IslemTipi, @Durum, @SilindiMi, @KayitTarihi)
";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@TalepNo", yatirimCekimTalepleri.TalepNo);
            parameters.Add("@TedarikciId", yatirimCekimTalepleri.TedarikciId);
            parameters.Add("@HavaleBankaId", yatirimCekimTalepleri.HavaleBankaId);
            parameters.Add("@BankaId", yatirimCekimTalepleri.BankaId);
            parameters.Add("@WebSiteId", yatirimCekimTalepleri.WebSiteId);
            parameters.Add("@WebSiteTransactionId", yatirimCekimTalepleri.WebSiteTransactionId);
            parameters.Add("@AdSoyad", yatirimCekimTalepleri.AdSoyad);
            parameters.Add("@UserName", yatirimCekimTalepleri.UserName);
            parameters.Add("@UserId", yatirimCekimTalepleri.UserId);
            parameters.Add("@YontemTipi", yatirimCekimTalepleri.YontemTipi);
            parameters.Add("@HesapSahibi", yatirimCekimTalepleri.HesapSahibi);
            parameters.Add("@HesapNo", yatirimCekimTalepleri.HesapNo);
            parameters.Add("@Tutar", yatirimCekimTalepleri.Tutar);
            parameters.Add("@TedarikciKomisyon", yatirimCekimTalepleri.TedarikciKomisyon);
            parameters.Add("@WebSiteKomisyon", yatirimCekimTalepleri.WebSiteKomisyon);
            parameters.Add("@Toplam", yatirimCekimTalepleri.Toplam);
            parameters.Add("@IslemTipi", yatirimCekimTalepleri.IslemTipi);
            parameters.Add("@Durum", yatirimCekimTalepleri.Durum);
            parameters.Add("@SilindiMi", yatirimCekimTalepleri.SilindiMi);
            parameters.Add("@KayitTarihi", yatirimCekimTalepleri.KayitTarihi);

            connection.Execute(query, parameters);
        }
    }
}
