using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOZea.Models
{
    public enum OrderItemStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Delivered,
        Cancelled,
        Returned
    }
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("Order")]
        public int OrderID { get; set; }
        public required Order Order { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public required Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public OrderItemStatus Status { get; set; }

        public OrderItem() { }
        public OrderItem(Order order, Product product, int quantity)
        {
            Order = order;
            OrderID = order.ID;
            Product = product;
            ProductID = product.ID;
            Quantity = quantity;
            Status = OrderItemStatus.Pending;
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

        public void UpdateStatus(OrderItemStatus newStatus)
        {
            Status = newStatus;
        }
    }
}