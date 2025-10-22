using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOZea.Models
{
    public class Shop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public required User User { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public required ICollection<Product> Products { get; set; }

        public Shop() { }
        public Shop(int id, string name, string description, string address, User user)
        {
            ID = id;
            Name = name;
            Description = description;
            Address = address;
            CreateDate = DateTime.UtcNow;
            User = user;
            UserID = user.ID;
            Products = new List<Product>();
        }

        public void updateShopProfile(string name, string description, string address)
        {
            Name = name;
            Description = description;
            Address = address;
        }

        // Computed property untuk mendapatkan average rating dari semua review produk
        [NotMapped]
        public float AverageRating
        {
            get
            {
                var allReviews = Products
                    .Where(p => p.Reviews != null && p.Reviews.Any())
                    .SelectMany(p => p.Reviews)
                    .ToList();

                return allReviews.Any() ? (float)allReviews.Average(r => r.Rating) : 0;
            }
        }

        public void createProduct(int productID, string name, string description, decimal price, ProductTransactionType transactionType, int stock, string image)
        {
            Product newProduct = new Product
            {
                ID = productID,
                Name = name,
                Description = description,
                Price = price,
                TransactionType = transactionType,
                Stock = stock,
                Image = image,
                Shop = this,
                ShopID = this.ID,
                Reviews = new List<Review>()
            };
            Products.Add(newProduct);
        }

        public void updateProduct(Product product, string name, string description, decimal price, int stock)
        {
            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.Stock = stock;

        }

        public void deleteProduct(Product product)
        {
            Products.Remove(product);
        }
    }
}