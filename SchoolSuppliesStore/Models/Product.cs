using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolSuppliesStore.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        // Lazy-loaded navigation property
        [JsonIgnore]
        public virtual Category? Category { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
        public List<ProductImage>? Images { get; set; }
        public string? Cover { get; set; }
        public bool? IsSaleOff {  get; set; }
    }
}
