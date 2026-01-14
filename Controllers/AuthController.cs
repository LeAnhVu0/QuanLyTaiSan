using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Services.Implementations;
using QuanLyTaiSan.Services.Interfaces;

namespace QuanLyTaiSan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IAuthService _authService;
        public AuthController(JwtService jwtService, IAuthService authService)
        {
            _jwtService = jwtService;
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponeDto>> Login([FromBody] UserLoginDto request)
        {
            var result = await _jwtService.Authenticate(request);
            return result is not null ? Ok(result) : Unauthorized();
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult<LoginResponeDto>> Refresh([FromBody] RefreshRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest("Invalid Token");
            var result = await _jwtService.ValidateRefreshToken(request.Token);
            return result is not null ? result : Unauthorized();
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] UserRegisterDto request)
        {
            var user = await _authService.RegisterAsync(request);
            return Ok(user);

        }
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUser()
        {
            var result = await _authService.GetAllUser();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            var result = await _authService.GetUserById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteUserAsync(string id)
        {
            var result = await _authService.GetUserById(id);
            if (result == null) return NotFound();
            await _authService.DeleteUser(id);
            return Ok("Xoa thanh cong ");
        }
        [HttpPatch("reset-password")]
        public async Task<ActionResult<string>> ResetPassword(ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok("Mật khẩu đã được cập nhật thành công.");
        }

    }
}
