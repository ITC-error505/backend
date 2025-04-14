using Microsoft.EntityFrameworkCore;
using Pizza_Games_Endpoints.Endpoints;
using Pizza_Games_Endpoints.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Local"))
);

var app = builder.Build();

app.MapGroup("/account").MapAccountEndpoints();
app.MapGroup("/score").MapScoreEndpoints();

app.Run();
