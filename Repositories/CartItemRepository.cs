using BOZea.Data;
using BOZea.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOZea.Repositories
{
    public interface ICartItemRepository
    {
        /// <summary>
        /// Mengambil satu item spesifik dari keranjang berdasarkan CartID dan ProductID.
        /// </summary>
        /// <param name="cartId">ID dari keranjang belanja.</param>
        /// <param name="productId">ID dari produk.</param>
        /// <returns>Objek CartItem atau null jika tidak ditemukan.</returns>
        Task<CartItem?> GetAsync(int cartId, int productId);

        /// <summary>
        /// Mengambil semua item yang ada di dalam satu keranjang belanja.
        /// </summary>
        /// <param name="cartId">ID dari keranjang belanja.</param>
        /// <returns>Koleksi objek CartItem.</returns>
        Task<IEnumerable<CartItem?>> GetByCartIdAsync(int cartId);

        /// <summary>
        /// Menambahkan item baru ke keranjang.
        /// </summary>
        Task AddAsync(CartItem cartItem);

        /// <summary>
        /// Menandai data item di keranjang untuk diperbarui (misalnya, mengubah kuantitas).
        /// </summary>
        void Update(CartItem cartItem);

        /// <summary>
        /// Menandai item di keranjang untuk dihapus.
        /// </summary>
        void Remove(CartItem cartItem);
    }

    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem?> GetAsync(int cartId, int productId)
        {
            return await _context.CartItems.FindAsync(cartId, productId);
        }

        public async Task<IEnumerable<CartItem?>> GetByCartIdAsync(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartID == cartId)
                .Include(ci => ci.Product)
                .ToListAsync();
        }

        public async Task AddAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
        }

        public void Update(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
        }

        public void Remove(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
        }
    }
}