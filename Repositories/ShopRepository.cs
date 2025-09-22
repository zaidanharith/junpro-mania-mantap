using junpro_mania_mantap.Data;
using junpro_mania_mantap.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace junpro_mania_mantap.Repositories
{
    public interface IShopRepository
    {
        /// <summary>
        /// </summary>
        /// <param name="id">Primary Key dari toko.</param>
        /// <returns>Objek Shop atau null jika tidak ditemukan.</returns>
        Task<Shop?> GetByIdAsync(int id);

        /// <summary>
        /// Mengambil data toko berdasarkan ID User pemiliknya.
        /// </summary>
        /// <param name="userId">Foreign Key dari User.</param>
        /// <returns>Objek Shop atau null jika tidak ditemukan.</returns>
        Task<Shop?> GetByUserIdAsync(int userId);

        /// <summary>
        /// Mengambil semua toko beserta data pemiliknya (User).
        /// </summary>
        /// <returns>Koleksi objek Shop.</returns>
        Task<IEnumerable<Shop?>> GetAllAsync();

        /// <summary>
        /// Menambahkan toko baru ke database.
        /// </summary>
        Task AddAsync(Shop shop);

        /// <summary>
        /// Menandai data toko yang ada untuk diperbarui.
        /// </summary>
        void Update(Shop shop);

        /// <summary>
        /// Menandai data toko untuk dihapus.
        /// </summary>
        void Remove(Shop shop);
    }

    public class ShopRepository : IShopRepository
    {
        private readonly AppDbContext _context;

        public ShopRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Shop?> GetByIdAsync(int id)
        {
            return await _context.Shops
                .Include(s => s.User)
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.ID == id);
        }

        public async Task<Shop?> GetByUserIdAsync(int userId)
        {
            return await _context.Shops
                .Include(s => s.User)
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.UserID == userId);
        }

        public async Task<IEnumerable<Shop?>> GetAllAsync()
        {
            return await _context.Shops
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task AddAsync(Shop shop)
        {
            await _context.Shops.AddAsync(shop);
        }

        public void Update(Shop shop)
        {
            _context.Shops.Update(shop);
        }

        public void Remove(Shop shop)
        {
            _context.Shops.Remove(shop);
        }
    }
}