using ASP.NET_API.Helpers;
using ASP.NET_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs;
using Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly APIKeyService _keyServices;

        public AccountsController(UserManager<User> userManager,
                                  IConfiguration configuration,
                                  SignInManager<User> signInManager,
                                  APIKeyService keyServices)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _keyServices = keyServices;
        }

        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentials userCredentials)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredentials.Email,
                                    userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(userCredentials.Email);
                return await BuildToken(userCredentials, user.Id);
            }
            else
            {
                return BadRequest("Incorrect login");
            }
        }

        [HttpPost("register", Name = "Register")]
        public async Task<ActionResult<AuthenticationResponse>> Register(UserCredentials userCredentials)
        {
            var user = new User
            {
                UserName = userCredentials.Email,
                Email = userCredentials.Email
            };

            var result = await _userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                await _keyServices.CreateKey(user.Id, KeyType.Free);
                return await BuildToken(userCredentials, user.Id);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet("RefreshToken", Name = "Refresh")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticationResponse>> Refresh()
        {
            var emailClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
            var email = emailClaim?.Value;

            var claimId = HttpContext.User.Claims.Where(c => c.Type == "id").FirstOrDefault();
            var userId = claimId?.Value;

            var userCredencials = new UserCredentials
            {
                Email = email,

            };

            return await BuildToken(userCredencials, userId);
        }

        [HttpPost("ConvertToAdmin", Name = "ConvertToAdmin")]
        public async Task<ActionResult> ConvertToAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await _userManager.FindByEmailAsync(editAdminDTO.Email);
            await _userManager.AddClaimAsync(user, new Claim(PoliciesHelper.IsAnAdmin, "1"));
            return NoContent();
        }

        [HttpPost("ConvertToNoAdmin", Name = "ConvertToNoAdmin")]
        public async Task<ActionResult> ConvertToNoAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await _userManager.FindByEmailAsync(editAdminDTO.Email);
            await _userManager.RemoveClaimAsync(user, new Claim(PoliciesHelper.IsAnAdmin, "1"));
            return NoContent();
        }

        private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials, string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email),
                new Claim("id", userId),
            };

            var user = await _userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDB = await _userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);

            return new AuthenticationResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration,
            };
        }
    }
}
