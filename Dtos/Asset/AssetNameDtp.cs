namespace QuanLyTaiSan.Dtos.Asset
{
    public class AssetNameDtp
    {
        public int AssetId { get; set; }

        public required string AssetCode { get; set; }

        public required string AssetName { get; set; }

        public required string AssetStatus { get; set; }
    }
}
