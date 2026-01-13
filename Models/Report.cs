using QuanLyTaiSanTest.Enum;
namespace QuanLyTaiSanTest.Models
{
    public class Report
    {
        public int ReportId {  get; set; }
        public required string Title { get; set; }
        public ReportType Type {  get; set; }
        public string? FilterJson {  get; set; }
        public DateTime CreateTime {  get; set; } = DateTime.Now;
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted {  get; set; } = false;
        public int? UserId { get; set; } 
    }
}
