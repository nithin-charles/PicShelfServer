using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PicShelfServer.Models.DTO;
using PicShelfServer.Services;

namespace PicShelfServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;
        public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            this._userManager = userManager;
            this._tokenService = tokenService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("/data")]
        public IActionResult getData()
        {
            return Ok("Hi welcome to Pic Shelf Server!");
        }
        
        [HttpGet("test")]
        public IActionResult TestEndpoint([FromHeader(Name = "Authorization")] string authHeader)
        {
            // Console.WriteLine($"Authorization Header: {authHeader}");
            var msg = authHeader;
            return Ok(authHeader);
        }


        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (!identityResult.Succeeded || registerRequestDto.Roles.Length == 0) 
                return BadRequest("Something went wrong");
            
            identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

            if (identityResult.Succeeded)
            {
                return Ok("User was registered! Please login.");
            }

            return BadRequest("Something went wrong");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);

            if (user == null) 
                return BadRequest("Username or password incorrect");
            
            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (!checkPasswordResult) 
                return BadRequest("Username or password incorrect");
            
            // Get Roles for this user
            var roles = await _userManager.GetRolesAsync(user);

            if (roles != null)
            {
                // Create Token

                var jwtToken = _tokenService.CreateJWTToken(user, roles.ToList());

                var response = new LoginResponseDto
                {
                    JwtToken = jwtToken
                };
                var cookieOption = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                };
                Response.Cookies.Append("JWTToken", jwtToken, cookieOption);

                return Ok(response);
            }

            return BadRequest("Username or password incorrect");
        }
    }
}
