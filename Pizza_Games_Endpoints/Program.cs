using System.Text;
using Microsoft.EntityFrameworkCore;
using Pizza_Games_Endpoints.Endpoints;
using Pizza_Games_Endpoints.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Local"))
);

var app = builder.Build();

app.MapGroup("/account").MapAccountEndpoints();

app.Run();
