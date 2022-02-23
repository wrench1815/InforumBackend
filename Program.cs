using InforumBackend.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;


// Custom CORS Rule
var CustomCORS = "customCORS";

var builder = WebApplication.CreateBuilder(args);

// var conString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<InforumBackendContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InforumBackendContext")));

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CustomCORS,
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    s =>
    {
        s.SwaggerDoc("v1", new OpenApiInfo { Title = "Inforum Backend API", Version = "v1" });
        s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme.<br/>Enter 'Bearer' [space] and then your token in the text input below.<br/>Example: <code>'Bearer 12345abcdef'</code>",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        s.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,

                },
                new List<string>()
            }
        });
    }
);

// Hook Core Admin
builder.Services.AddCoreAdmin();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<InforumBackendContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            // ValidAudience = Environment.GetEnvironmentVariable("VALID_AUDIENCE"),
            // ValidIssuer = Environment.GetEnvironmentVariable("VALID_ISSUER"),
            // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")))
            ValidAudience = builder.Configuration["JwtValidAudience"],
            ValidIssuer = builder.Configuration["JwtValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecret"]))
        };
    });

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CustomCORS);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// to load static files for Core Admin
app.UseStaticFiles();

// to allow Core Admin to find routes
app.MapDefaultControllerRoute();

app.Run();
