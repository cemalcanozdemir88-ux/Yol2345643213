using Dapper;
using PayCoTransactions.Infrastructure.Entities;
using PayCoTransactions.Infrastructure.Repositories.Interfaces;
using PayCoTransactions.Models;

namespace PayCoTransactions.Infrastructure.Repositories.Concretes
{
    public class YatirimTalepleriRepository : IYatirimTalepleriRepository
    {
        private readonly DapperContext _dapperContext;

        public YatirimTalepleriRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<YatirimTalepleriStatusCheckDto> GetStatusCheckDto(Guid webSiteId, string webSiteTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1 
                                Id,
                                WebSiteTransactionId
                             from YatirimTalepleri
                             where
                                WebSiteId = @WebSiteId and 
                                WebSiteTransactionId = @WebSiteTransactionId and 
                                SilindiMi = 0
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@WebSiteId", webSiteId);
            parameters.Add("@WebSiteTransactionId", webSiteTransactionId);

            return await connection.QueryFirstOrDefaultAsync<YatirimTalepleriStatusCheckDto>(query, parameters);
        }

        public Guid Insert(YatirimTalepleri yatirimTalepleri)
        {
            using var connection = _dapperContext.CreateConnection();

            string sql = @"
                INSERT INTO YatirimTalepleri (
                    Id, 
                    TalepNo, 
                    WebSiteId, 
                    WebSiteTransactionId, 
                    AdSoyad, 
                    UserName, 
                    UserId, 
                    Tutar, 
                    WebSiteKomisyon, 
                    Toplam, 
                    IptalAciklama, 
                    OnayTarihi, 
                    SilindiMi, 
                    KayitTarihi, 
                    KaydedenKullaniciId, 
                    GuncellemeTarihi, 
                    GuncelleyenKullaniciId, 
                    IsSendToWebSite, 
                    WebSiteCallBackResult
                ) 
                OUTPUT INSERTED.Id
                VALUES (
                    @Id, 
                    @TalepNo, 
                    @WebSiteId, 
                    @WebSiteTransactionId, 
                    @AdSoyad, 
                    @UserName, 
                    @UserId, 
                    @Tutar, 
                    @WebSiteKomisyon, 
                    @Toplam, 
                    @IptalAciklama, 
                    @OnayTarihi, 
                    @SilindiMi, 
                    @KayitTarihi, 
                    @KaydedenKullaniciId, 
                    @GuncellemeTarihi, 
                    @GuncelleyenKullaniciId, 
                    @IsSendToWebSite, 
                    @WebSiteCallBackResult
                )";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@TalepNo", yatirimTalepleri.TalepNo);
            parameters.Add("@WebSiteId", yatirimTalepleri.WebSiteId);
            parameters.Add("@WebSiteTransactionId", yatirimTalepleri.WebSiteTransactionId);
            parameters.Add("@AdSoyad", yatirimTalepleri.AdSoyad);
            parameters.Add("@UserName", yatirimTalepleri.UserName);
            parameters.Add("@UserId", yatirimTalepleri.UserId);
            parameters.Add("@Tutar", yatirimTalepleri.Tutar);
            parameters.Add("@WebSiteKomisyon", yatirimTalepleri.WebSiteKomisyon);
            parameters.Add("@Toplam", yatirimTalepleri.Toplam);
            parameters.Add("@IptalAciklama", yatirimTalepleri.IptalAciklama);
            parameters.Add("@OnayTarihi", yatirimTalepleri.OnayTarihi);
            parameters.Add("@SilindiMi", yatirimTalepleri.SilindiMi);
            parameters.Add("@KayitTarihi", yatirimTalepleri.KayitTarihi);
            parameters.Add("@KaydedenKullaniciId", yatirimTalepleri.KaydedenKullaniciId);
            parameters.Add("@GuncellemeTarihi", yatirimTalepleri.GuncellemeTarihi);
            parameters.Add("@GuncelleyenKullaniciId", yatirimTalepleri.GuncelleyenKullaniciId);
            parameters.Add("@IsSendToWebSite", yatirimTalepleri.IsSendToWebSite);
            parameters.Add("@WebSiteCallBackResult", yatirimTalepleri.WebSiteCallBackResult);

            Guid insertedId = connection.QuerySingle<Guid>(sql, parameters);
            return insertedId;
        }

        public void UpdateWebSiteCallBackResult(Guid id, string webSiteCallBackResult)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"
                    update YatirimTalepleri set IsSendToWebSite=1, WebSiteCallBackResult=@webSiteCallBackResult where Id=@id
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@id", id);
            parameters.Add("@webSiteCallBackResult", webSiteCallBackResult);

            connection.Execute(query, parameters);
        }
    }
}
