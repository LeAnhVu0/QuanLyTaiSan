using QuanLyTaiSan.Dtos.Auth;

namespace QuanLyTaiSan.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(UserRegisterDto dto);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
        Task<List<UserDto>> GetAllUser();
        Task<UserDto> GetUserById(string id);
        Task<string> DeleteUser(string id);
    }
}

