using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Models;

namespace QuanLyTaiSan.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
        Task<List<UserResponseDto>> GetAllUser();
        Task<UserResponseDto> GetUserById(string id);
        Task<string> DeleteUser(string id);
        Task<UserUpdateDto> UpdateUser(string id, UserUpdateDto dto);

        Task<PagedResult<UserResponseDto>> GetUserAsync(int pageIndex, int pageSize,string? search);
    }
}

