using QuanLyTaiSanTest.Enum;

namespace QuanLyTaiSanTest.Dtos.Report
{
    public class ReportHistoryDto
    {
        public int ReportId { get; set; }
        public required string Title { get; set; }
        public ReportType Type { get; set; }
        public string? FilterJson { get; set; }
        public DateTime CreateTime { get; set; } 
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? UserId { get; set; }
    }
}
