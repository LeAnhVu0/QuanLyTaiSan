using QuanLyTaiSan.Models;
using QuanLyTaiSan.Repositories.Interfaces;
using QuanLyTaiSanTest.Data;
using Microsoft.EntityFrameworkCore;
namespace QuanLyTaiSan.Repositories.Implementations
{
        public class RefreshTokenRepository : IRefreshTokenRepository
        {
            private readonly AppDbContext _context;
            public RefreshTokenRepository(AppDbContext context)
            {
                _context = context;
            }
            public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
            {
                _context.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();
                return refreshToken;
            }

            public async Task<RefreshToken?> GetByTokenAsync(string token)
            {
                return await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == token);
            }

            public async Task<RefreshToken?> GetValidTokenAsync(string token)
            {
                return await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt =>
                        rt.Token == token &&
                        !rt.IsRevoked &&
                        rt.ExpiresAt > DateTime.UtcNow
                    );
            }

            public async Task<List<RefreshToken>> GetByUserIdAsync(string userId)
            {
                return await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId)
                    .OrderByDescending(rt => rt.CreatedAt)
                    .ToListAsync();
            }

            public async Task RevokeAsync(string token)
            {
                var refreshToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == token);

                if (refreshToken != null)
                {
                    refreshToken.IsRevoked = true;
                    refreshToken.RevokedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            public async Task RevokeAllByUserIdAsync(string userId)
            {
                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                    .ToListAsync();

                foreach (var token in tokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            public async Task DeleteExpiredTokensAsync()
            {
                var expiredTokens = await _context.RefreshTokens
                    .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
                    .ToListAsync();

                _context.RefreshTokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
        }
    
}
