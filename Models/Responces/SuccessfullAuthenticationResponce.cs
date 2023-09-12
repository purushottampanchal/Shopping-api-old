namespace shoppingApi.Models.Responces
{
    public class SuccessfullAuthenticationResponce
    {

        public string AccessToken { get; set; }


        //public DateTime AccessTokenExpirationTime { get; set; }
        
        public string RefreshToken { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }

    }
}
