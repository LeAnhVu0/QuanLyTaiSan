using QuanLyTaiSanTest.Enum;

namespace QuanLyTaiSanTest.Dtos.Report
{
    public class ReportAnalyticsDto
    {
        public string GroupName { get; set; } // Tên nhóm (VD: Phòng IT, Máy tính...)
        public int Quantity { get; set; }     // Số lượng tài sản
        public decimal TotalValue { get; set; }
    }
}
