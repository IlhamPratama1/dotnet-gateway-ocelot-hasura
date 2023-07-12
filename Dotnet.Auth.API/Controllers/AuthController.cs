using Microsoft.AspNetCore.Mvc;
using Dotnet.Auth.API.Entities;
using Dotnet.Auth.API.Services;
using Dotnet.Auth.API.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.Net;

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
            Response<DirectusRes> res = await HttpRequestService<Response<DirectusRes>>.PostAsync("auth/login", value);

            DirectusToken? directusToken = TokenService.DecodeDirectusToken(res.data?.access_token);
            LoginRes? result = TokenService.GenerateMultiserviceToken(directusToken, res.data?.refresh_token);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                statusCode = 401,
                success = false,
                message= ex.Message,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                statusCode = 401,
                success = false,
                message = ex.Message,
            });
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
    public async Task<IActionResult> RefreshToken([FromBody] RefreshDto value)
    {
        try
        {
            Response<DirectusRes> res = await HttpRequestService<Response<DirectusRes>>.PostAsync("auth/refresh", value);

            DirectusToken? directusToken = TokenService.DecodeDirectusToken(res.data?.access_token);
            LoginRes? result = TokenService.GenerateMultiserviceToken(directusToken, res.data?.refresh_token);

            return Ok(result);
        }
        catch (SecurityTokenException)
        {
            return Unauthorized();
        }
    }
}
