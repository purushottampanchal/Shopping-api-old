namespace ShoppingApi.Models.Dto
{
    public class AddressDto
    {
        public int UserId { get; set; }
        public string HouseNo { get; set; }
        public string StreetArea { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
    }
}
