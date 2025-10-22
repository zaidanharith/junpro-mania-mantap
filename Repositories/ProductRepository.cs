using BOZea.Data;
using BOZea.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOZea.Repositories
{
    public interface IProductRepository
    {
        /// <summary>
        /// Mengambil satu produk berdasarkan ID-nya, termasuk data Toko dan semua Review-nya.
        /// </summary>
        /// <param name="id">Primary Key dari produk.</param>
        /// <returns>Objek Product atau null jika tidak ditemukan.</returns>
        Task<Product?> GetByIdAsync(int id);

        /// <summary>
        /// Mengambil semua produk yang ada di toko tertentu.
        /// </summary>
        /// <param name="shopId">ID dari toko.</param>
        /// <returns>Koleksi objek Product.</returns>
        Task<IEnumerable<Product?>> GetByShopIdAsync(int shopId);

        /// <summary>
        /// Mengambil semua produk beserta data tokonya.
        /// </summary>
        /// <returns>Koleksi objek Product.</returns>
        Task<IEnumerable<Product?>> GetAllAsync();

        /// <summary>
        /// Menambahkan produk baru ke database.
        /// </summary>
        Task AddAsync(Product product);

        /// <summary>
        /// Menandai data produk yang ada untuk diperbarui.
        /// </summary>
        void Update(Product product);

        /// <summary>
        /// Menandai data produk untuk dihapus.
        /// </summary>
        void Remove(Product product);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task<IEnumerable<Product?>> GetByShopIdAsync(int shopId)
        {
            return await _context.Products
                .Where(p => p.ShopID == shopId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product?>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Shop)
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Remove(Product product)
        {
            _context.Products.Remove(product);
        }
    }
}