using QuanLyTaiSan.Models;

namespace QuanLyTaiSan.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetValidTokenAsync(string token);
        Task<List<RefreshToken>> GetByUserIdAsync(string userId);
        Task RevokeAsync(string token);
        Task RevokeAllByUserIdAsync(string userId);
        Task DeleteExpiredTokensAsync();
    }
}
