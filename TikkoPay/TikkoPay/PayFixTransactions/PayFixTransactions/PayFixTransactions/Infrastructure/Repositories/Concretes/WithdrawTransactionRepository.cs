using Dapper;
using PayFixTransactions.Infrastructure.Entities;
using PayFixTransactions.Infrastructure.Repositories.Interfaces;
using PayFixTransactions.Models;
using System.Transactions;

namespace PayFixTransactions.Infrastructure.Repositories.Concretes
{
    public class WithdrawTransactionRepository : IWithdrawTransactionRepository
    {
        private readonly DapperContext _dapperContext;

        public WithdrawTransactionRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<WithdrawTransaction> GetByTransactionId(string transactionId)
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

        public void CreateWithdraw(WithdrawCreateRequest request, WebSiteleri webSiteItem)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"insert into WithdrawTransactions
(Id, WebSiteId, UserName, UserId, FullName, TransactionId, Amount, PayfixId, PayfixFullName, ClientId, ClientSecret, Hash, KayitTarihi)
values
(@Id, @WebSiteId, @UserName, @UserId, @FullName, @TransactionId, @Amount, @PayfixId, @PayfixFullName, @ClientId, @ClientSecret, @Hash, @KayitTarihi)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@WebSiteId", webSiteItem.Id);
            parameters.Add("@UserName", request.UserName);
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@FullName", request.FullName);
            parameters.Add("@TransactionId", request.TransactionId);
            parameters.Add("@Amount", request.Amount.ToString());
            parameters.Add("@PayfixId", request.PayfixId);
            parameters.Add("@PayfixFullName", request.PayfixFullName);
            parameters.Add("@ClientId", request.ClientId);
            parameters.Add("@ClientSecret", request.ClientSecret);
            parameters.Add("@Hash", request.Hash);
            parameters.Add("@KayitTarihi", DateTime.Now);

            connection.Execute(query, parameters);
        }

    }

}
