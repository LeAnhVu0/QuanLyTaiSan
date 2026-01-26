using Microsoft.AspNetCore.Identity;
using QuanLyTaiSan.Dtos.AssetHistory;
using QuanLyTaiSan.Dtos.Auth;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Dtos.AssetHistory;
using QuanLyTaiSanTest.Models;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuanLyTaiSanTest.Services.Implementations
{
    public class AssetHistoryService:IAssetHistoryService
    {
        private readonly IAssetHistoryRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public AssetHistoryService(IAssetHistoryRepository repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        public async Task<AssetHistoryAllDto> GetAll(int pageIndex, int pageSize, string? searchName, string? actionType)
        {
            var result = await _repo.GetAll(pageIndex,pageSize,searchName,actionType);
            var items =  result.items.Select(h => new AssetHistoryDto
            {
                HistoryID = h.HistoryID,
                ActionType = h.ActionType,
                ActionDate = h.ActionDate,
                Descriptions = h.Descriptions,
                AssetId = h.AssetId,
                AssetName = h.AssetName,
                Status = h.Status,
                CreatedByUser = new UserDto
                {
                    Id = h.CreatedByUserId,
                    Username = h.CreatedByUser.UserName,
                    Fullname = h.CreatedByUser.FullName

                },
                AssignedToUser = h.AssignedToUserId == null ? null : new UserDto
                { 
                    Id = h.AssignedToUserId,
                    Username = h.AssignedToUser?.UserName ,
                    Fullname = h.AssignedToUser?.FullName 

                }
            }).ToList();

            var totalCount = result.totalCount;
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new AssetHistoryAllDto
            {
                ListAssetHistory = items,
                SearchName = searchName,
                ActionType = actionType,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPage = totalPage,
                HasNextPage = totalPage > pageIndex,
                HasPreviousPage = pageIndex > 1
            };
        }
        public async Task<List<AssetHistoryDto>> GetById(int assetId)
        {
            var list = await _repo.GetById(assetId);
            if(list == null)
            {
                throw new KeyNotFoundException("Không tồn tại tài sản");
            }    
            else
            {
                return list.Select(h => new AssetHistoryDto
                {
                    HistoryID = h.HistoryID,
                    ActionType = h.ActionType,
                    ActionDate = h.ActionDate,
                    Descriptions = h.Descriptions,
                    AssetId = h.AssetId,
                    AssetName = h.AssetName,
                    Status = h.Status,
                    CreatedByUser = new UserDto
                    {
                        Id = h.CreatedByUserId,
                        Username = h.CreatedByUser.UserName,
                        Fullname = h.CreatedByUser.FullName
                        //Email = h.CreatedByUser.Email,
                        //PhoneNumber = h.CreatedByUser.PhoneNumber
                    },
                    AssignedToUser = h.AssignedToUserId == null ? null : new UserDto
                    {
                        Id = h.AssignedToUserId,
                        Username = h.AssignedToUser.UserName ,
                        Fullname = h.AssignedToUser.FullName
                        //Email = h.AssignedToUser.Email,
                        //PhoneNumber = h.AssignedToUser.PhoneNumber
                    }
                }).ToList();
            }    
        }

    }
}
