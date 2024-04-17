namespace SchoolSuppliesStore.Models
{
    public class ViewModels
    {
        public IEnumerable<SchoolSuppliesStore.Models.Product> Products { get; set; }
        public IEnumerable<SchoolSuppliesStore.Models.Category> Categories { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }   
        public int TotalProductsCount { get; set; }
    }
}
