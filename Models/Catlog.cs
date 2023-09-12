using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApi.Models
{
    public class Catlog
    {
 //       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public string Name { get; set; }

        public string CatalogCover { get; set; }
        public int TotalItems { get; set; }


    }
}
