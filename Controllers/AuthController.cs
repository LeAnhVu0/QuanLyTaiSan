using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Models;
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
        [Authorize]
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
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserRegisterDto request)
        {
            var user = await _authService.RegisterAsync(request);
            return Ok(user);

        }
        [Authorize(Policy = Permissions.UserGet)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUser()
        {
            var result = await _authService.GetAllUser();
            return Ok(result);
        }
        [Authorize(Policy = Permissions.UserGet)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(string id)
        {
            var result = await _authService.GetUserById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [Authorize(Policy = Permissions.UserDelete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteUserAsync(string id)
        {
            var result = await _authService.GetUserById(id);
            if (result == null) return NotFound();
            await _authService.DeleteUser(id);
            return Ok("Change status done ");
        }

        [HttpPatch("reset-password")]
        public async Task<ActionResult<string>> ResetPassword(ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok("Mật khẩu đã được cập nhật thành công.");
        }
        [Authorize(Policy =Permissions.UserDelete)]
        [HttpPatch("update-user")] 
        public async Task<ActionResult<UserUpdateDto>> UpdateUser(string id,UserUpdateDto dto)
        {
            var result= await _authService.UpdateUser(id, dto);
            if (result == null) return NotFound();
            
            return Ok(result);
        }

        [Authorize(Policy = Permissions.UserGet)]
        [HttpGet("paged")]
        public async Task<ActionResult> GetUserPagedAsync(  [FromQuery] string? search, int pageIndex = 1, int pageSize = 5)
        {
            var result = await _authService.GetUserAsync(pageIndex, pageSize, search);
            return Ok(result);
        }
    }
}
