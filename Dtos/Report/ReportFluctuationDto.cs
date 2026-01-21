namespace QuanLyTaiSan.Dtos.Report
{
    public class ReportFluctuationDto
    {
        public int NewAssetsCount { get; set; }        // Số lượng mua mới
        public decimal NewAssetsValue { get; set; }    // Giá trị mua mới

        public int LiquidatedAssetsCount { get; set; } // Số lượng thanh lý/hủy
        public decimal LiquidatedAssetsValue { get; set; } // Giá trị thanh lý
    }
}
