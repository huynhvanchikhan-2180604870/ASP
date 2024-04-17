using SchoolSuppliesStore.Models;

namespace SchoolSuppliesStore.Areas.Admin.Models
{
    public class OrdersAndCategorie
    {
        public IEnumerable<Order> ?Orders { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Product> ?Products { get; set; }
    }
}
