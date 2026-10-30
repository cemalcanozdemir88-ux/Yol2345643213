using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PayFixYonetici.Core.Models;
using System.Text;
using PayFixYonetici.Core;
using PayFixYonetici.Infrastructure;
using PayFixYonetici.Services;
using Microsoft.OpenApi.Models;

namespace PayFixYonetici.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtTokenSettings = builder.Configuration.GetSection("JwtTokenSettings").Get<JwtTokenSettings>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                     .AddJwtBearer(options =>
                     {
                         options.RequireHttpsMetadata = false;
                         options.TokenValidationParameters = new TokenValidationParameters
                         {
                             ValidateIssuer = true,
                             ValidIssuer = jwtTokenSettings?.Issuer,
                             ValidateAudience = true,
                             ValidAudience = jwtTokenSettings?.Audience,
                             ClockSkew = TimeSpan.Zero,
                             ValidateLifetime = true,
                             ValidateIssuerSigningKey = true,
                             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSettings.SecurityKey))
                         };
                     });

            builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("JwtTokenSettings"));


            // Add services to the container.
            builder.Services.ConfigureCoreLayerDiRegistration();
            builder.Services.ConfigureInfrastructureLayerDiRegistration(builder.Configuration);
            builder.Services.ConfigureServiceLayerDiRegistration();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "PayFix API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
            });

            builder.Services.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
