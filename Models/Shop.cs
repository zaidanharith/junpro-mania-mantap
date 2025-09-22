using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class Shop
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public float Rating { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<Product> Products { get; set; }

        public Shop(int id, string name, string description, float rating, User user)
        {
            ID = id;
            Name = name;
            Description = description;
            Rating = rating;
            Date = DateTime.Now;
            User = user;
            UserID = user.ID;
            Products = new List<Product>();
        }

        public void updateShopProfile(string name, string description, float rating)
        {
            Name = name;
            Description = description;
            Rating = rating;
        }

        public void createProduct(int productID, string name, string description, decimal price, ProductTransactionType transactionType, int stock, string image)
        {
            Product newProduct = new Product(productID, name, description, price, transactionType, stock, image, this);
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