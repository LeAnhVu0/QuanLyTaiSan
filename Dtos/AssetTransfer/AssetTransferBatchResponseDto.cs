namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class AssetTransferBatchResponseDto
    {
        public Guid? BatchId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string TransferType { get; set; }

        // Thông tin đại diện cho một lô
        public string? FromUserName { get; set; }
        public string? ToUserName { get; set; }
        public string? FromDepartmentName { get; set; }
        public string? ToDepartmentName { get; set; }
        public string? Purpose { get; set; }
        public string? Note { get; set; }
        public int AssetCount { get; set; } // Tổng số tài sản trong lô

        // Danh sách tài sản con bên trong
        public List<AssetTransferDetailDto> AssetsTransfer { get; set; } = new List<AssetTransferDetailDto>();
    }
}
