using Clinic.API;
using Clinic.Application;
using Clinic.Infrastructure.Sqlserver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructureSqlServer(builder.Configuration);

// Add Authentication and Authorization services
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = ClaimTypes.Role
        };
    });
// Register permission policy provider and handler for dynamic permission policies
builder.Services.AddAuthorization();
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider, Clinic.API.Authorization.PermissionPolicyProvider>();
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, Clinic.API.Authorization.PermissionAuthorizationHandler>();

// Register current-user accessor (đọc UserId từ ClaimsPrincipal của request hiện tại)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Clinic.Application.Interfaces.ICurrentUser, Clinic.API.Authentication.CurrentUser>();

// Cấu hình business rule của Appointment (vd: số giờ tối thiểu Patient phải đổi lịch trước hạn), đọc từ section "Appointment".
builder.Services.Configure<Clinic.API.Configuration.AppointmentPolicySettings>(
    builder.Configuration.GetSection(Clinic.API.Configuration.AppointmentPolicySettings.SectionName));
builder.Services.AddScoped<Clinic.Application.Interfaces.IAppointmentPolicySettings>(sp =>
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<Clinic.API.Configuration.AppointmentPolicySettings>>().Value);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Bắt mọi exception chưa xử lý và trả về envelope ApiResponse (qua GlobalExceptionHandler).
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();