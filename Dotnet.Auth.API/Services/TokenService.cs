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

        public static DirectusToken? DecodeDirectusToken(string? token)
        {
            if (token.IsNullOrEmpty()) return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            DirectusToken directusToken = new DirectusToken
            {
                id = jwtSecurityToken.Claims.First(claim => claim.Type == "id").Value,
                role = jwtSecurityToken.Claims.First(claim => claim.Type == "role").Value,

                app_access = bool.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "app_access").Value),
                admin_access = bool.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "admin_access").Value),
                exp = long.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "exp").Value),

                iat = long.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "iat").Value),
                iss = jwtSecurityToken.Claims.First(claim => claim.Type == "iss").Value,
            };

            return directusToken;
        }

        public static LoginRes? GenerateMultiserviceToken(DirectusToken? token, string? refresh_token)
        {
            if (token == null) return null;

            var claims = new List<Claim>
            {
                new Claim("id", token.id ?? ""),
                new Claim("role", token.role ?? ""),

                new Claim("app_access", token.app_access.ToString(), ClaimValueTypes.Boolean),
                new Claim("admin_access", token.admin_access.ToString(), ClaimValueTypes.Boolean),

                new Claim(JwtRegisteredClaimNames.Iat, token.iat.ToString(), ClaimValueTypes.Integer64),
                new Claim("exp", token.exp.ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Iss, token.iss ?? ""),

                new Claim("https://hasura.io/jwt/claims", JsonConvert.SerializeObject(new HasuraClaim
                {
                    roles = new[] { "admin" },
                    role = "admin",
                    id = token.id
                }), JsonClaimValueTypes.Json)
            };

            var refreshClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, token.id ?? ""),
                new Claim("user-id", token.id ?? "")
            };

            string jwtToken = GenerateJwtToken(claims);
            
            LoginRes hasuraRes = new LoginRes()
            {
                access_token = jwtToken,
                expires = DateTime.Now.AddMinutes(15).Ticks,
                refresh_token = refresh_token
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
    }
}
