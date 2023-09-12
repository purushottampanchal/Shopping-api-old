using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApi.Models
{
    public class Item
    {
 //       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int CatlogId { get; set; }
        public int Cost { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
