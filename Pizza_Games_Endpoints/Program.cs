using System.Text;
using Microsoft.EntityFrameworkCore;
using Pizza_Games_Endpoints.AuthenticationMethods;
using Pizza_Games_Endpoints.Endpoints;
using Pizza_Games_Endpoints.Models;

var builder = WebApplication.CreateBuilder(args);

// Using user secrets
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Local"))
);

// Using env variables
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionString"))
//);

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
