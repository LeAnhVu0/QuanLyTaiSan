namespace QuanLyTaiSan.Dtos.Asset
{
    public class AssetHandoverDto
    {
        public int AssetId { get; set; }

        public required string AssetCode { get; set; }

        public required string AssetName { get; set; }
        public string? Status { get; set; }
        public string? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
