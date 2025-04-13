using System.ComponentModel.DataAnnotations;

namespace Pizza_Games_Endpoints.Models
{
    public class Account
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
