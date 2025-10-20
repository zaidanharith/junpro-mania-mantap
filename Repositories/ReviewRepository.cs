using junpro_mania_mantap.Data;
using junpro_mania_mantap.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace junpro_mania_mantap.Repositories
{
    public interface IReviewRepository
    {
        /// <summary>
        /// Mengambil satu review berdasarkan ID-nya, termasuk data User dan Product.
        /// </summary>
        /// <param name="id">Primary Key dari review.</param>
        /// <returns>Objek Review atau null jika tidak ditemukan.</returns>
        Task<Review?> GetByIdAsync(int id);

        /// <summary>
        /// Mengambil semua review untuk sebuah produk tertentu.
        /// </summary>
        /// <param name="productId">ID dari produk yang direview.</param>
        /// <returns>Koleksi objek Review.</returns>
        Task<IEnumerable<Review?>> GetByProductIdAsync(int productId);

        /// <summary>
        /// Mengambil semua review yang ditulis oleh seorang user.
        /// </summary>
        /// <param name="userId">ID dari user yang menulis review.</param>
        /// <returns>Koleksi objek Review.</returns>
        Task<IEnumerable<Review?>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Menambahkan review baru ke database.
        /// </summary>
        Task AddAsync(Review review);

        /// <summary>
        /// Menandai data review yang ada untuk diperbarui.
        /// </summary>
        void Update(Review review);

        /// <summary>
        /// Menandai data review untuk dihapus.
        /// </summary>
        void Remove(Review review);
    }

    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ID == id);
        }

        public async Task<IEnumerable<Review?>> GetByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductID == productId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review?>> GetByUserIdAsync(int userId)
        {
            return await _context.Reviews
                .Where(r => r.UserID == userId)
                .Include(r => r.Product)
                .ToListAsync();
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public void Update(Review review)
        {
            _context.Reviews.Update(review);
        }

        public void Remove(Review review)
        {
            _context.Reviews.Remove(review);
        }
    }
}