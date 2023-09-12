namespace ShoppingApi.Models.Dto
{
    public class CardDto
    {
        public int UserId { get; set; }
        public string NameOnCard { get; set; }
        public string CardNo { get; set; }
        public string CVV { get; set; }
        public string Expiration { get; set; }
    }
}
