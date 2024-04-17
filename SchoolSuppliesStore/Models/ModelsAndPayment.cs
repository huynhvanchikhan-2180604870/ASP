namespace SchoolSuppliesStore.Models
{
    public class ModelsAndPayment
    {
        public IEnumerable<Order> Orders { get; set; }
        public ApplicationUser User { get; set; }
    }
}
