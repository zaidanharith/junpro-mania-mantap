using BOZea.Data;
using BOZea.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOZea.Repositories
{
    public interface ICartRepository
    {
        /// <summary>
        /// Mengambil data keranjang berdasarkan ID-nya secara lengkap,
        /// termasuk semua item dan detail produk di dalamnya.
        /// </summary>
        /// <param name="id">Primary Key dari keranjang.</param>
        /// <returns>Objek Cart atau null jika tidak ditemukan.</returns>
        Task<Cart?> GetByIdAsync(int id);

        /// <summary>
        /// Mengambil data keranjang milik seorang user secara lengkap.
        /// Ini adalah metode pencarian yang paling umum untuk keranjang.
        /// </summary>
        /// <param name="userId">ID dari user.</param>
        /// <returns>Objek Cart atau null jika tidak ditemukan.</returns>
        Task<Cart?> GetByUserIdAsync(int userId);

        /// <summary>
        /// Menambahkan keranjang baru ke database.
        /// </summary>
        Task AddAsync(Cart cart);

        /// <summary>
        /// Menandai data keranjang yang ada untuk diperbarui.
        /// </summary>
        void Update(Cart cart);

        /// <summary>
        /// Menandai data keranjang untuk dihapus.
        /// </summary>
        void Remove(Cart cart);
    }

    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByIdAsync(int id)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public void Update(Cart cart)
        {
            _context.Carts.Update(cart);
        }

        public void Remove(Cart cart)
        {
            _context.Carts.Remove(cart);
        }
    }
}