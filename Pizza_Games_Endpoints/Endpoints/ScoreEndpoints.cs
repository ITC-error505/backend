using System.Security.Claims;
using System.Security.Principal;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pizza_Games_Endpoints.DTOs;
using Pizza_Games_Endpoints.Models;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.WebRequestMethods;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Pizza_Games_Endpoints.Endpoints
{
    public static class ScoreEndpoints
    {
        public static RouteGroupBuilder MapScoreEndpoints(this RouteGroupBuilder group)
        {
            group.MapPost("/post", PostScore);
            group.MapGet("/highScore", GetHighScore);
            group.MapGet("/leaderboard", GetLeaderboard);
            group.MapGet("/sse", PageRefreshManager);
            return group;
        }

        private static bool brokeHighScore = false;

        [Authorize]
        public static async Task<IResult> PostScore(
            [FromBody] Score score,
            ApplicationDbContext db,
            HttpContext http
        )
        {
            try
            {
                // Get the userId from the token
                var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                score.accountId = int.Parse(userId);

                db.Scores.Add(score);
                await db.SaveChangesAsync();

                var higherScoreCount = await db
                    .Scores.Where(s => s.gameId == score.gameId && s.score >= score.score)
                    .CountAsync();
                if (higherScoreCount < 10)
                {
                    brokeHighScore = true;
                }

                return TypedResults.Ok();
            }
            catch (DbUpdateException)
            {
                return TypedResults.BadRequest("Can't add score");
            }
        }

        [Authorize]
        public static async Task<IResult> GetHighScore(
            int gameId,
            ApplicationDbContext db,
            HttpContext http
        )
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int accountId = int.Parse(userId);
            var highScore = await db
                .Database.SqlQuery<HighScoreDTO>(
                    $@"
                    SELECT score AS ""highScore"", rank FROM (
                        SELECT 
                            score, DENSE_RANK() OVER (ORDER BY ""Scores"".score DESC) AS ""rank"", ""accountId""
                        FROM ""Scores""
                        WHERE ""gameId"" = {gameId}
                    ) AS ""RankedScores""
                    WHERE ""accountId"" = {accountId}
                    LIMIT 1"
                )
                .FirstOrDefaultAsync();
            return TypedResults.Ok(highScore);
        }

        public static async Task<IResult> GetLeaderboard(int gameId, ApplicationDbContext db)
        {
            var leaderboard = await db
                .Database.SqlQuery<LeaderboardDTO>(
                    $@"
                    SELECT ""Accounts"".username AS ""username"", score, DENSE_RANK() OVER(ORDER BY score DESC) AS ""rank""
                    FROM ""Scores"" 
                    INNER JOIN ""Games"" ON ""Scores"".""gameId"" = ""Games"".id 
                    INNER JOIN ""Accounts"" ON ""Scores"".""accountId"" = ""Accounts"".id 
                    WHERE ""gameId"" = {gameId}
                    ORDER BY score DESC;
                    "
                )
                .ToListAsync();
            return TypedResults.Ok(leaderboard);
        }

        public static async Task PageRefreshManager(HttpContext context)
        {
            context.Response.Headers.Add("Content-Type", "text/event-stream");
            while (true)
            {
                if (brokeHighScore)
                {
                    await context.Response.WriteAsync($"data: Refresh at {DateTime.Now}\n\n");
                    await context.Response.Body.FlushAsync();
                    brokeHighScore = false;
                }

                await Task.Delay(1000); // Check every 1 second to see if it should refresh
            }
        }
    }
}
