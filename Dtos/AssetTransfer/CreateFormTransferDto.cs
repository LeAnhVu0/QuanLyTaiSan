using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class CreateFormTransferDto
    {
        public int? DepartmentId { get; set; }

        public int AssetId { get; set; }

        public string? ToUserId { get; set; }
        public string? Purpose { get; set; }
        public string? Note { get; set; }
    }
}
