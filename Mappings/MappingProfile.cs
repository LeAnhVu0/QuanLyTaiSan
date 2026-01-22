using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Dtos.Department;
using QuanLyTaiSan.Models;
using AutoMapper;
namespace QuanLyTaiSan.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Department, DepartmentResponseDto>();
            CreateMap<DepartmentCreateDto, Department>();
            CreateMap<DepartmentUpdateDto, Department>();
            CreateMap<ApplicationUser, UserResponseDto>();
            CreateMap<ApplicationUser, UserUpdateDto>();
            CreateMap<ApplicationUser, UserInDepartmentDto>();
        }
    }
}
