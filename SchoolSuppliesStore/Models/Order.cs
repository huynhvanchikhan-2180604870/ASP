using System.ComponentModel.DataAnnotations;

namespace SchoolSuppliesStore.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime OrderDate { get; set; }
        // Add more properties as needed
        public List<OrderDetail>? OrderDetails { get; set; }
        public string? PaymentType { get; set; }
        public bool? Status { get; set; }
    }
}
