using System.Text;
using Microsoft.EntityFrameworkCore;
using Pizza_Games_Endpoints.AuthenticationMethods;
using Pizza_Games_Endpoints.Endpoints;
using Pizza_Games_Endpoints.Models;

var builder = WebApplication.CreateBuilder(args);

// Using env variables
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionString"))
    );
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(Environment.GetEnvironmentVariable("LocalConnectionString"))
    );
}

JWT.ConfigureServices(builder.Services);

builder.Services.AddCors(p =>
    p.AddPolicy(
        "corsapp",
        builder =>
        {
            builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        }
    )
);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("corsapp");

app.MapGroup("/account").MapAccountEndpoints();
app.MapGroup("/score").MapScoreEndpoints();
app.MapGroup("/game").MapGameEndpoints();

app.Run();
