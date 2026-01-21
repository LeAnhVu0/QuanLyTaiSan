using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiSan.Models;
using QuanLyTaiSanTest.Models;

namespace QuanLyTaiSanTest.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class AppDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<AssetHistory> AssetHistory { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<AssetTransfer> AssetTransfer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Asset>(e =>
            {
                e.Property(x => x.OriginalValue).HasPrecision(19, 0);
            });

            modelBuilder.Entity<Asset>(e =>
            {
                e.HasOne(a => a.Category)
                .WithMany(a => a.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
                
                
                e.HasOne(a => a.User)
                .WithMany(a => a.Assets)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a => a.Department)
                .WithMany(a => a.Assets)
                .HasForeignKey(a => a.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Inventory>(e =>
            {
                e.HasOne(a => a.Department)
                .WithMany()
                .HasForeignKey(a => a.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


                e.HasOne(x => x.User)
                .WithMany(u => u.inventories)
                .HasForeignKey(x => x.UserIdBy)
                .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<AssetHistory>(e =>
            {
                e.HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a => a.AssignedToUser)
                .WithMany()
                .HasForeignKey(a => a.AssignedToUserId) 
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Report>(e =>
            { 
                e.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                entity.HasIndex(rt => rt.Token).IsUnique();
                entity.HasIndex(rt => rt.UserId);
                entity.HasIndex(rt => rt.ExpiresAt);

                entity.HasOne(rt => rt.User)
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rt => rt.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(rt => rt.IsRevoked)
                    .HasDefaultValue(false);
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Department)
                    .WithMany(d => d.User)
                    .HasForeignKey(u => u.DepartmentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<AssetTransfer>(entity =>
            {
                entity.HasKey(x => x.TransferId);

                entity.HasOne(x => x.Asset)
                      .WithMany()
                      .HasForeignKey(x => x.AssetId)
                      .OnDelete(DeleteBehavior.Restrict);
               
                entity.HasOne(x => x.Department)
                    .WithMany()
                    .HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.FromUser)
                      .WithMany()
                      .HasForeignKey(x => x.FromUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ToUser)
                      .WithMany()
                      .HasForeignKey(x => x.ToUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(x => x.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ApprovedByUser)
                      .WithMany()
                      .HasForeignKey(x => x.ApprovedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.TransferType)
                      .HasConversion<int>();

                entity.Property(x => x.Status)
                      .HasConversion<int>();
            });
        }
    }

}
