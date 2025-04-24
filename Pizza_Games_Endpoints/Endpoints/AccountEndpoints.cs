using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pizza_Games_Endpoints.AuthenticationMethods;
using Pizza_Games_Endpoints.Models;

namespace Pizza_Games_Endpoints.Endpoints
{
    public static class AccountEndpoints
    {
        public static RouteGroupBuilder MapAccountEndpoints(this RouteGroupBuilder group)
        {
            group.MapPost("/login", AuthenticateAccount);
            group.MapPost("/register", CreateAccount);
            group.MapPost("/validateTokenExpiration", ValidateTokenExpiration);
            return group;
        }

        public static async Task<IResult> CreateAccount(
            [FromBody] Account account,
            ApplicationDbContext db
        )
        {
            try
            {
                db.Accounts.Add(account);
                await db.SaveChangesAsync();
                return TypedResults.Ok();
            }
            catch (UniqueConstraintException ex)
            {
                return TypedResults.BadRequest(
                    "Duplicate value at "
                        + ex.ConstraintName.Substring(ex.ConstraintName.IndexOf('_') + 1)
                );
            }
            catch (DbUpdateException)
            {
                return TypedResults.BadRequest("Something went wrong with creating account");
            }
        }

        public static async Task<IResult> AuthenticateAccount(
            Account account,
            ApplicationDbContext db
        )
        {
            try
            {
                var existingAccount = await db.Accounts.FirstOrDefaultAsync(a =>
                    a.username == account.username && a.password == account.password
                );
                if (existingAccount == null)
                {
                    return TypedResults.BadRequest("Invalid username or password");
                }
                string encryptedToken = JWT.GenerateJwtToken(existingAccount);
                return TypedResults.Ok(encryptedToken);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest("Something went wrong with logging in");
            }
        }

        public static async Task<IResult> ValidateTokenExpiration(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expInSeconds = jwtToken.Payload.Expiration;
            var expDate = DateTimeOffset.FromUnixTimeSeconds((long)expInSeconds);
            if (expDate > DateTime.UtcNow)
            {
                return TypedResults.Ok();
            }
            else
            {
                return TypedResults.BadRequest();
            }
        }
    }
}
