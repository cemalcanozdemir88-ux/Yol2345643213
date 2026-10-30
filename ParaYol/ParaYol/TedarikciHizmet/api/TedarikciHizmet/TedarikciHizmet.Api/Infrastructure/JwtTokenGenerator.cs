using TedarikciHizmet.Core.DTOs.Kullanicilar;
using TedarikciHizmet.Core.DTOs.KullaniciYetkileri;
using TedarikciHizmet.Core.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TedarikciHizmet.Api.Infrastructure
{
    public class JwtTokenGenerator
    {
        public static JwtTokenResponse GenerateToken(KullanicilarListDto user, List<KullaniciYetkileriListDto> kullaniciYetkileri, JwtTokenSettings jwtTokenSettings)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSettings.SecurityKey));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Role, ((int)user.KullaniciTipi).ToString()),
                new Claim("role", ((int)user.KullaniciTipi).ToString()),
                new Claim("name", user.AdSoyad),
                new Claim("tedarikciId", user.TedarikciId.ToString()),
                new Claim("nameidentifier", user.Id.ToString())
            };

            foreach (var yetki in kullaniciYetkileri.Where(k => k.KullaniciYetkiliMi))
            {
                claims.Add(new Claim("roles", yetki.YetkiId.ToString()));
            }

            var notBefore = DateTime.UtcNow;
            var expireDate = notBefore.AddDays(jwtTokenSettings.AccessTokenExpiration);

            JwtSecurityToken token = new(
                issuer: jwtTokenSettings.Issuer,
                audience: jwtTokenSettings.Audience,
                claims: claims,
                notBefore: notBefore,
                expires: expireDate,
                signingCredentials: credentials
            );

            JwtSecurityTokenHandler handler = new();

            return new JwtTokenResponse(handler.WriteToken(token), expireDate);
        }

    }
}
