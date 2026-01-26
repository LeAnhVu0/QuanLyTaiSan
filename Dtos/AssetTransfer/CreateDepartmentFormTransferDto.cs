namespace QuanLyTaiSan.Dtos.AssetTransfer
{
    public class CreateDepartmentFormTransferDto
    {
          public required int AssetId { get; set; }
          public required int FromDepartmentId { get; set; }
          public required int ToDepartmentId { get; set; }
          public string? Note { get; set; }
          public string? Purpose { get; set; }
    }
}
