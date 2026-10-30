namespace ParaYolYonetici.Api.Infrastructure
{
    public class JwtTokenResponse
    {
        public string Token { get; set; }

        public DateTime ExpireDate { get; set; }

        public JwtTokenResponse(string token, DateTime expireDateTime)
        {
            Token = token;
            ExpireDate = expireDateTime;
        }
    }
}
