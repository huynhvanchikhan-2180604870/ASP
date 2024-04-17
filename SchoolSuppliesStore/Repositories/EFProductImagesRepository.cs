using Microsoft.EntityFrameworkCore;
using SchoolSuppliesStore.Data;
using SchoolSuppliesStore.Models;

namespace SchoolSuppliesStore.Repositories
{
    public class EFProductImagesRepository : IProductImagesRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductImagesRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ProductImage product)
        {
            _context.ProductImages.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = _context.ProductImages.FirstOrDefault(x => x.Id == id);
            _context.ProductImages.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductImage> GetImageByIdAsync(int id)
        {
            return _context.ProductImages.Include(p => p.Product).FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<ProductImage>> GetImagesAsync()
        {
            return await _context.ProductImages.Include(p => p.Product).ToListAsync();
        }

        public async Task UpdateAsync(ProductImage product)
        {
            _context.ProductImages.Update(product);
            await _context.SaveChangesAsync();
        }

        
    }
}
