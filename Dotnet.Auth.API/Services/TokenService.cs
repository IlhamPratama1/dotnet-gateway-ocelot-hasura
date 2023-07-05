using Dotnet.Auth.API.Entities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dotnet.Auth.API.Services
{
    public static class TokenService
    {
        public static string SECRET_KEY;
        static TokenService()
        {
            SECRET_KEY = Environment.GetEnvironmentVariable("ASPNETCORE_HASURA_SECRET_KEY") ?? "NOT_FOUND";
        }

        public static DirectusToken DecodeDirectusToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            DirectusToken directusToken = new DirectusToken
            {
                id = jwtSecurityToken.Claims.First(claim => claim.Type == "id").Value,
                role = jwtSecurityToken.Claims.First(claim => claim.Type == "role").Value,

                app_access = bool.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "app_access").Value),
                admin_access = bool.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "admin_access").Value),
                exp = long.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "exp").Value),

                iat = jwtSecurityToken.Claims.First(claim => claim.Type == "iat").Value,
                iss = jwtSecurityToken.Claims.First(claim => claim.Type == "iss").Value,
            };
            return directusToken;
        }

        public static HasuraRes GenerateHasuraClaimToken(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer),
                new Claim("https://hasura.io/jwt/claims", JsonConvert.SerializeObject(new HasuraClaim
                {
                    roles = new[] { "admin" },
                    role = "admin",
                    id = userId
                }), JsonClaimValueTypes.Json)
            };

            var refreshClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("user-id", userId)
            };

            string refreshToken = GenerateRefreshToken(refreshClaims);
            string jwtToken = GenerateJwtToken(claims);
            
            HasuraRes hasuraRes = new HasuraRes()
            {
                access_token = jwtToken,
                expires = DateTime.Now.AddMinutes(15).Ticks,
                refresh_token = refreshToken
            };

            return hasuraRes;
        }

        public static string GenerateJwtToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(15),
                claims: claims,
                signingCredentials: signInCred
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public static string GenerateRefreshToken(Claim[] claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = signingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
