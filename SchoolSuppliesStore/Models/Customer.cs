namespace SchoolSuppliesStore.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        // Add more properties as needed
        public List<Order> Orders { get; set; }
    }
}
