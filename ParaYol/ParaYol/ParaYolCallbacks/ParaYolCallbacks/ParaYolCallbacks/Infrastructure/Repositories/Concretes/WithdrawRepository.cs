using Dapper;
using ParaYolCallbacks.Infrastructure.Entities;
using ParaYolCallbacks.Infrastructure.Repositories.Interfaces;

namespace ParaYolCallbacks.Infrastructure.Repositories.Concretes
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
    }
}
