using Microsoft.AspNetCore.Mvc;
using Dotnet.Auth.API.Entities;
using Dotnet.Auth.API.Services;
using Dotnet.Auth.API.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dotnet.Auth.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> PostLogin([FromBody] LoginDto value)
    {
        try
        {
            Response<LoginRes> res = await HttpRequestService<Response<LoginRes>>.PostAsync("auth/login", value);
            DirectusToken directusToken = TokenService.DecodeDirectusToken(res.data?.access_token ?? "");
            HasuraRes hasuraRes = TokenService.GenerateHasuraClaimToken(directusToken.id ?? "");
            return Ok(hasuraRes);
        }
        catch (ArgumentException ex)
        {
            // Handle ArgumentException
            return BadRequest(new { message = "An argument error occurred: " + ex.Message });
        }
        catch (Exception ex)
        {
            // Handle all other exceptions
            return BadRequest(new { message = "An unknown error occurred: " + ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> PostRegister([FromBody] string value)
    {
        try {
            Response<RegisterRes> res = await HttpRequestService<Response<RegisterRes>>.PostAsync("auth/login", value);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            // Handle ArgumentException
            return BadRequest(new { message = "An argument error occurred: " + ex.Message });
            
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "An unknown error occurred: " + ex.Message });
        }
    }

    [HttpPost("refresh")]
    public  IActionResult RefreshToken([FromBody] RefreshDto res)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenService.SECRET_KEY));

        try
        {
            // Validate the JWT refresh token
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            var principal = tokenHandler.ValidateToken(res.refresh_token, validationParameters, out _);

            // Generate a new access token and refresh token
            string user_id = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            HasuraRes hasuraRes = TokenService.GenerateHasuraClaimToken(user_id);

            // Return the new tokens
            return Ok(hasuraRes);
        }
        catch (SecurityTokenException)
        {
            return Unauthorized();
        }
    }
}
