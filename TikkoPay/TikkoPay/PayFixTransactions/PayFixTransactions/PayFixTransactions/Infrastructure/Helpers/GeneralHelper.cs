using System.Security.Cryptography;
using System.Text;

namespace PayFixTransactions.Infrastructure.Helpers
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

        public bool PayfixValidateToken(string receivedToken, string transactionId, string amount, string secret)
        {
            string expectedToken = PayfixGenerateMd5Hash(transactionId, amount, secret);
            return receivedToken.Equals(expectedToken, StringComparison.OrdinalIgnoreCase);
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

        private string PayfixGenerateMd5Hash(string transactionId, string amount, string secret)
        {
            string input = $"{transactionId}{amount}{secret}";

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
