using Dapper;
using NorthHavaleTransactions.Infrastructure.Dtos;
using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Infrastructure.Helpers;
using NorthHavaleTransactions.Infrastructure.Repositories.Interfaces;

namespace NorthHavaleTransactions.Infrastructure.Repositories.Concretes
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
            var tryParseResult = Int64.TryParse(webSiteTransactionId, out var transactionIntValue);

            if (tryParseResult)
            {
                using var connection = _dapperContext.CreateConnection();
                string query = @"
                            SELECT 
                                COUNT(*)
                            FROM YatirimCekimTalepleri WITH (NOLOCK)
                            WHERE 
                                    WebSiteTransactionIdBigIntValue = @WebSiteTransactionIdBigIntValue
                                AND Durum != @DurumBekliyor
                                AND SilindiMi = 0";

                var parameters = new DynamicParameters();
                parameters.Add("@WebSiteTransactionIdBigIntValue", transactionIntValue);
                parameters.Add("@DurumBekliyor", (int)Durum.Bekliyor);

                return await connection.ExecuteScalarAsync<int>(query, parameters);
            }
            else
            {
                using var connection = _dapperContext.CreateConnection();
                string query = @"
                            SELECT 
                                COUNT(*)
                            FROM YatirimCekimTalepleri WITH (NOLOCK)
                            WHERE 
                                    WebSiteTransactionId = @WebSiteTransactionId
                                AND Durum != @DurumBekliyor
                                AND SilindiMi = 0";

                var parameters = new DynamicParameters();
                parameters.Add("@WebSiteTransactionId", webSiteTransactionId);
                parameters.Add("@DurumBekliyor", (int)Durum.Bekliyor);

                return await connection.ExecuteScalarAsync<int>(query, parameters);
            }
        }

        public async Task<YatirimCekimTalepleri> GetById(Guid id)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            SELECT 
                                top 1 *
                            FROM YatirimCekimTalepleri WITH (NOLOCK)
                            WHERE 
                                    Id = @id
                                AND SilindiMi = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@id", id);

            return await connection.QueryFirstOrDefaultAsync<YatirimCekimTalepleri>(query, parameters);
        }

        public async Task<IEnumerable<YatirimCekimTalepleri>> GetCekimTalepleriByTopId(Guid topId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            SELECT 
                                Id,
                                Durum
                            FROM YatirimCekimTalepleri WITH (NOLOCK)
                            WHERE 
                                    TopId = @topId
                                AND IslemTipi = 2
                                AND SilindiMi = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@topId", topId);

            return await connection.QueryAsync<YatirimCekimTalepleri>(query, parameters);
        }

        public async Task<YatirimCekimTalepleri> GetCekimTalepleriByTedarikciAndWebSiteTransactionId(Guid tedarikciId, string webSiteTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            SELECT 
                                top 1 *
                            FROM YatirimCekimTalepleri WITH (NOLOCK)
                            WHERE 
                                    TedarikciId = @tedarikciId
                                AND WebSiteTransactionId = @WebSiteTransactionId
                                AND IslemTipi = 2
                                AND SilindiMi = 0";

            var parameters = new DynamicParameters();
            parameters.Add("@tedarikciId", tedarikciId);
            parameters.Add("@WebSiteTransactionId", webSiteTransactionId);

            return await connection.QueryFirstOrDefaultAsync<YatirimCekimTalepleri>(query, parameters);
        }

        public async Task<YatirimCekimTalepleri> GetYatirimTalepleriByTedarikciAndWebSiteTransactionId(Guid tedarikciID, string webSiteTransactionId)
        {
            var tryParseResult = Int64.TryParse(webSiteTransactionId, out var transactionIntValue);

            if (tryParseResult)
            {
                using var connection = _dapperContext.CreateConnection();
                string query = @"
                            SELECT 
                                top 1 *
                            FROM YatirimCekimTalepleri WITH (NOLOCK)
                            WHERE 
                                    WebSiteTransactionIdBigIntValue = @WebSiteTransactionIdBigIntValue
                                AND TedarikciId = @tedarikciId
                                AND IslemTipi = 1
                                AND SilindiMi = 0";

                var parameters = new DynamicParameters();
                parameters.Add("@WebSiteTransactionIdBigIntValue", transactionIntValue);
                parameters.Add("@tedarikciId", tedarikciID);

                return await connection.QueryFirstOrDefaultAsync<YatirimCekimTalepleri>(query, parameters);
            }
            else
            {
                using var connection = _dapperContext.CreateConnection();
                string query = @"
                            SELECT 
                                top 1 *
                            FROM YatirimCekimTalepleri WITH (NOLOCK)
                            WHERE 
                                    WebSiteTransactionId = @WebSiteTransactionId
                                AND TedarikciId = @tedarikciId
                                AND IslemTipi = 1
                                AND SilindiMi = 0";

                var parameters = new DynamicParameters();
                parameters.Add("@WebSiteTransactionId", webSiteTransactionId);
                parameters.Add("@tedarikciId", tedarikciID);

                return await connection.QueryFirstOrDefaultAsync<YatirimCekimTalepleri>(query, parameters);
            }
        }

        public async Task<YatirimCekimTalepleri> GetPendingYatirimTalepleri(string webSiteTransactionId)
        {
            var tryParseResult = Int64.TryParse(webSiteTransactionId, out var transactionIntValue);

            if (tryParseResult)
            {
                using var connection = _dapperContext.CreateConnection();
                string query = @"
                            SELECT 
                                top 1 *
                            FROM YatirimCekimTalepleri WITH (NOLOCK)
                            WHERE 
                                    WebSiteTransactionIdBigIntValue = @WebSiteTransactionIdBigIntValue
                                AND Durum = @DurumBekliyor
                                AND SilindiMi = 0";

                var parameters = new DynamicParameters();
                parameters.Add("@WebSiteTransactionIdBigIntValue", transactionIntValue);
                parameters.Add("@DurumBekliyor", (int)Durum.Bekliyor);

                return await connection.QueryFirstOrDefaultAsync<YatirimCekimTalepleri>(query, parameters);
            }
            else
            {
                using var connection = _dapperContext.CreateConnection();
                string query = @"
                            SELECT 
                                top 1 *
                            FROM YatirimCekimTalepleri WITH (NOLOCK)
                            WHERE 
                                    WebSiteTransactionId = @WebSiteTransactionId
                                AND Durum = @DurumBekliyor
                                AND SilindiMi = 0";

                var parameters = new DynamicParameters();
                parameters.Add("@WebSiteTransactionId", webSiteTransactionId);
                parameters.Add("@DurumBekliyor", (int)Durum.Bekliyor);

                return await connection.QueryFirstOrDefaultAsync<YatirimCekimTalepleri>(query, parameters);
            }   
        }

        public async Task<YatirimCekimTalepleriStatusCheckDto> GetStatusCheckDto(Guid webSiteId, string webSiteTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1 
                                WebSiteTransactionId,
                                Durum
                             from YatirimCekimTalepleri WITH (NOLOCK)
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

        public async Task InsertAsync(YatirimCekimTalepleri yatirimCekimTalepleri)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
insert into YatirimCekimTalepleri
(Id, TalepNo, PaymentMethodId, WebSitePaymentMethodId, TedarikciId, HavaleBankaId, BankaId, WebSiteId, WebSiteTransactionId, WebSiteTransactionIdBigIntValue, DepositTransactionId, AdSoyad, AdSoyadYatirimYapan, UserName, UserId, UserIdBigIntValue, YontemTipi, HesapSahibi, HesapNo, Tutar, TedarikciKomisyon, WebSiteKomisyon, Toplam, IslemTipi, Durum, SilindiMi, KayitTarihi)
values
(@Id, @TalepNo, @PaymentMethodId, @WebSitePaymentMethodId, @TedarikciId, @HavaleBankaId, @BankaId, @WebSiteId, @WebSiteTransactionId, @WebSiteTransactionIdBigIntValue, @DepositTransactionId, @AdSoyad, @AdSoyadYatirimYapan, @UserName, @UserId, @UserIdBigIntValue, @YontemTipi, @HesapSahibi, @HesapNo, @Tutar, @TedarikciKomisyon, @WebSiteKomisyon, @Toplam, @IslemTipi, @Durum, @SilindiMi, @KayitTarihi)
";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@TalepNo", yatirimCekimTalepleri.TalepNo);
            parameters.Add("@PaymentMethodId", Constants.NorthPaymentMethodId);
            parameters.Add("@WebSitePaymentMethodId", yatirimCekimTalepleri.WebSitePaymentMethodId);
            parameters.Add("@TedarikciId", yatirimCekimTalepleri.TedarikciId);
            parameters.Add("@HavaleBankaId", yatirimCekimTalepleri.HavaleBankaId);
            parameters.Add("@BankaId", yatirimCekimTalepleri.BankaId);
            parameters.Add("@WebSiteId", yatirimCekimTalepleri.WebSiteId);
            parameters.Add("@WebSiteTransactionId", yatirimCekimTalepleri.WebSiteTransactionId);
            parameters.Add("@WebSiteTransactionIdBigIntValue", yatirimCekimTalepleri.WebSiteTransactionIdBigIntValue);
            parameters.Add("@DepositTransactionId", yatirimCekimTalepleri.DepositTransactionId);
            parameters.Add("@AdSoyad", yatirimCekimTalepleri.AdSoyad);
            parameters.Add("@AdSoyadYatirimYapan", yatirimCekimTalepleri.AdSoyad);
            parameters.Add("@UserName", yatirimCekimTalepleri.UserName);
            parameters.Add("@UserId", yatirimCekimTalepleri.UserId);
            parameters.Add("@UserIdBigIntValue", yatirimCekimTalepleri.UserIdBigIntValue);
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

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateDurumByIdAsync(Guid id, int durum)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"Update YatirimCekimTalepleri Set Durum=@Durum, OnayTarihi=@OnayTarihi Where Id=@id";

            var parameters = new DynamicParameters();
            parameters.Add("@Durum", durum);
            parameters.Add("@OnayTarihi", DateTime.Now);
            parameters.Add("@id", id);

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateReceiptByIdAsync(Guid id, string receipt)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"Update YatirimCekimTalepleri Set IptalAciklama=@IptalAciklama  Where Id=@id";

            var parameters = new DynamicParameters();
            parameters.Add("@IptalAciklama", receipt);
            parameters.Add("@id", id);

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateDurumByTransactionIdAsync(long transactionId, int durum)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"Update YatirimCekimTalepleri Set Durum=@Durum, OnayTarihi=@OnayTarihi Where WebSiteTransactionId=@WebSiteTransactionId";

            var parameters = new DynamicParameters();
            parameters.Add("@Durum", durum);
            parameters.Add("@OnayTarihi",  DateTime.Now);
            parameters.Add("@WebSiteTransactionId", transactionId);

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateWebsiteCallBackResultByIdAsync(string id, string content)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"Update YatirimCekimTalepleri 
                             Set 
                                IsSendToWebSite = @IsSendToWebSite, 
                                WebSiteCallBackResult = @WebSiteCallBackResult 
                             Where  
                                Id = @id";

            var parameters = new DynamicParameters();
            parameters.Add("@IsSendToWebSite", true);
            parameters.Add("@WebSiteCallBackResult", content);
            parameters.Add("@Id", id);

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateWebsiteCallBackResultByTransactionIdAsync(long transactionId, string content)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"Update YatirimCekimTalepleri 
                             Set 
                                IsSendToWebSite = @IsSendToWebSite, 
                                WebSiteCallBackResult = @WebSiteCallBackResult 
                             Where  
                                WebSiteTransactionIdBigIntValue = @WebSiteTransactionIdBigIntValue";

            var parameters = new DynamicParameters();
            parameters.Add("@IsSendToWebSite", true);
            parameters.Add("@WebSiteCallBackResult", content);
            parameters.Add("@WebSiteTransactionIdBigIntValue", transactionId);

            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateWebsiteCallBackResultByTransactionIdWithTutarAsync(long transactionId, string content, decimal tutar, decimal tedarikciKomisyon, decimal webSiteKomisyon)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"Update YatirimCekimTalepleri 
                             Set 
                                Tutar = @Tutar,
                                TedarikciKomisyon = @TedarikciKomisyon,
                                WebSiteKomisyon = @WebSiteKomisyon,
                                Toplam = @Toplam,
                                IsSendToWebSite = @IsSendToWebSite, 
                                WebSiteCallBackResult = @WebSiteCallBackResult 
                             Where  
                                WebSiteTransactionIdBigIntValue = @WebSiteTransactionIdBigIntValue";

            var parameters = new DynamicParameters();
            parameters.Add("@Tutar", tutar);
            parameters.Add("@TedarikciKomisyon", tedarikciKomisyon);
            parameters.Add("@WebSiteKomisyon", webSiteKomisyon);
            parameters.Add("@Toplam", tutar - tedarikciKomisyon);

            parameters.Add("@IsSendToWebSite", true);
            parameters.Add("@WebSiteCallBackResult", content);
            parameters.Add("@WebSiteTransactionIdBigIntValue", transactionId);

            await connection.ExecuteAsync(query, parameters);
        }
    }
}
