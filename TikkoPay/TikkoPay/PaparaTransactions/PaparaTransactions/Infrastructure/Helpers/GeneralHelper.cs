using System.Security.Cryptography;
using System.Text;

namespace Transactions.Infrastructure.Helpers
{
    public class GeneralHelper : IGeneralHelper
    {
        public bool CheckHash(string hash, string TransactionId, string UserId, string ClientId, string ClientSecret)
        {
            string generatedHash = GenerateHash(TransactionId, UserId, ClientId, ClientSecret);
            return hash == generatedHash;
        }

        public bool CheckHash(string hash, string TransactionId, string UserId, string Amount, string ClientId, string ClientSecret)
        {
            string generatedHash = GenerateHash(TransactionId, UserId, Amount, ClientId, ClientSecret);
            return hash == generatedHash;
        }

        private string GenerateHash(string TransactionId, string UserId, string ClientId, string ClientSecret)
        {
            using (var sha1 = SHA1.Create())
            {
                var hashInput = $"{TransactionId}{UserId}{ClientId}{ClientSecret}";
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private string GenerateHash(string TransactionId, string UserId, string Amount, string ClientId, string ClientSecret)
        {
            using (var sha1 = SHA1.Create())
            {
                var hashInput = $"{TransactionId}{UserId}{Amount}{ClientId}{ClientSecret}";
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
