using QuanLyTaiSan.Dtos.Auth;

namespace QuanLyTaiSan.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
        Task<List<UserResponseDto>> GetAllUser();
        Task<UserResponseDto> GetUserById(string id);
        Task<string> DeleteUser(string id);
    }
}

