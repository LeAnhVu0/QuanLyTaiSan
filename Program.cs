
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuanLyTaiSan.Data;
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
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "QuanLySach API", Version = "v1" });

                //  Khai báo JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập JWT theo dạng: Bearer {token}"
                });

                //  Áp dụng cho các API có [Authorize]
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            //Đăng kí cors 
            builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
                                                                         policy.AllowAnyOrigin()
                                                                              .AllowAnyHeader()
                                                                              .AllowAnyMethod()));

            //DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
            //var connectString = builder.Configuration.GetConnectionString("MyDb");
            //builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(connectString));
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
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Permissions.GrantPermission,
                    p => p.RequireClaim("Permission", Permissions.GrantPermission));

                options.AddPolicy(Permissions.AssetCreate,
                    p => p.RequireClaim("Permission", Permissions.AssetCreate));

                options.AddPolicy(Permissions.AssetAssign,
                    p => p.RequireClaim("Permission", Permissions.AssetAssign));

                options.AddPolicy(Permissions.AssetDelete,
                    p => p.RequireClaim("Permission", Permissions.AssetDelete));
                options.AddPolicy(Permissions.AssetUpdate,
                  p => p.RequireClaim("Permission", Permissions.AssetUpdate));

                options.AddPolicy(Permissions.AssetGetHistory,
                    p => p.RequireClaim("Permission", Permissions.AssetGetHistory));

                options.AddPolicy(Permissions.AssetRecall,
                    p => p.RequireClaim("Permission", Permissions.AssetRecall));

                options.AddPolicy(Permissions.DepartmentCreate,
                    p => p.RequireClaim("Permission", Permissions.DepartmentCreate));

                options.AddPolicy(Permissions.DepartmentDelete,
                    p => p.RequireClaim("Permission", Permissions.DepartmentDelete));

                options.AddPolicy(Permissions.DepartmentGet,
                    p => p.RequireClaim("Permission", Permissions.DepartmentGet));

                options.AddPolicy(Permissions.DepartmentUpdate,
                    p => p.RequireClaim("Permission", Permissions.DepartmentUpdate));

                options.AddPolicy(Permissions.InventoryCreate,
                    p => p.RequireClaim("Permission", Permissions.InventoryCreate));

                options.AddPolicy(Permissions.ReportGet,
                    p => p.RequireClaim("Permission", Permissions.ReportGet));

                options.AddPolicy(Permissions.UserCreate,
                    p => p.RequireClaim("Permission", Permissions.UserCreate));

                options.AddPolicy(Permissions.UserDelete,
                    p => p.RequireClaim("Permission", Permissions.UserDelete));

                options.AddPolicy(Permissions.UserGet,
                    p => p.RequireClaim("Permission", Permissions.UserGet));

                options.AddPolicy(Permissions.UserRefreshToken,
                    p => p.RequireClaim("Permission", Permissions.UserRefreshToken));
            });


            //Repo
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            //Service
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
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
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

            //cấu hình httpClient để gọi api khác
            builder.Services.AddHttpClient();
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                await IdentitySeeder.SeedAsync(scope.ServiceProvider);
            }
            // sử dụng HttpContextAccessor
            builder.Services.AddHttpContextAccessor();
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
