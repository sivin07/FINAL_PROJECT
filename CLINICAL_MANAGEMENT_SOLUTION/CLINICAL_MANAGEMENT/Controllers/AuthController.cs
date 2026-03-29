using CLINICAL_MANAGEMENT.DTOs.Auth;
using CLINICAL_MANAGEMENT.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        // Constructor Injection ✅
        public AuthController(IAuthService service)
        {
            _service = service;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _service.LoginAsync(dto);
            if (result == null)
                return Unauthorized("Invalid username or password!");
            return Ok(result);
        }
    }
}
