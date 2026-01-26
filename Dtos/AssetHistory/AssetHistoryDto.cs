using QuanLyTaiSan.Dtos.Auth;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSanTest.Dtos.AssetHistory
{
    public class AssetHistoryDto
    {

        public int HistoryID { get; set; }

        public string? ActionType { get; set; }

        public string? Descriptions { get; set; }

        public DateTime ActionDate { get; set; }

        public string? AssetName { get; set; }
        public int Status { get; set; }

        // Khóa ngoại
        public int AssetId { get; set; }
        public UserDto? AssignedToUser { get; set; }
        public UserDto? CreatedByUser { get; set; } 

    }
}
