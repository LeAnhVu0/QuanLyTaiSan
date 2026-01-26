namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class CreateMultiFormTransferDto
    {
        public int? DepartmentId { get; set; }

        public List<int> AssetIds { get; set; }

        public string? ToUserId { get; set; }
        public string? Purpose { get; set; }
        public string? Note { get; set; }
    }
}
