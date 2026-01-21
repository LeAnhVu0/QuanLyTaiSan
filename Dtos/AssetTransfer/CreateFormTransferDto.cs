using QuanLyTaiSan.Enum;
using QuanLyTaiSan.Models;

namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class CreateFormTransferDto
    {
        public int AssetId { get; set; }
        public string? ToUserId { get; set; }
        public string? Note { get; set; }
    }
}
