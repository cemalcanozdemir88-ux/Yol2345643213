using Dapper;
using PayFixTransactions.Infrastructure.Entities;
using PayFixTransactions.Infrastructure.Repositories.Interfaces;
using PayFixTransactions.Models;

namespace PayFixTransactions.Infrastructure.Repositories.Concretes
{
    public class CallbackRepository : ICallbackRepository
    {
        private readonly DapperContext _dapperContext;

        public CallbackRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<CallbackRequest> GetByCustomerTransactionId(string customerTransactionId)
        {
            using var connection = _dapperContext.CreateConnection();
            string query = @"select
                                top 1 
                                *
                             from CallbackRequests
                             where
                                CustomerTransactionId = @customerTransactionId
                            ";

            var parameters = new DynamicParameters();
            parameters.Add("@customerTransactionId", customerTransactionId);

            return await connection.QueryFirstOrDefaultAsync<CallbackRequest>(query, parameters);
        }

        public async Task<int> Insert(PayFixCallbackRequest request)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
            INSERT INTO CallbackRequests
            (Id, PaymentShortCode, TransactionId, EndUserUsername, FullName, Amount, Token, CustomerTransactionId, Type, Status, Description, CreatedDate)
            VALUES 
            (@Id, @PaymentShortCode, @TransactionId, @EndUserUsername, @FullName, @Amount, @Token, @CustomerTransactionId, @Type, @Status, @Description, @CreatedDate)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@PaymentShortCode", request.payment_short_code);
            parameters.Add("@TransactionId", request.transaction_id);
            parameters.Add("@EndUserUsername", request.enduser_username);
            parameters.Add("@FullName", request.fullName);
            parameters.Add("@Amount", request.amount);
            parameters.Add("@Token", request.token);
            parameters.Add("@CustomerTransactionId", request.customer_transaction_id);
            parameters.Add("@Type", request.type);
            parameters.Add("@Status", request.status);
            parameters.Add("@Description", request.description);
            parameters.Add("@CreatedDate", DateTime.Now);

           return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
