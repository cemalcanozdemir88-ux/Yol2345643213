using Dapper;
using PayCoTransactions.Infrastructure.Entities;
using PayCoTransactions.Infrastructure.Repositories.Interfaces;
using PayCoTransactions.Models;

namespace PayCoTransactions.Infrastructure.Repositories.Concretes
{
    public class DepositTransactionRepository : IDepositTransactionRepository
    {
        private readonly DapperContext _dapperContext;

        public DepositTransactionRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<DepositTransaction> GetByTransactionId(string transactionId)
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

        public Guid Insert(DepositCreateRequest request, Guid webSiteId)
        {
            using var connection = _dapperContext.CreateConnection();

            var query = @"insert into DepositTransactions
                          (Id, WebSiteId, UserName, UserId, TransactionId, FullName, Amount, ClientId, ClientSecret, Hash, KayitTarihi)
                           OUTPUT INSERTED.Id
                          values 
                          (@Id, @WebSiteId, @UserName, @UserId, @TransactionId, @FullName, @Amount, @ClientId, @ClientSecret, @Hash, @KayitTarihi)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@WebSiteId", webSiteId);
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

        public void UpdateIsCompleted(Guid id)
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"
                    update DepositTransactions set IsCompleted=1 where Id=@id
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@id", id);

            connection.Execute(query, parameters);
        }
    }
}
