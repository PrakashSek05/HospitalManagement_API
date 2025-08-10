using Microsoft.AspNetCore.Mvc;
using HospitalManagement.API.Auth;

namespace HospitalManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public AuthController(ITokenService tokenService) => _tokenService = tokenService;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // TODO: validate user from DB instead of hardcoding
            if (request.Username == "admin" && request.Password == "admin123")
            {
                var token = _tokenService.CreateToken("1", "admin", "Admin");
                return Ok(new { token });
            }
            return Unauthorized();
        }
    }

    public record LoginRequest(string Username, string Password);
}
