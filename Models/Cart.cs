using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOZea.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public required User User { get; set; }

        public required ICollection<CartItem> Items { get; set; }

        public Cart() { }

        public Cart(int id, User user)
        {
            ID = id;
            User = user;
            UserID = user.ID;
            Items = new List<CartItem>();
        }

        public void AddItem(Product product, int quantity)
        {
            var existingItem = Items.FirstOrDefault(item => item.ProductID == product.ID);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    Cart = this,
                    CartID = this.ID,
                    Product = product,
                    ProductID = product.ID,
                    Quantity = quantity
                });
            }
        }

        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(item => item.ProductID == productId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(item => item.ProductID == productId);
            if (item != null)
            {
                if (quantity > 0)
                    item.UpdateQuantity(quantity);
                else
                    RemoveItem(productId);
            }
        }

        public decimal GetTotalPrice()
        {
            return Items.Sum(item => (decimal)item.GetTotalPrice());
        }

        public void ClearCart()
        {
            Items.Clear();
        }

        public int GetItemCount()
        {
            return Items.Sum(item => item.Quantity);
        }
    }
}