using Microsoft.AspNetCore.Identity;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Services.Interfaces;
using AutoMapper;
namespace QuanLyTaiSan.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtService _jwtService;
        private readonly IMapper _mapper;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, JwtService jwtService, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<UserDto> RegisterAsync(UserRegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                DateOfBirth = dto.DateofBirth,
                DepartmentId = dto.DepartmentId == 0 ? null : dto.DepartmentId
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Register failed: {errors}");
            }

            return new UserDto
            {

                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                DepartmentId = dto.DepartmentId,
            };
        }
        public async Task<List<UserDto>> GetAllUser()
        {
            var users = _userManager.Users.ToList();
            return _mapper.Map<List<UserDto>>(users);
        }
        public async Task<UserDto> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;
            return _mapper.Map<UserDto>(user);
        }
        public async Task<string> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return "User not found";
            _userManager.DeleteAsync(user);
            return $"User {user.Id} deleted";
        }
        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return null;


            string decodedToken = System.Net.WebUtility.UrlDecode(dto.Token);


            string cleanToken = decodedToken.Replace(" ", "+");

            var result = await _userManager.ResetPasswordAsync(user, cleanToken, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Lỗi: {errors}");
            }
            return "Mật khẩu đã được đặt lại thành công.";
        }
    }
}
