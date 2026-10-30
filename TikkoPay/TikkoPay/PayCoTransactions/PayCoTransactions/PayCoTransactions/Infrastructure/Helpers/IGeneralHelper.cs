namespace PayCoTransactions.Infrastructure.Helpers
{
    public interface IGeneralHelper
    {
        bool CheckHash(string hash, string TransactionId, string UserId, string ClientId, string ClientSecret);
        bool CheckHash(string hash, string TransactionId, string UserId, string Amount, string ClientId, string ClientSecret);
        bool PaycoValidateToken(string receivedToken, string transactionId, string amount, string secret);
    }
}
