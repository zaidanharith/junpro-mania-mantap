using BOZea.Data;
using BOZea.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOZea.Repositories
{
    public interface IOrderItemRepository
    {
        /// <summary>
        /// Mengambil satu item pesanan berdasarkan ID-nya.
        /// </summary>
        /// <param name="id">Primary Key dari item pesanan.</param>
        /// <returns>Objek OrderItem atau null jika tidak ditemukan.</returns>
        Task<OrderItem?> GetByIdAsync(int id);

        /// <summary>
        /// Mengambil semua item pesanan yang termasuk dalam satu pesanan (Order) tertentu.
        /// </summary>
        /// <param name="orderId">ID dari pesanan (Order).</param>
        /// <returns>Koleksi objek OrderItem.</returns>
        Task<IEnumerable<OrderItem?>> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// Menambahkan item pesanan baru ke database.
        /// </summary>
        Task AddAsync(OrderItem orderItem);

        /// <summary>
        /// Menandai data item pesanan yang ada untuk diperbarui.
        /// </summary>
        void Update(OrderItem orderItem);

        /// <summary>
        /// Menandai data item pesanan untuk dihapus.
        /// </summary>
        void Remove(OrderItem orderItem);
    }

    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem?> GetByIdAsync(int id)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.ID == id);
        }

        public async Task<IEnumerable<OrderItem?>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderID == orderId)
                .Include(oi => oi.Product)
                .ToListAsync();
        }

        public async Task AddAsync(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
        }

        public void Update(OrderItem orderItem)
        {
            _context.OrderItems.Update(orderItem);
        }

        public void Remove(OrderItem orderItem)
        {
            _context.OrderItems.Remove(orderItem);
        }
    }
}