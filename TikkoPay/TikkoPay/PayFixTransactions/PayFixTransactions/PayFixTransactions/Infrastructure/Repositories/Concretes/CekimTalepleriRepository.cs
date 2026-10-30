using Dapper;
using PayFixTransactions.Infrastructure.Entities;
using PayFixTransactions.Infrastructure.Repositories.Interfaces;

namespace PayFixTransactions.Infrastructure.Repositories.Concretes
{
    public class CekimTalepleriRepository : ICekimTalepleriRepository
    {
        private readonly DapperContext _dapperContext;

        public CekimTalepleriRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public Guid Insert(CekimTalepleri cekimTalepleri)
        {
            using var connection = _dapperContext.CreateConnection();

            string sql = @"
                INSERT INTO CekimTalepleri (
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
                    Durum, 
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
                    @Durum, 
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
            parameters.Add("@TalepNo", cekimTalepleri.TalepNo);
            parameters.Add("@WebSiteId", cekimTalepleri.WebSiteId);
            parameters.Add("@WebSiteTransactionId", cekimTalepleri.WebSiteTransactionId);
            parameters.Add("@AdSoyad", cekimTalepleri.AdSoyad);
            parameters.Add("@UserName", cekimTalepleri.UserName);
            parameters.Add("@UserId", cekimTalepleri.UserId);
            parameters.Add("@Tutar", cekimTalepleri.Tutar);
            parameters.Add("@WebSiteKomisyon", cekimTalepleri.WebSiteKomisyon);
            parameters.Add("@Toplam", cekimTalepleri.Toplam);
            parameters.Add("@Durum", cekimTalepleri.Durum);
            parameters.Add("@IptalAciklama", cekimTalepleri.IptalAciklama);
            parameters.Add("@OnayTarihi", cekimTalepleri.OnayTarihi);
            parameters.Add("@SilindiMi", cekimTalepleri.SilindiMi);
            parameters.Add("@KayitTarihi", cekimTalepleri.KayitTarihi);
            parameters.Add("@KaydedenKullaniciId", cekimTalepleri.KaydedenKullaniciId);
            parameters.Add("@GuncellemeTarihi", cekimTalepleri.GuncellemeTarihi);
            parameters.Add("@GuncelleyenKullaniciId", cekimTalepleri.GuncelleyenKullaniciId);
            parameters.Add("@IsSendToWebSite", cekimTalepleri.IsSendToWebSite);
            parameters.Add("@WebSiteCallBackResult", cekimTalepleri.WebSiteCallBackResult);

            Guid insertedId = connection.QuerySingle<Guid>(sql, parameters);
            return insertedId;
        }

        public void UpdateWebSiteCallBackResult(Guid id, string webSiteCallBackResult)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"
                    update CekimTalepleri set IsSendToWebSite=1, WebSiteCallBackResult=@webSiteCallBackResult where Id=@id
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@id", id);
            parameters.Add("@webSiteCallBackResult", webSiteCallBackResult);

            connection.Execute(query, parameters);
        }
    }
}
