using Dapper;
using System.Transactions;
using Transactions.Infrastructure.Dtos;
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

        public async Task<WithdrawTransaction> GetById(Guid Id)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1
                                *
                             from WithdrawTransactions
                             where
                                Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id);

            return await connection.QueryFirstOrDefaultAsync<WithdrawTransaction>(query, parameters);
        }

        public async Task<WithdrawTransaction> GetByTransactionIdAsync(string transactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1
                                *
                             from WithdrawTransactions
                             where
                                TransactionId = @TransactionId";

            var parameters = new DynamicParameters();
            parameters.Add("@TransactionId", transactionId);

            return await connection.QueryFirstOrDefaultAsync<WithdrawTransaction>(query, parameters);
        }

        public void CreateWithdraw(WithdrawCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto, Guid? tedarikciId, string bankName)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                using (var transaction = new TransactionScope())
                {
                    var withdrawTransactionId = Guid.NewGuid();

                    var withDrawCommit = CreateWithdrawTransactionItem(request, webSitePaymentMethodDto, withdrawTransactionId);
                    var yatirimCekimCommit = CreateYatirimCekimTalepleri(request, webSitePaymentMethodDto, tedarikciId, withdrawTransactionId, bankName);

                    connection.Execute(withDrawCommit.Query, withDrawCommit.Parameters);
                    connection.Execute(yatirimCekimCommit.Query, yatirimCekimCommit.Parameters);

                    transaction.Complete();
                }
            }
        }

        private CommitModel CreateWithdrawTransactionItem(WithdrawCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto, Guid withdrawTransactionId)
        {
            string query = @"insert into WithdrawTransactions
(Id, WebSiteId, PaymentMethodId, WebSitePaymentMethodId, UserName, UserId, FullName, TransactionId, Amount, Iban, ClientId, ClientSecret, Hash, KayitTarihi)
values
(@Id, @WebSiteId, @PaymentMethodId, @WebSitePaymentMethodId, @UserName, @UserId, @FullName, @TransactionId, @Amount, @Iban, @ClientId, @ClientSecret, @Hash, @KayitTarihi)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", withdrawTransactionId);
            parameters.Add("@WebSiteId", webSitePaymentMethodDto.WebSiteleriId);
            parameters.Add("@PaymentMethodId", Constants.TikkoPaymentMethodId);
            parameters.Add("@WebSitePaymentMethodId", webSitePaymentMethodDto.Id);
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

        private CommitModel CreateYatirimCekimTalepleri(WithdrawCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto, Guid? tedarikciId, Guid withdrawTransactionId, string bankName)
        {
            string query = @"insert into YatirimCekimTalepleri
(Id, TalepNo, PaymentMethodId, WebSitePaymentMethodId, TedarikciId, WebSiteId, WebSiteTransactionId, WebSiteTransactionIdBigIntValue, WithdrawTransactionId, AdSoyad, UserName, UserId, UserIdBigIntValue, YontemTipi, HesapSahibi, HesapNo, HesapNoBanka, Tutar, TedarikciKomisyon, WebSiteKomisyon, Toplam, IslemTipi, Durum, SilindiMi, KayitTarihi)
values
(@Id, @TalepNo, @PaymentMethodId, @WebSitePaymentMethodId, @TedarikciId, @WebSiteId, @WebSiteTransactionId, @WebSiteTransactionIdBigIntValue, @WithdrawTransactionId, @AdSoyad, @UserName, @UserId, @UserIdBigIntValue, @YontemTipi, @HesapSahibi, @HesapNo, @HesapNoBanka, @Tutar, @TedarikciKomisyon, @WebSiteKomisyon, @Toplam, @IslemTipi, @Durum, @SilindiMi, @KayitTarihi)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@TalepNo", DateTime.Now.ToString("yyyyMMdd"));
            parameters.Add("@PaymentMethodId", Constants.TikkoPaymentMethodId);
            parameters.Add("@WebSitePaymentMethodId", webSitePaymentMethodDto.Id);
            parameters.Add("@TedarikciId", tedarikciId);
            parameters.Add("@WebSiteId", webSitePaymentMethodDto.WebSiteleriId);
            parameters.Add("@WebSiteTransactionId", request.TransactionId);
            parameters.Add("@WebSiteTransactionIdBigIntValue", ParseToLong(request.TransactionId));
            parameters.Add("@WithdrawTransactionId", withdrawTransactionId);
            parameters.Add("@AdSoyad", request.FullName);
            parameters.Add("@UserName", request.UserName);
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@UserIdBigIntValue", ParseToLong(request.UserId));
            parameters.Add("@YontemTipi", "Banka Havale");
            parameters.Add("@HesapSahibi", request.FullName);
            parameters.Add("@HesapNo", request.Iban);
            parameters.Add("@HesapNoBanka", bankName);
            parameters.Add("@Tutar", (decimal)request.Amount);
            parameters.Add("@TedarikciKomisyon", 0);
            parameters.Add("@WebSiteKomisyon", GetWebSiteCekimKomisyon(webSitePaymentMethodDto, (decimal)request.Amount));
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

        private decimal GetWebSiteCekimKomisyon(WebSitePaymentMethodsDto webSitePaymentMethodDto, decimal tutar)
        {
            var cekimKomisyonOrani = webSitePaymentMethodDto.CekimKomisyonOrani / 100;
            return tutar * (decimal)cekimKomisyonOrani;
        }

        public long? ParseToLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (long.TryParse(value, out long result))
            {
                return result;
            }

            return null;
        }
    }

    public class CommitModel
    {
        public string Query { get; set; }
        public DynamicParameters Parameters { get; set; }
    }
}
