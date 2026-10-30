using Dapper;
using NorthKriptoTransactions.Infrastructure.Dtos;
using NorthKriptoTransactions.Infrastructure.Entities;
using NorthKriptoTransactions.Infrastructure.Repositories.Interfaces;
using NorthKriptoTransactions.Models;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Concretes
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
            parameters.Add("@PaymentMethodId", Constants.NorthKriptoPaymentMethodId);
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
    }
}
