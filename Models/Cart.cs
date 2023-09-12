namespace ShoppingApi.Models
{
    //to store in db
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CartStatus { get; set; }
        public int CartCost { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
    }

    // for sending and receiving in api
    public class CartResponce
    {
 
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<CarItemtResponceDto> CartItems { get; set; }
        public string CartStatus { get; set; }
        public int CartCost { get; set; }

    }

    public class CarItemtResponceDto
    {
        public int ItemId { get; set; }
        public string name { get; set; }
        public int qty { get; set; }
        public int ItemCost { get; set; }
        public int TotalCost { get; set; } //=itemCost* qty
        public string ImgUrl { get; set; }
        public int BrandId { get; set; }
        public int TypeId { get; set; }
        public string Description { get; set; }

    }
}
