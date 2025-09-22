using junpro_mania_mantap.Data;
using junpro_mania_mantap.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace junpro_mania_mantap.Repositories
{
    public interface IProductCategoryRepository
    {
        /// <summary>
        /// Mengambil hubungan spesifik berdasarkan ProductID dan CategoryID.
        /// </summary>
        /// <param name="productId">ID dari Produk.</param>
        /// <param name="categoryId">ID dari Kategori.</param>
        /// <returns>Objek ProductCategory atau null jika tidak ditemukan.</returns>
        Task<ProductCategory?> GetAsync(int productId, int categoryId);

        /// <summary>
        /// Mengambil semua kategori yang terhubung dengan satu produk.
        /// </summary>
        /// <param name="productId">ID dari Produk.</param>
        /// <returns>Koleksi objek ProductCategory.</returns>
        Task<IEnumerable<ProductCategory?>> GetByProductIdAsync(int productId);

        /// <summary>
        /// Menambahkan hubungan baru antara produk dan kategori.
        /// </summary>
        Task AddAsync(ProductCategory productCategory);

        /// <summary>
        /// Menghapus hubungan antara produk dan kategori.
        /// </summary>
        void Remove(ProductCategory productCategory);
    }

    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly AppDbContext _context;

        public ProductCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductCategory?> GetAsync(int productId, int categoryId)
        {
            return await _context.ProductCategories.FindAsync(productId, categoryId);
        }

        public async Task<IEnumerable<ProductCategory?>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductCategories
                .Where(pc => pc.ProductID == productId)
                .Include(pc => pc.Category)
                .ToListAsync();
        }

        public async Task AddAsync(ProductCategory productCategory)
        {
            await _context.ProductCategories.AddAsync(productCategory);
        }

        public void Remove(ProductCategory productCategory)
        {
            _context.ProductCategories.Remove(productCategory);
        }
    }
}