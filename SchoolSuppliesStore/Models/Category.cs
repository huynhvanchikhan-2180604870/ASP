using System.ComponentModel.DataAnnotations;

namespace SchoolSuppliesStore.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        // Collection of products (eager loading can create a cycle)
        public ICollection<Product>? Products { get; set; }
    }
}
