using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Services.Interfaces;
using QuanLyTaiSanTest.Data;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
namespace QuanLyTaiSan.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, JwtService jwtService, IMapper mapper, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _jwtService = jwtService;
            _mapper = mapper;

            _context = context;
        }
        public async Task<UserResponseDto> RegisterAsync(UserRegisterDto dto)
        {
            if (await _userManager.FindByNameAsync(dto.Username) != null)
                throw new InvalidOperationException("Username đã tồn tại");

            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                throw new InvalidOperationException("Email đã tồn tại");

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = dto.Fullname,
                Status = dto.Status,
                Address = dto.Address,
                DateOfBirth = dto.DateofBirth,
                DepartmentId = dto.DepartmentId == 0 ? null : dto.DepartmentId
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new InvalidOperationException(string.Join(", ", errors));
            }

            var role = dto.IsAdmin ? "Admin" : "User";
            await _userManager.AddToRoleAsync(user, role);

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Fullname = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                DepartmentId = user.DepartmentId,
                Role = role
            };
        }

        public async Task<List<UserResponseDto>> GetAllUser()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Fullname = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Status = user.Status,
                    Address = user.Address,
                    DateOfBirth = user. DateOfBirth,
                    DepartmentId = user.DepartmentId,
                    Role = roles.FirstOrDefault() 
                });
            }

            return result;
        }
        public async Task<PagedResult<UserResponseDto>> GetUserAsync(
     int pageIndex,
     int pageSize,
     string? search,
     int? departmentId, UserStatus? status, string? role)
        {
            var query = _userManager.Users.AsQueryable();

            // 🔎 Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.UserName.Contains(search) ||
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search) ||
                    u.PhoneNumber.Contains(search));
            }

            // 🏢 Filter theo department
            if (departmentId.HasValue)
            {
                query = query.Where(u => u.DepartmentId == departmentId.Value);
            }
            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }
            if (!string.IsNullOrEmpty(role))
            {
                 query = from u in query
                                join ur in _context.UserRoles on u.Id equals ur.UserId
                                join r in _context.Roles on ur.RoleId equals r.Id
                                where r.Name == role
                                select u;
            }
            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            var users = await query
                .OrderByDescending(u => u.CreateTime) // ⚠️ chỉ OrderBy 1 lần
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var items = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                items.Add(new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Fullname = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Status = user.Status,
                    Address = user.Address,
                    DateOfBirth = user.DateOfBirth,
                    DepartmentId = user.DepartmentId,
                    Role = roles.FirstOrDefault()
                });
            }

            return new PagedResult<UserResponseDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPage = totalPage,
                Items = items
            };
        }



        public async Task<UserResponseDto> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            

            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.UserName,
                Fullname = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                DepartmentId = user.DepartmentId,
                Role = roles.FirstOrDefault()
            };
        }
        public async Task<string> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return "User not found";
            if(user.Status == UserStatus.inactive)
            {
                return "User is already inactive";
            }
            user.Status = UserStatus.inactive;
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            await _userManager.UpdateAsync(user);
            return $"User {user.Id} change status";
        }
        public async Task<string> ChangePasswordAsync(
    string userId,
    ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User không tồn tại");

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword
            );

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            return "Đổi mật khẩu thành công";
        }
    

        public async Task<UserUpdateDto> UpdateUser(string id, UserUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy người dùng");

            user.UserName = dto.Username;
            user.Email = dto.Email;
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.Status = dto.Status;
            user.DepartmentId = dto.DepartmentId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Cập nhật thất bại: {errors}");
            }
            return dto;
        }

    }
}
