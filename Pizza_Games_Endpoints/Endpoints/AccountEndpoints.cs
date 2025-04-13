using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pizza_Games_Endpoints.Models;

namespace Pizza_Games_Endpoints.Endpoints
{
    public static class AccountEndpoints
    {
        public static RouteGroupBuilder MapAccountEndpoints(this RouteGroupBuilder group)
        {
            group.MapPost("/login", AuthenticateAccount);
            group.MapPost("/register", CreateAccount);
            return group;
        }

        public static async Task<IResult> CreateAccount(Account account, ApplicationDbContext db)
        {
            try
            {
                db.Accounts.Add(account);
                await db.SaveChangesAsync();
                return TypedResults.Ok();
            }
            catch (DbUpdateException)
            {
                return TypedResults.BadRequest("Can't create account");
            }
        }

        public static async Task<IResult> AuthenticateAccount(
            Account account,
            ApplicationDbContext db
        )
        {
            var existingAccount = await db.Accounts.FirstOrDefaultAsync(a =>
                a.username == account.username && a.password == account.password
            );
            if (existingAccount == null)
            {
                return TypedResults.BadRequest("Invalid username or password");
            }
            return TypedResults.Ok(existingAccount.id);
        }
    }
}
