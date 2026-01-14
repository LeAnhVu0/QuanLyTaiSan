
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyTaiSan.Mappings;
using QuanLyTaiSan.Models;
using QuanLyTaiSan.Repositories.Implementations;
using QuanLyTaiSan.Repositories.Interfaces;
using QuanLyTaiSan.Services.Implementations;
using QuanLyTaiSan.Services.Interfaces;
using QuanLyTaiSanTest.Data;
using QuanLyTaiSanTest.Repositories.Implementations;
using QuanLyTaiSanTest.Repositories.Interfaces;
using QuanLyTaiSanTest.Services.Implementations;
using QuanLyTaiSanTest.Services.Interfaces;
using System.Security.Claims;
using System.Text;

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
        //    builder.Services.AddDbContext<AppDbContext>(options =>
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        //);
            var connectString = builder.Configuration.GetConnectionString("MyDb");
            builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(connectString));
            // Identity
            builder.Services
                .AddIdentityCore<ApplicationUser>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // JWT
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                        ),
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            builder.Services.AddAuthorization();
            //Repo
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            //Service
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            //mapping
            builder.Services.AddAutoMapper(typeof(MappingProfile));
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
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
