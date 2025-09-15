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
        public int ID { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Rating { get; private set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("User")]
        private User owner;

        public Shop(string ID, string Name, string Description, float Rating)
        {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.Rating = Rating;
            this.DateCreated = DateTime.Now;
        }

        public void updateShopProfile(string Name, string Description, float Rating)
        {
            this.Name = Name;
            this.Description = Description;
            this.Rating = Rating;
        }

        public void createProduct(string productID, string Name, string Description, string Category, float Price, int Stock, string Condition, string Status)
        {
            Product newProduct = new Product(productID, Name, Description, Category, Price, Stock, Condition, Status);
        }

        public void updateProduct(Product product, string Name, string Description, string Category, float Price, int Stock, string Condition)
        {
            product.Name = Name;
            product.Description = Description;
            product.Category = Category;
            product.Price = Price;
            product.Stock = Stock;
            product.Condition = Condition;
        }

        public void deleteProduct(Product product)
        {

        }
    }
}