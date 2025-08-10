using HospitalManagement.API.Auth;
using HospitalManagement.Core.Repositories;
using HospitalManagement.Infrastructure;
using HospitalManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// JWT options from config
//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
//var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
//var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwtOptions.Issuer,
//            ValidAudience = jwtOptions.Audience,
//            IssuerSigningKey = new SymmetricSecurityKey(key)
//        };
//    });

//builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseMiddleware<HospitalManagement.API.Middleware.JwtMiddleware>();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();
