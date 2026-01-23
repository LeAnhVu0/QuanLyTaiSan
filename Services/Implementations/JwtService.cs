using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace QuanLyTaiSan.Services.Implementations
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public JwtService(IConfiguration config, UserManager<ApplicationUser> userManager, IRefreshTokenRepository refreshTokenRepository)
        {
            _config = config;
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<LoginResponeDto?> Authenticate(UserLoginDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
                return null;

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return null;

            if (user.Status == UserStatus.inactive)
            {
                return new LoginResponeDto
                {
                    Message = "Tài khoản đã bị ngưng sử dụng"
                };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return null;

            return await GenerateToken(user);
        }

        public async Task<LoginResponeDto> ValidateRefreshToken(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
            if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return null;
            }
            await _refreshTokenRepository.DeleteExpiredTokensAsync();
            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null) return null;
            return await GenerateToken(user);
        }
        public async Task<LoginResponeDto> GenerateToken(ApplicationUser user)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var tokenValidityMins = _config.GetValue<int>("Jwt:TokenValidityMins");

            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

           
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims.Where(c => c.Type == "Permission"))
            {
                claims.Add(claim);
            }

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: tokenExpiryTimeStamp,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponeDto
            {
                Username = user.UserName,
                AccessToken = accessToken,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds,
                RefreshToken = await GenerateRefreshToken(user.Id)
            };
        }

        public async Task<string> GenerateRefreshToken(string userId)
        {
            var refreshTokenValidityMins = _config.GetValue<int>("Jwt:RefreshTokenValidityMins");
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(refreshTokenValidityMins),
                UserId = userId
            };
            await _refreshTokenRepository.CreateAsync(refreshToken);
            return refreshToken.Token;
        }
    }
    }
