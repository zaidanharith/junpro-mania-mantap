using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOZea.Models
{
    public enum ProductTransactionType
    {
        Sale,
        Rent
    }

    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }


        [ForeignKey("Shop")]
        public int ShopID { get; set; }
        public required Shop Shop { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public ProductTransactionType TransactionType { get; set; }
        public required string Image { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public Product() { }

        public Product(int id, string name, string description, decimal price, ProductTransactionType transactionType, int stock, string image, Shop shop)
        {
            ID = id;
            Name = name;
            Description = description;
            Price = price;
            TransactionType = transactionType;
            Stock = stock;
            Image = image;
            Shop = shop;
            ShopID = shop.ID;
            Reviews = new List<Review>();
        }

        public void UpdateTransactionType(ProductTransactionType newTransactionType)
        {
            TransactionType = newTransactionType;
        }
    }
}