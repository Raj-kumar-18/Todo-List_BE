using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ZOI.BAL.Services.Interface;
using ZOI.DAL.Utilities.CommonFunctions;
using ZOI.Domain.Models;

namespace ToDoList.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly commonFunctions _commonFunctions;
        private readonly IConfiguration _configuration;

        public UsersController(IUserServices userServices, IConfiguration configuration )
        {
            _userServices = userServices;
            _configuration = configuration;
            _commonFunctions = new commonFunctions( configuration );
        } 

        [HttpPost(Name = "SignUp")]
        public JsonResponse SignUp([FromBody] Users data)
        {
            JsonResponse response = _userServices.SignUp(data);

            if (response.Status == "S")
            {
                Response.Cookies.Append("jwt_token", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                });
                // Refresh Token
                Response.Cookies.Append("refresh_token", response.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }

            return response;
        }

        [HttpPost(Name = "Social-Login")]
        public async Task<JsonResponse> SocialLogin([FromBody] TokenRequest data)
        {
            JsonResponse response = await _userServices.SocialLogin(data);

            if(response.Status == "S")
            {
                Response.Cookies.Append("jwt_token", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                });
                // Refresh Token
                Response.Cookies.Append("refresh_token", response.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }

            return response;
        }

        [HttpPost(Name = "Custom-Login")]
        public async Task<JsonResponse> CustomLogin([FromBody] Users data)
        {
            JsonResponse response = await _userServices.CustomLogin(data);

            if (response.Status == "S")
            {
                Response.Cookies.Append("jwt_token", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                });
                // Refresh Token
                Response.Cookies.Append("refresh_token", response.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }

            return response;
        }

        [Authorize]
        [HttpGet(Name = "GetUsers")]
        public JsonResponse GetUsers()
        {
            JsonResponse response = _userServices.GetUsers();
            return response;
        }

        [HttpPost(Name = "Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            Response.Cookies.Delete("refresh_token");

            return Ok(new { message = "Logout successful" });
        }

        [Authorize]
        [HttpGet(Name = "GetProfile")]
        public JsonResponse GetProfile()
        {
            JsonResponse response = new JsonResponse();

            try
            {
                var jwt = Request.Cookies["jwt_token"];

                if (string.IsNullOrEmpty(jwt))
                {
                    response.Status = "F";
                    response.Message = "No Token Found";
                    return response;
                }

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);

                var id = token.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                var name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                var email = token.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value;
                var role = token.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

                response.Status = "S";
                response.Message = "Profile fetched successfully";
                response.Data = new
                {
                    UserID = id,
                    FullName = name,
                    Email = email,
                    Role = role
                };
            }
            catch (Exception ex)
            {
                response.Status = "F";
                response.Message = "Error while decoding token: " + ex.Message;
            }

            return response;
        }

        [HttpPost(Name = "Refresh Token")]
        public JsonResponse RefreshToken()
        {
            JsonResponse response = new JsonResponse();

            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                response.Status = "F";
                response.Message = "No refresh token found";
                return response;
            }

            try
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                // validate the refresh token
                handler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var token = (JwtSecurityToken)validatedToken;

                var userId = token.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                var name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                var email = token.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value;
                var role = token.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    response.Status = "F";
                    response.Message = "Invalid refresh token";
                    return response;
                }

                var newAccessToken = _commonFunctions.GenerateJwtToken(userId, email, role, name);

                Response.Cookies.Append("jwt_token", newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                });

                response.Status = "S";
                response.Message = "Token refreshed successfully";
                response.Data = new
                {
                    Token = newAccessToken,
                    FullName = name,
                    Role = role,
                    Email = email
                };
            }
            catch(Exception ex)
            {
                response.Status = "F";
                response.Message = "Error refreshing token: " + ex.Message;
            }

            return response;
        }
    }
}
