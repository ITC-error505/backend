using Microsoft.EntityFrameworkCore;
using Pizza_Games_Endpoints.Models;

namespace Pizza_Games_Endpoints.Endpoints
{
    public static class GameEndpoint
    {
        public static RouteGroupBuilder MapGameEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("/all", GetGames);
            return group;
        }

        public static async Task<IResult> GetGames(ApplicationDbContext db)
        {
            var games = await db.Games.ToListAsync();
            return Results.Ok(games);
        }
    }
}
