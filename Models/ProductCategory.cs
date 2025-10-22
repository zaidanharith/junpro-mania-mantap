using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOZea.Models
{
    public class ProductCategory
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public required Product Product { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public required Category Category { get; set; }

        public ProductCategory() { }

        public ProductCategory(Product product, Category category)
        {
            Product = product;
            ProductID = product.ID;
            Category = category;
            CategoryID = category.ID;
        }

        public void UpdateCategory(Category category)
        {
            Category = category;
            CategoryID = category.ID;
        }
    }
}