using System.ComponentModel.DataAnnotations;

namespace SchoolSuppliesStore.Models
{
    public class ProductType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
