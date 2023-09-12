using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApi.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        //public int ItemId { get; set; }
        
    }



}

