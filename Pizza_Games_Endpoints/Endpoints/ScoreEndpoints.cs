using System.Security.Principal;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pizza_Games_Endpoints.DTOs;
using Pizza_Games_Endpoints.Models;
using static System.Formats.Asn1.AsnWriter;
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
            return group;
        }

        public static async Task<IResult> PostScore([FromBody] Score score, ApplicationDbContext db)
        {
            try
            {
                db.Scores.Add(score);
                await db.SaveChangesAsync();
                return TypedResults.Ok();
            }
            catch (DbUpdateException)
            {
                return TypedResults.BadRequest("Can't add score");
            }
        }

        public static async Task<IResult> GetHighScore(
            int accountId,
            int gameId,
            ApplicationDbContext db
        )
        {
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
    }
}
