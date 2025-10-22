using BOZea.Data;
using BOZea.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOZea.Repositories
{
    public interface IPaymentRepository
    {
        /// <summary>
        /// Mengambil satu data pembayaran berdasarkan ID-nya.
        /// </summary>
        /// <param name="id">Primary Key dari pembayaran.</param>
        /// <returns>Objek Payment atau null jika tidak ditemukan.</returns>
        Task<Payment?> GetByIdAsync(int id);

        /// <summary>
        /// Mengambil semua data pembayaran yang memiliki status tertentu.
        /// </summary>
        /// <param name="status">Status pembayaran (Pending, Success, Failed).</param>
        /// <returns>Koleksi objek Payment.</returns>
        Task<IEnumerable<Payment?>> GetByStatusAsync(PaymentStatus status);

        /// <summary>
        /// Mengambil semua data pembayaran.
        /// </summary>
        /// <returns>Koleksi semua objek Payment.</returns>
        Task<IEnumerable<Payment?>> GetAllAsync();

        /// <summary>
        /// Menambahkan data pembayaran baru ke database.
        /// </summary>
        Task AddAsync(Payment payment);

        /// <summary>
        /// Menandai data pembayaran yang ada untuk diperbarui.
        /// </summary>
        void Update(Payment payment);

        /// <summary>
        /// Menandai data pembayaran untuk dihapus.
        /// </summary>
        void Remove(Payment payment);
    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task<IEnumerable<Payment?>> GetByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment?>> GetAllAsync()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public void Update(Payment payment)
        {
            _context.Payments.Update(payment);
        }

        public void Remove(Payment payment)
        {
            _context.Payments.Remove(payment);
        }
    }
}