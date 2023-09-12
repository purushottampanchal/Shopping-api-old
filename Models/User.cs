using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApi.Models
{
    public class DemoUsers
    {
        public static PasswordHasher<string> pw = new PasswordHasher<string>();
        public static List<User> UsersRecords = new List<User>()
        {
            new User(){Id = 1, UserName = "trainer1", HashedPassword = pw.HashPassword("trainer1","123") , Role = "Trainer" },
            new User(){Id = 1, UserName = "admin1", HashedPassword = pw.HashPassword("admin1","123") , Role = "Admin" }
        };
    }

    public class User
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string HashedPassword { get; set; }
        public string Role { get; set; }
    }
}
