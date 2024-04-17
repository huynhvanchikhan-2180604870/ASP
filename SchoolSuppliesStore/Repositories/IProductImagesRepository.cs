using SchoolSuppliesStore.Models;

namespace SchoolSuppliesStore.Repositories
{
    public interface IProductImagesRepository
    {
        Task<IEnumerable<ProductImage>> GetImagesAsync();
        Task<ProductImage> GetImageByIdAsync(int id);
        Task AddAsync(ProductImage product);
        Task UpdateAsync(ProductImage product);
        Task DeleteAsync(int id);
    }
}
