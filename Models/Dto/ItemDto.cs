using Newtonsoft.Json;

namespace ShoppingApi.Models.Dto
{
    public class ItemDto
    {
        public string Name { get; set; }
        public int BrandId { get; set; }
        public int CatlogId { get; set; }
        public int Cost { get; set; }
        public string ImageUrl { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
