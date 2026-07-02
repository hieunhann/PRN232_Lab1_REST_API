using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NSwag.Generation.Processors.Security;
using PRN232.LAB_2_REST_API.API.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using PRN232.LAB_2_REST_API.Repositories;
using PRN232.LAB_2_REST_API.Services;
using PRN232.LAB_2_REST_API.Services.Interfaces;
using PRN232.LAB_2_REST_API.Services.Validators;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════════════════
// 1. CONTROLLERS + Content Negotiation (JSON & XML)
// ═══════════════════════════════════════════════════════════════════════════
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    // JSON: camelCase convention
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    // Ignore null values in output
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    // Handle circular references
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
})
.AddXmlSerializerFormatters(); // Support application/xml

// ═══════════════════════════════════════════════════════════════════════════
// CORS & RATE LIMITING & HEALTH CHECKS
// ═══════════════════════════════════════════════════════════════════════════
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
    options.RejectionStatusCode = 429;
});

builder.Services.AddHealthChecks();

// ═══════════════════════════════════════════════════════════════════════════
// 2. API VERSIONING
// ═══════════════════════════════════════════════════════════════════════════
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Return header: api-supported-versions
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),   // /api/v1/students
        new HeaderApiVersionReader("x-api-version"), // Header: x-api-version: 1
        new QueryStringApiVersionReader("api-version") // ?api-version=1
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ═══════════════════════════════════════════════════════════════════════════
// 3. DATABASE
// ═══════════════════════════════════════════════════════════════════════════
builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ═══════════════════════════════════════════════════════════════════════════
// 4. DEPENDENCY INJECTION
// ═══════════════════════════════════════════════════════════════════════════
// Repository Layer
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Service Layer
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<PRN232.LAB_2_REST_API.API.Profiles.MappingProfile>());

// HttpContextAccessor (required for AuditLog UserId injection from JWT)
builder.Services.AddHttpContextAccessor();

// ═══════════════════════════════════════════════════════════════════════════
// 5. FLUENT VALIDATION
// ═══════════════════════════════════════════════════════════════════════════
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<StudentRequestValidator>();

// ═══════════════════════════════════════════════════════════════════════════
// 6. JWT AUTHENTICATION & AUTHORIZATION
// ═══════════════════════════════════════════════════════════════════════════
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured!");
var issuer = jwtSettings["Issuer"] ?? "PRN232Lab2API";
var audience = jwtSettings["Audience"] ?? "PRN232Lab2Client";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Không cho phép sai lệch thời gian
    };

    // Trả về response lỗi chuẩn khi không có quyền
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = "You must be logged in to access this resource. Please provide a valid JWT Bearer Token.",
                errors = "Unauthorized"
            });
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = "You do not have permission to access this resource. Higher Role required.",
                errors = "Forbidden"
            });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization();

// ═══════════════════════════════════════════════════════════════════════════
// 7. SWAGGER / OPENAPI (với JWT Auth button)
// ═══════════════════════════════════════════════════════════════════════════
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "PRN232 LAB 2 REST API";
    config.Version = "v2";
    config.Description = "Advanced REST API for Learning Management System. Supports JWT Authentication, API Versioning, Content Negotiation (JSON/XML), FluentValidation, and more.";

    // Add JWT Security Definition
    config.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste your JWT Token here. The 'Bearer' prefix will be added automatically by Swagger."
    });

    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});

var app = builder.Build();

// ═══════════════════════════════════════════════════════════════════════════
// 8. MIDDLEWARE PIPELINE (Thứ tự QUAN TRỌNG!)
// ═══════════════════════════════════════════════════════════════════════════

// 1. Global Exception Handler (phải đứng đầu tiên)
app.UseMiddleware<GlobalExceptionMiddleware>();

// 2. Request Logging
app.UseMiddleware<RequestLoggingMiddleware>();

// 3. Swagger UI (chỉ trong Development)
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(); // Sinh file JSON /swagger/v1/swagger.json
    app.UseSwaggerUi(); // Giao diện WebUI /swagger/index.html
}

// 4. Auto Seed/Migrate Database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    dbContext.Database.EnsureCreated();

    // Runtime seed: Đảm bảo user admin/user luôn tồn tại với password '123456'
    // Giống cơ chế DbInitializer của Lab1 - tạo hash tại runtime để đảm bảo chính xác
    var adminUser = dbContext.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Username == "admin");
    var regularUser = dbContext.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Username == "user");

    if (adminUser == null)
    {
        dbContext.Users.Add(new PRN232.LAB_2_REST_API.Repositories.Entities.User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "Admin"
        });
    }
    else
    {
        // Cập nhật lại hash nếu đang dùng hash cũ (Admin@123)
        if (!BCrypt.Net.BCrypt.Verify("123456", adminUser.PasswordHash))
        {
            adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        }
    }

    if (regularUser == null)
    {
        dbContext.Users.Add(new PRN232.LAB_2_REST_API.Repositories.Entities.User
        {
            Username = "user",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "User"
        });
    }
    else
    {
        if (!BCrypt.Net.BCrypt.Verify("123456", regularUser.PasswordHash))
        {
            regularUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        }
    }

    dbContext.SaveChanges();
}

// 5. HTTPS Redirect
app.UseHttpsRedirection();

// 5.5 CORS & Rate Limiting
app.UseCors();
app.UseRateLimiter();

// 6. Authentication TRƯỚC Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6.5 Health Checks
app.MapHealthChecks("/health");

// 7. Map Controllers
app.MapControllers();

app.Run();

// http://localhost:5000/swagger/index.html
// Test Login: POST /api/v1/auth/login { "username": "admin", "password": "Admin@123" }