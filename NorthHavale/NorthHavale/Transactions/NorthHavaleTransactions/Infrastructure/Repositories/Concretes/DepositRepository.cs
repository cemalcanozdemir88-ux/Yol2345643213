using Dapper;
using NorthHavaleTransactions.Infrastructure.Dtos;
using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Infrastructure.Helpers;
using NorthHavaleTransactions.Infrastructure.Repositories.Interfaces;
using NorthHavaleTransactions.Models;

namespace NorthHavaleTransactions.Infrastructure.Repositories.Concretes
{
    public class DepositRepository : IDepositRepository
    {
        private readonly DapperContext _dapperContext;

        public DepositRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<DepositTransaction> GetById(Guid Id)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1
                                Id,
                                WebSiteId,
                                WebSitePaymentMethodId,
                                UserName,
                                UserId,
                                TransactionId,
                                FullName,
                                Amount,
                                IsCompleted,
                                KayitTarihi
                             from DepositTransactions
                             where
                                Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id);

            return await connection.QueryFirstOrDefaultAsync<DepositTransaction>(query, parameters);
        }

        public async Task<DepositTransaction> GetByTransactionIdAsync(string transactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1
                                *
                             from DepositTransactions
                             where
                                TransactionId = @TransactionId";

            var parameters = new DynamicParameters();
            parameters.Add("@TransactionId", transactionId);

            return await connection.QueryFirstOrDefaultAsync<DepositTransaction>(query, parameters);
        }

        public async Task<int> GetPendingTransactionsCountByUserId(Guid WebSiteId, string UserId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"
                            SELECT 
                                COUNT(*)
                            FROM DepositTransactions AS dt
                            INNER JOIN YatirimCekimTalepleri AS yct ON dt.TransactionId = yct.WebSiteTransactionId
                            WHERE 
                                    yct.IslemTipi = @IslemTip
                                AND yct.Durum = @Durum
                                AND yct.SilindiMi = 0
                                AND yct.WebSiteId = @WebSiteId
                                AND dt.UserId = @UserId;
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@IslemTip", (int)IslemTip.Yatirim);
            parameters.Add("@Durum", (int)Durum.Bekliyor);
            parameters.Add("@WebSiteId", WebSiteId);
            parameters.Add("@UserId", UserId);

            return await connection.ExecuteScalarAsync<int>(query, parameters);
        }

        public async Task<DepositTransaction> GetWaitingDepositTransaction(Guid webSiteId, string userId)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"select
	                             top 1
	                             Id,
	                             KayitTarihi
                             from DepositTransactions
                             where
	                             WebSiteId = @WebSiteId and
	                             UserId = @UserId and
	                             IsCompleted = 0
                             order by KayitTarihi desc";

            var parameters = new DynamicParameters();
            parameters.Add("@WebSiteId", webSiteId);
            parameters.Add("@UserId", userId);

            return await connection.QueryFirstOrDefaultAsync<DepositTransaction>(query, parameters);
        }

        public Guid Insert(DepositCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto)
        {
            using var connection = _dapperContext.CreateConnection();

            var query = @"insert into DepositTransactions
                          (Id, WebSiteId, PaymentMethodId, WebSitePaymentMethodId, UserName, UserId, TransactionId, FullName, Amount, ClientId, ClientSecret, Hash, KayitTarihi)
                           OUTPUT INSERTED.Id
                          values 
                          (@Id, @WebSiteId, @PaymentMethodId, @WebSitePaymentMethodId, @UserName, @UserId, @TransactionId, @FullName, @Amount, @ClientId, @ClientSecret, @Hash, @KayitTarihi)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@WebSiteId", webSitePaymentMethodDto.WebSiteleriId);
            parameters.Add("@PaymentMethodId", Constants.NorthPaymentMethodId);
            parameters.Add("@WebSitePaymentMethodId", webSitePaymentMethodDto.Id);
            parameters.Add("@UserName", request.UserName);
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@TransactionId", request.TransactionId);
            parameters.Add("@FullName", request.FullName);
            parameters.Add("@Amount", request.Amount);
            parameters.Add("@ClientId", request.ClientId);
            parameters.Add("@ClientSecret", request.ClientSecret);
            parameters.Add("@Hash", request.Hash);
            parameters.Add("@KayitTarihi", DateTime.Now);

            Guid depositTransactionId = connection.QuerySingle<Guid>(query, parameters);
            return depositTransactionId;
        }

        public async Task<int> UpdateIsCompletedAsync(Guid Id, bool isCompleted)
        {
            using var connection = _dapperContext.CreateConnection();

            var query = "Update DepositTransactions Set IsCompleted=@IsCompleted Where Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id);
            parameters.Add("@IsCompleted", isCompleted);

            var result = await connection.ExecuteAsync(query, parameters);
            return result;
        }
    }
}
