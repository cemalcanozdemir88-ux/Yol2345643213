using Dapper;
using ParaYolCallbacks.Infrastructure.Dtos;
using ParaYolCallbacks.Infrastructure.Entities;
using ParaYolCallbacks.Infrastructure.Helpers;
using ParaYolCallbacks.Infrastructure.Repositories.Interfaces;

namespace ParaYolCallbacks.Infrastructure.Repositories.Concretes
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

        public async Task<YatirimCekimTalepleri> GetByWebSiteTransactionId(Guid tedarikciId, string webSiteTransactionId)
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
                                    TedarikciId = @TedarikciId
                                AND WebSiteTransactionIdBigIntValue = @WebSiteTransactionIdBigIntValue
                                AND SilindiMi = 0";

                var parameters = new DynamicParameters();
                parameters.Add("@TedarikciId", tedarikciId);
                parameters.Add("@WebSiteTransactionIdBigIntValue", transactionIntValue);

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
                                    TedarikciId = @TedarikciId
                                AND WebSiteTransactionId = @WebSiteTransactionId
                                AND SilindiMi = 0";

                var parameters = new DynamicParameters();
                parameters.Add("@TedarikciId", tedarikciId);
                parameters.Add("@WebSiteTransactionId", webSiteTransactionId);

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
            parameters.Add("@OnayTarihi", DateTime.Now);
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
                                Id = @Id";

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
