using LikesAndSwipes.Data;
using LikesAndSwipes.Models;
using LikesAndSwipes.Repositories;
using LikesAndSwipes.Options;
using LikesAndSwipes.Services;
using Minio;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LikesAndSwipes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString, sqlOption => sqlOption.UseNetTopologySuite()));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    )
                };
            });

            builder.Services.AddControllersWithViews();
            builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection(MinioOptions.SectionName));
            builder.Services.AddSingleton<IMinioClient>(_ =>
            {
                var minioOptions = builder.Configuration.GetSection(MinioOptions.SectionName).Get<MinioOptions>()
                    ?? throw new InvalidOperationException("Minio configuration is missing.");

                if (string.IsNullOrWhiteSpace(minioOptions.Endpoint) ||
                    string.IsNullOrWhiteSpace(minioOptions.AccessKey) ||
                    string.IsNullOrWhiteSpace(minioOptions.SecretKey) ||
                    string.IsNullOrWhiteSpace(minioOptions.BucketName))
                {
                    throw new InvalidOperationException("Minio configuration is incomplete.");
                }

                var clientBuilder = new MinioClient()
                    .WithEndpoint(minioOptions.Endpoint)
                    .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);

                if (minioOptions.UseSsl)
                {
                    clientBuilder = clientBuilder.WithSSL();
                }

                return clientBuilder.Build();
            });
            builder.Services.AddScoped<IMinioStorageService, MinioStorageService>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
            });

            builder.Services.AddTransient<DataRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
