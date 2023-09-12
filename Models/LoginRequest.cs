using System.ComponentModel.DataAnnotations;

namespace shoppingApi.Models
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

    }
}

