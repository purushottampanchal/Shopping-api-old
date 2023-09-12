namespace ShoppingApi.Models.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string UserPhoneNo { get; set; }
        public List<OrderDtoItem> OrderItems { get; set; }
        public int TotalCost{ get; set; } //excluding shipping 
        public int ShippingChareges{ get; set; } // shipping 
        public string OrderStatus{ get; set; }
        public string Remark { get; set; }
    }

    public class OrderDtoItem
    {
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int itemQty { get; set; }
        public int CatlogId { get; set; }
        public int Cost { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

}


