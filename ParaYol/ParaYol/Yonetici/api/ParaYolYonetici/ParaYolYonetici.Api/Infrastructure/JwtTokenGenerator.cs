using ParaYolYonetici.Core.DTOs.Kullanicilar;
using ParaYolYonetici.Core.DTOs.KullaniciYetkileri;
using ParaYolYonetici.Core.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ParaYolYonetici.Api.Infrastructure
{
    public class JwtTokenGenerator
    {
        public static JwtTokenResponse GenerateToken(KullanicilarListDto user, List<KullaniciYetkileriListDto> kullaniciYetkileri, JwtTokenSettings jwtTokenSettings)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSettings.SecurityKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, ((int)user.KullaniciTipi).ToString()));
            claims.Add(new Claim("role", ((int)user.KullaniciTipi).ToString()));
            claims.Add(new Claim("name", user.AdSoyad));
            claims.Add(new Claim("tedarikciId", user.TedarikciId.ToString()));
            claims.Add(new Claim("websiteId", user.WebSiteId.ToString()));
            claims.Add(new Claim("affiliateId", user.AffiliateId.ToString()));
            claims.Add(new Claim("nameidentifier", user.Id.ToString()));

            foreach (var yetki in kullaniciYetkileri.Where(k => k.KullaniciYetkiliMi))
            {
                claims.Add(new Claim("roles", yetki.YetkiId.ToString()));
            }

            var notBefore = DateTime.UtcNow;
            var expireDate = notBefore.AddDays(jwtTokenSettings.AccessTokenExpiration);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtTokenSettings.Issuer,
                audience: jwtTokenSettings.Audience,
                claims: claims,
                notBefore: notBefore,
                expires: expireDate,
                signingCredentials: credentials
            );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return new JwtTokenResponse(handler.WriteToken(token), expireDate);
        }

    }
}
