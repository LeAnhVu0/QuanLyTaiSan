namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class CreateMultiDepartmentFormTransferDto
    {
        public required List<int> AssetIds { get; set; }
        public required int FromDepartmentId { get; set; }
        public required int ToDepartmentId { get; set; }
        public string? Note { get; set; }
        public string? Purpose { get; set; }
    }
}
