using Dapper;
using PayCoTransactions.Infrastructure.Entities;
using PayCoTransactions.Infrastructure.Repositories.Interfaces;
using PayCoTransactions.Models;

namespace PayCoTransactions.Infrastructure.Repositories.Concretes
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

        public async Task<int> Insert(PaycoCallbackRequest request)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
            INSERT INTO CallbackRequests
            (Id, Type, TransactionId, PlayerUsername, Amount, PlayerFullname, Token, CustomerTransactionId, Status, Description, CreatedDate)
            VALUES 
            (@Id, @Type, @TransactionId, @PlayerUsername, @Amount, @PlayerFullname, @Token, @CustomerTransactionId, @Status, @Description, @CreatedDate)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@Type", request.type);
            parameters.Add("@TransactionId", request.cp_tx_id);
            parameters.Add("@PlayerUsername", request.player_username);
            parameters.Add("@Amount", request.amount);
            parameters.Add("@PlayerFullname", request.player_fullname);
            parameters.Add("@Token", request.token);
            parameters.Add("@CustomerTransactionId", request.your_tx_id);
            parameters.Add("@Status", request.status);
            parameters.Add("@Description", request.description);
            parameters.Add("@CreatedDate", DateTime.Now);

            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
