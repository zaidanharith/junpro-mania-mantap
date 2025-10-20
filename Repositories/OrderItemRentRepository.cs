using junpro_mania_mantap.Data;
using junpro_mania_mantap.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace junpro_mania_mantap.Repositories
{
    public interface IOrderItemRentRepository
    {
        /// <summary>
        /// Mengambil detail sewa berdasarkan ID OrderItem-nya.
        /// </summary>
        /// <param name="orderItemId">Primary Key dari OrderItemRent.</param>
        /// <returns>Objek OrderItemRent atau null jika tidak ditemukan.</returns>
        Task<OrderItemRent?> GetByIdAsync(int orderItemId);

        /// <summary>
        /// Mengambil semua data sewa yang masih aktif (belum dikembalikan).
        /// </summary>
        /// <returns>Koleksi objek OrderItemRent yang aktif.</returns>
        Task<IEnumerable<OrderItemRent?>> GetActiveRentalsAsync();

        /// <summary>
        /// Mengambil semua data sewa yang sudah melewati tanggal jatuh tempo.
        /// </summary>
        /// <returns>Koleksi objek OrderItemRent yang telat.</returns>
        Task<IEnumerable<OrderItemRent?>> GetOverdueRentalsAsync();

        /// <summary>
        /// Menambahkan detail sewa baru ke database.
        /// </summary>
        Task AddAsync(OrderItemRent orderItemRent);

        /// <summary>
        /// Menandai detail sewa yang ada untuk diperbarui.
        /// </summary>
        void Update(OrderItemRent orderItemRent);

        /// <summary>
        /// Menandai detail sewa untuk dihapus.
        /// </summary>
        void Remove(OrderItemRent orderItemRent);
    }

    public class OrderItemRentRepository : IOrderItemRentRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItemRent?> GetByIdAsync(int orderItemId)
        {
            return await _context.OrderItemRents
                .Include(oir => oir.OrderItem)
                .FirstOrDefaultAsync(oir => oir.OrderItemID == orderItemId);
        }

        public async Task<IEnumerable<OrderItemRent?>> GetActiveRentalsAsync()
        {
            return await _context.OrderItemRents
                .Where(oir => !oir.IsReturned)
                .Include(oir => oir.OrderItem)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderItemRent?>> GetOverdueRentalsAsync()
        {
            return await _context.OrderItemRents
                .Where(oir => !oir.IsReturned && oir.DueDate < DateTime.Now)
                .Include(oir => oir.OrderItem)
                .ToListAsync();
        }

        public async Task AddAsync(OrderItemRent orderItemRent)
        {
            await _context.OrderItemRents.AddAsync(orderItemRent);
        }

        public void Update(OrderItemRent orderItemRent)
        {
            _context.OrderItemRents.Update(orderItemRent);
        }

        public void Remove(OrderItemRent orderItemRent)
        {
            _context.OrderItemRents.Remove(orderItemRent);
        }
    }
}