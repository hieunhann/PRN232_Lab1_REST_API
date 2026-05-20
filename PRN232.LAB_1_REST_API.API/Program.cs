using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PRN232.LAB_1_REST_API.Repositories;
using PRN232.LAB_1_REST_API.Services;
using PRN232.LAB_1_REST_API.Services.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Yêu cầu format camelCase chuẩn cho API Json
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Bỏ qua giá trị null khi trả về API để làm sạch response
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Cấu hình Database
builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection - Tầng Data (Repository)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Dependency Injection - Tầng Logic (Service)
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();

// Đăng ký AutoMapper (Quét tự động các file Profile)
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<PRN232.LAB_1_REST_API.API.Profiles.MappingProfile>());

// Cấu hình Swagger API Testing & Docs (NSwag cho .NET 9)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "PRN232 LAB 1 REST API";
    config.Version = "v1";
});

var app = builder.Build();

// Khởi chạy các Middleware
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(); // Sinh ra file JSON
    app.UseSwaggerUi(); // Giao diện WebUI
}

// Enable tính năng tự động Seed Database (Migration) nếu DB chưa có
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    dbContext.Database.EnsureCreated(); // Sẽ tự tạo database và Seed Data nếu DB chưa tồn tại
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


// http://localhost:5000/swagger/index.html