using Microsoft.EntityFrameworkCore;
using InforumBackend.Data;

// Custom CORS Rule
var CustomCORS = "customCORS";

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSwaggerGen();

// Hook Core Admin
builder.Services.AddCoreAdmin();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseCors(CustomCORS);

// to load static files for Core Admin
app.UseStaticFiles();

// to allow Core Admin to find routes
app.MapDefaultControllerRoute();

app.Run();
