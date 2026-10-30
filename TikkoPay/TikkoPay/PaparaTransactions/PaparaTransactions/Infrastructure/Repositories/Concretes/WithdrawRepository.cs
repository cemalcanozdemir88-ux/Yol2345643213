using Dapper;
using System.Transactions;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Models;

namespace Transactions.Infrastructure.Repositories.Concretes
{
    public class WithdrawRepository : IWithdrawRepository
    {
        private readonly DapperContext _dapperContext;

        public WithdrawRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public void CreateWithdraw(WithdrawCreateRequest request, WebSiteleri webSiteItem)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                using (var transaction = new TransactionScope())
                {
                    var withDrawCommit = CreateWithdrawTransactionItem(request, webSiteItem);
                    var yatirimCekimCommit = CreateYatirimCekimTalepleri(request, webSiteItem);

                    connection.Execute(withDrawCommit.Query, withDrawCommit.Parameters);
                    connection.Execute(yatirimCekimCommit.Query, yatirimCekimCommit.Parameters);

                    transaction.Complete();
                }
            }

        }

        private CommitModel CreateWithdrawTransactionItem(WithdrawCreateRequest request, WebSiteleri webSiteItem)
        {
            string query = @"insert into WithdrawTransactions
(Id, WebSiteId, UserName, UserId, FullName, TransactionId, Amount, Iban, ClientId, ClientSecret, Hash, KayitTarihi)
values
(@Id, @WebSiteId, @UserName, @UserId, @FullName, @TransactionId, @Amount, @Iban, @ClientId, @ClientSecret, @Hash, @KayitTarihi)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@WebSiteId", webSiteItem.Id);
            parameters.Add("@UserName", request.UserName);
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@FullName", request.FullName);
            parameters.Add("@TransactionId", request.TransactionId);
            parameters.Add("@Amount", request.Amount.ToString());
            parameters.Add("@Iban", request.Iban);
            parameters.Add("@ClientId", request.ClientId);
            parameters.Add("@ClientSecret", request.ClientSecret);
            parameters.Add("@Hash", request.Hash);
            parameters.Add("@KayitTarihi", DateTime.Now);

            return new CommitModel
            {
                Query = query,
                Parameters = parameters
            };
        }

        private CommitModel CreateYatirimCekimTalepleri(WithdrawCreateRequest request, WebSiteleri webSiteItem)
        {
            string query = @"insert into YatirimCekimTalepleri
(Id, TalepNo, WebSiteId, WebSiteTransactionId, AdSoyad, UserName, UserId, YontemTipi, HesapSahibi, HesapNo, Tutar, TedarikciKomisyon, WebSiteKomisyon, Toplam, IslemTipi, Durum, SilindiMi, KayitTarihi)
values
(@Id, @TalepNo, @WebSiteId, @WebSiteTransactionId, @AdSoyad, @UserName, @UserId, @YontemTipi, @HesapSahibi, @HesapNo, @Tutar, @TedarikciKomisyon, @WebSiteKomisyon, @Toplam, @IslemTipi, @Durum, @SilindiMi, @KayitTarihi)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@TalepNo", DateTime.Now.ToString("yyyyMMdd"));
            parameters.Add("@WebSiteId", webSiteItem.Id);
            parameters.Add("@WebSiteTransactionId", request.TransactionId);
            parameters.Add("@AdSoyad", request.FullName);
            parameters.Add("@UserName", request.UserName);
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@YontemTipi", "Banka Havale");
            parameters.Add("@HesapSahibi", request.FullName);
            parameters.Add("@HesapNo", request.Iban);
            parameters.Add("@Tutar", (decimal)request.Amount);
            parameters.Add("@TedarikciKomisyon", 0);
            parameters.Add("@WebSiteKomisyon", GetWebSiteCekimKomisyon(webSiteItem, (decimal)request.Amount));
            parameters.Add("@Toplam", 0);
            parameters.Add("@IslemTipi", (int)IslemTip.Cekim);
            parameters.Add("@Durum", (int)Durum.Bekliyor);
            parameters.Add("@SilindiMi", false);
            parameters.Add("@KayitTarihi", DateTime.Now);

            return new CommitModel
            {
                Query = query,
                Parameters = parameters
            };
        }

        private decimal GetWebSiteCekimKomisyon(WebSiteleri webSiteItem, decimal tutar)
        {
            var cekimKomisyonOrani = webSiteItem.CekimKomisyonOrani / 100;
            return tutar * (decimal)cekimKomisyonOrani;
        }
    }

    public class CommitModel
    {
        public string Query { get; set; }
        public DynamicParameters Parameters { get; set; }
    }
}
