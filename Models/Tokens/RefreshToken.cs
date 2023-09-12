using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace shoppingApi.Models.AuthenticationModels
{

    public static class DemoRefreshDto
    {
        public static List<RefreshToken> refreshTokenDtos = new List<RefreshToken>();

    }

    public class RefreshToken
    {
        [Required]
        [Key]
        public string UserName { get; set; }

        [Required]
        public string Token { get; set; }

    }
}
