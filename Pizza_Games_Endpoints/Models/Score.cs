using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Pizza_Games_Endpoints.Models
{
    public class Score
    {
        public int id { get; set; }
        public int accountId { get; set; }
        public Account Account { get; init; }
        public int gameId { get; set; }
        public Game Game { get; init; }
        public int score { get; set; }
    }
}
