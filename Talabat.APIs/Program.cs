using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middleware;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.contract;
using Talabat.Core.Services.contract;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Data.Identity;
using Talabat.Service;
using Talabat.Service.AuthService;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Dependancy Injection

            builder.Services.AddControllers();
            //.AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //});
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            //******************************************************************
            //DbContext
            builder.Services.AddSwaggerServices();

            builder.Services.AddDbContext<StoreContext>( options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            //******************************************************************

            builder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            }
            );

            builder.Services.AddApplicationServices();

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                //options.Password.RequiredUniqueChars = 2;
                //options.Password.RequireNonAlphanumeric = true;
                //options.Password.RequireDigit = true;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();

            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:AuthKey"] ?? string.Empty))
                };
            });

            builder.Services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JWTToken_Auth_API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });

            #endregion

            var app = builder.Build();

            //-------------------------------------

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<StoreContext>();
            var _IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            var _userManager = services.GetRequiredService<UserManager<AppUser>>();

            try
            {
                await _dbContext.Database.MigrateAsync(); // Update-DataBase => StoreContext

                await StoreContextSeed.SeedAsync(_dbContext); // Data Seeding

                //---------------------------------

                await _IdentityDbContext.Database.MigrateAsync(); // Update-DataBase => AppIdentityDbContext

                await AppIdentityDbContextSeed.SeedUsersAsync(_userManager); // Data Seeding
            }
            catch (Exception e)
            {

                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(e, "an error occurred during apply migration.");
            }

            //-------------------------------------

            // Configure the HTTP request pipeline.

            #region Middlewares

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddleware();
            }

            app.UseStatusCodePagesWithReExecute("/Errors/{0}");

            app.UseHttpsRedirection();


            app.UseAuthorization();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.MapControllers();


            #endregion

            app.Run();
        }
    }
}
