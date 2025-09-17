using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public enum OrderItemSaleStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Delivered,
        Cancelled
    }
    public class OrderItemSale
    {
        [Key, ForeignKey("OrderItem")]
        public int OrderItemID { get; set; }
        public OrderItem OrderItem { get; set; }

        public OrderItemSaleStatus Status { get; set; }
        public decimal Price { get; set; }

        public OrderItemSale(OrderItem orderItem, decimal price)
        {
            OrderItem = orderItem;
            OrderItemID = orderItem.OrderID;
            Status = OrderItemSaleStatus.Pending;
            Price = price;
        }
    }

}