using junpro_mania_mantap.Data;
using junpro_mania_mantap.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace junpro_mania_mantap.Repositories
{
    public interface ICategoryRepository
    {
        /// <summary>
        /// Mengambil satu kategori berdasarkan ID-nya.
        /// </summary>
        /// <param name="id">Primary Key dari kategori.</param>
        /// <returns>Objek Category atau null jika tidak ditemukan.</returns>
        Task<Category?> GetByIdAsync(int id);

        /// <summary>
        /// Mengambil satu kategori berdasarkan namanya.
        /// </summary>
        /// <param name="name">Nama kategori.</param>
        /// <returns>Objek Category atau null jika tidak ditemukan.</returns>
        Task<Category?> GetByNameAsync(string name);

        /// <summary>
        /// Mengambil semua kategori yang ada.
        /// </summary>
        /// <returns>Koleksi semua objek Category.</returns>
        Task<IEnumerable<Category?>> GetAllAsync();

        /// <summary>
        /// Menambahkan kategori baru ke database.
        /// </summary>
        Task AddAsync(Category category);

        /// <summary>
        /// Menandai data kategori yang ada untuk diperbarui.
        /// </summary>
        void Update(Category category);

        /// <summary>
        /// Menandai data kategori untuk dihapus.
        /// </summary>
        void Remove(Category category);
    }

    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Category?>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Remove(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}