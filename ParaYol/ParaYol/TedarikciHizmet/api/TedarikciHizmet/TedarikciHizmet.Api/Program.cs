using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TedarikciHizmet.Api.ActionFilters;
using TedarikciHizmet.Api.Infrastructure;
using TedarikciHizmet.Core;
using TedarikciHizmet.Core.Models;
using TedarikciHizmet.Infrastructure;
using TedarikciHizmet.Services;

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
     options.Events = new JwtBearerEvents
     {
         OnMessageReceived = context =>
         {
             var accessToken = context.Request.Query["access_token"];

             if (!string.IsNullOrEmpty(accessToken) &&
                 context.HttpContext.Request.Path.StartsWithSegments("/hubs/notify"))
             {
                 context.Token = accessToken;
             }

             return Task.CompletedTask;
         }
     };
 });

builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("JwtTokenSettings"));
builder.Services.Configure<OtpSettings>(builder.Configuration.GetSection("OtpSettings"));
builder.Services.Configure<YoneticiPanelSettings>(builder.Configuration.GetSection("YoneticiPanelSettings"));

builder.Services.AddScoped<LogActionFilter>();

// Add services to the container.
builder.Services.ConfigureCoreLayerDiRegistration();
builder.Services.ConfigureInfrastructureLayerDiRegistration(builder.Configuration);
builder.Services.ConfigureServiceLayerDiRegistration();

builder.Services.AddControllers();
// SignalR
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Tedarikçi Hizmet API", Version = "v1" });
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
                return uri.Host.EndsWith(".parayolpayment.com");
            }

            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader();
        
        // allow credentials for SignalR
        opt.AllowCredentials();
    });
});

builder.Services.AddHttpClient();

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

app.UseSerilogRequestLogging();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CheckUserStatusMiddleware>();
//app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Map SignalR hubs
app.MapHub<TedarikciHizmet.Api.Hubs.NotifyHub>("/hubs/notify");

app.MapControllers();

app.Run();
