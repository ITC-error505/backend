using System.Text;
using Microsoft.EntityFrameworkCore;
using Pizza_Games_Endpoints.Endpoints;
using Pizza_Games_Endpoints.Models;

var builder = WebApplication.CreateBuilder(args);

// Using user secrets
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("Remote"))
//);

// Using env variables
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionString"))
);

var app = builder.Build();

app.MapGroup("/account").MapAccountEndpoints();
app.MapGroup("/score").MapScoreEndpoints();
app.MapGroup("/game").MapGameEndpoints();

app.Run();
