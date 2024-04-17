using SchoolSuppliesStore.Models;

namespace SchoolSuppliesStore.Areas.Admin.Models
{
    public class Dashboard
    {
        public int orderCount { get; set; }
        public decimal totals { get; set; }
        public int countUser { get; set; }
    }
}
