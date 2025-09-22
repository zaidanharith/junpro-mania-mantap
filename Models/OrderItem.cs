using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class OrderItem
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Order")]
        public int OrderID { get; set; }
        public Order Order { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Status Status { get; set; }

        public OrderItem(Order order, Product product, int quantity)
        {
            Order = order;
            OrderID = order.ID;
            Product = product;
            ProductID = product.ID;
            Quantity = quantity;
            Status = Status.Pending;
            Price = product.Price;
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