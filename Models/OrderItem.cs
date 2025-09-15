using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class OrderItem
    {
        [ForeignKey("Order")]
        public string OrderID { get; set; }

        [ForeignKey("Product")]
        public string ProductID { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

        public OrderItem(string orderId, string productId, int quantity, decimal price)
        {
            OrderID = orderId;
            ProductID = productId;
            Quantity = quantity;
            Price = price;
        }

        public decimal CalculateSubtotal()
        {
            return Price * Quantity;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity > 0)
            {
                Quantity = newQuantity;
            }
            else
            {
                throw new ArgumentException("Quantity must be greater than 0");
            }
        }
    }
}