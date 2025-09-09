using System;
using System.Collections.Generic;
using System.Linq;

namespace BOZea
{
    public class Category
    {
        // Attributes
        public string CategoryID { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Constructor
        public Category(string id, string name, string description)
        {
            CategoryID = id;
            Name = name;
            Description = description;
        }

        // Method
        public List<Product> GetProductByCategory(List<Product> allProducts)
        {
            // Menggunakan LINQ untuk memfilter produk berdasarkan kategori
            return allProducts.Where(p => p.CategoryName == this.Name).ToList();
        }
    }
}