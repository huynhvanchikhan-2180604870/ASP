using SchoolSuppliesStore.Models;

namespace SchoolSuppliesStore.ShopingModels
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price => Product.Price * Quantity;
    }
}
