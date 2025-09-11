using System;
using System.Collections.Generic;
using System.Linq;

namespace BOZea
{
    public class Category
    {
        // Attributes sesuai class diagram
        public string categoryID { get; private set; }
        public string category { get; set; }
        public string description { get; set; }

        // Relasi ke produk
        public List<Product> Products { get; private set; }

        // Constructor
        public Category(string id, string category, string description)
        {
            this.categoryID = id;
            this.category = category;
            this.description = description;
            this.Products = new List<Product>();
        }

        // Method: menambah produk ke kategori
        public void AddProduct(Product product)
        {
            if (product.Category == this.category)
                Products.Add(product);
        }

        // Method: mengambil produk berdasarkan kategori
        public List<Product> GetProductByCategory()
        {
            return Products;
        }
    }
}