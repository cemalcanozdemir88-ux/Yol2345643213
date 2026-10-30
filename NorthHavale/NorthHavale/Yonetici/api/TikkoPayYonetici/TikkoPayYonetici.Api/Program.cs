using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TikkoPayYonetici.Api.ActionFilters;
using TikkoPayYonetici.Api.ActionFilters.PermissionControl;
using TikkoPayYonetici.Api.Infrastructure;
using TikkoPayYonetici.Core;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Infrastructure;
using TikkoPayYonetici.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var jwtTokenSettings = builder.Configuration.GetSection("JwtTokenSettings").Get<JwtTokenSettings>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.RequireHttpsMetadata = false;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidIssuer = jwtTokenSettings.Issuer,
         ValidateAudience = true,
         ValidAudience = jwtTokenSettings.Audience,
         ClockSkew = TimeSpan.Zero,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSettings.SecurityKey))
     };
 });

builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("JwtTokenSettings"));

builder.Services.AddScoped<LogActionFilter>();

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

// Add services to the container.
builder.Services.ConfigureCoreLayerDiRegistration();
builder.Services.ConfigureInfrastructureLayerDiRegistration(builder.Configuration);
builder.Services.ConfigureServiceLayerDiRegistration();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "NorthHavale API", Version = "v1" });
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
    o.AddPolicy("CorsPolicy", opt =>
    {
        opt.SetIsOriginAllowed(origin =>
        {
            if (builder.Environment.IsDevelopment())
            {
                return true;
            }

            if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            {
                return uri.Host.EndsWith("thyhometurnnn.com");
            }

            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient();

builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCaching();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CheckUserStatusMiddleware>();
//app.UseMiddleware<IpRestrictionMiddleware>();

app.MapControllers();

app.Run();
