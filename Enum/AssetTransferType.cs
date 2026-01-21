    namespace QuanLyTaiSan.Enum
{
    public enum AssetTransferType
    {
        Handover = 1,   // Bàn giao
        Recall = 2 // Thu hồi
    }
    public static class AssetTransferTypeExtention
    {
        public static string ToDisplayName(this AssetTransferType s)
        {
            return s switch
            {
                AssetTransferType.Handover => "Bàn giao",
                AssetTransferType.Recall => "Thu hồi",
                _ => "Không xác định"
            };
        }
    }
}
