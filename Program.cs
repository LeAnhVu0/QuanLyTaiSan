
using Microsoft.EntityFrameworkCore;
using QuanLyTaiSanTest.Data;
using QuanLyTaiSanTest.Repositories.Implementations;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Implementations;
using QuanLyTaiSanTest.Services.Interfaces;

namespace QuanLyTaiSan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //Đăng kí cors 
            builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
                                                                         policy.AllowAnyOrigin()
                                                                              .AllowAnyHeader()
                                                                              .AllowAnyMethod()));

            //DbContext
            var connectString = builder.Configuration.GetConnectionString("MyDb");
            builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(connectString));



            //Khai báo di
            builder.Services.AddScoped<IAssetRepository, AssetRepository>();
            builder.Services.AddScoped<IAssetService, AssetService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IAssetHistoryRepository, AssetHistoryRepository>();
            builder.Services.AddScoped<IAssetHistoryService, AssetHistoryService>();
            builder.Services.AddScoped<IReportRepository, ReportRepository>();
            builder.Services.AddScoped<IReportService, ReportService>();

            //cấu hình httpClient để gọi api khác
            builder.Services.AddHttpClient();
            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
