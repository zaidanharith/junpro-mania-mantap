using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class Cart
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        public List<CartItem> Items { get; set; }

        public Cart()
        {
            Items = new List<CartItem>();
        }

        public void AddItem(Product product, int quantity)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductID == product.ID);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ProductID = product.ID,
                    Product = product,
                    Quantity = quantity
                });
            }
        }

        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductID == product.ID);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.ProductID == product.ID);
            if (item != null)
            {
                if (quantity > 0)
                    item.Quantity = quantity;
                else
                    RemoveItem(productId);
            }
        }

        public decimal GetTotalPrice()
        {
            return Items.Sum(i => i.Product.Price * i.Quantity);
        }

        public void ClearCart()
        {
            Items.Clear();
        }

        public int GetItemCount()
        {
            return Items.Sum(i => i.Quantity);
        }
    }
}