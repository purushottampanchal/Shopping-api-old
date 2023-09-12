using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApi.Models
{
    public class Brand
    {
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string BrandCover { get; set; }
        public int TotalItems { get; set; }
    }
}
