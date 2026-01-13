using Microsoft.EntityFrameworkCore;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<AssetHistory> AssetHistory { get; set; }
        public DbSet<Report> Report { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>(e =>
            {
                e.Property(x => x.OriginalValue).HasPrecision(19,0);
            });

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany(a => a.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // ràng buộc xóa
            
            modelBuilder.Entity<AssetHistory>(e =>
            {
                e.Property(x => x.OriginalValue).HasPrecision(19, 0);
            });



        }
    }
}
