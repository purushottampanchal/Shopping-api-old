using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApi.Models
{
    public class OrderDetail
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int TotalItems { get; set; }
        public int ShippingCharges { get; set; }
        public int OrderCost { get; set; } //excluding shipping charges
        public string OrderStatus { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }

    }

    public class OrderItem
    {
        [Key]


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int ItemQty { get; set; }

    }

    //for req

    public class AddOrderDto
    {

        public int UserId{ get; set; }
        public List<OrderItemDto> OrderItems { get; set;}
        public int TotalQty { get; set; }
       // public int ShippingCharges { get; set; }
       // public int OrderCost { get; set; } // calculating on server side
        public string OrderStatus { get; set; }
        public string Remark { get; set; }
        public string Address { get; set; }

    }

    public class OrderItemDto{

        public int ItemId { get; set; }
        public int Qty { get; set; }

    }
}
