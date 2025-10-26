using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BOZea.Data;

namespace BOZea.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public Category() { }

        public Category(int id, string name, string description)
        {
            ID = id;
            Name = name;
            Description = description;
        }

        public List<Product> GetProducts(AppDbContext context)
        {
            var productIds = context.ProductCategories
                .Where(pc => pc.CategoryID == ID)
                .Select(pc => pc.ProductID)
                .ToList();

            return context.Products
                .Where(p => productIds.Contains(p.ID))
                .ToList();
        }
    }
}