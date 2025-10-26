using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
 using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters()
//         {
//             ClockSkew = TimeSpan.Zero,
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
//             ),
//         };
//     });

// builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
// {
//     builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
// }));
var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("corsapp");

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
Path.Combine(Directory.GetCurrentDirectory(), "Static")),
    RequestPath = "/Static",
    EnableDefaultFiles = true
});

app.MapControllers();

app.Run();
