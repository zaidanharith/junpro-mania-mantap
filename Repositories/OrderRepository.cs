using BOZea.Data;
using BOZea.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOZea.Repositories
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Mengambil satu data pesanan berdasarkan ID-nya secara lengkap.
        /// </summary>
        /// <param name="id">Primary Key dari pesanan.</param>
        /// <returns>Objek Order atau null jika tidak ditemukan.</returns>
        Task<Order?> GetByIdAsync(int id);

        /// <summary>
        /// Mengambil semua pesanan yang dibuat oleh seorang user.
        /// </summary>
        /// <param name="userId">ID dari user.</param>
        /// <returns>Koleksi objek Order.</returns>
        Task<IEnumerable<Order?>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Menambahkan pesanan baru ke database.
        /// </summary>
        Task AddAsync(Order order);

        /// <summary>
        /// Menandai data pesanan yang ada untuk diperbarui.
        /// </summary>
        void Update(Order order);

        /// <summary>
        /// Menandai data pesanan untuk dihapus.
        /// </summary>
        void Remove(Order order);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.ID == id);
        }

        public async Task<IEnumerable<Order?>> GetByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserID == userId)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Remove(Order order)
        {
            _context.Orders.Remove(order);
        }
    }
}