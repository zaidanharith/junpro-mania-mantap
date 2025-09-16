using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class OrderItemRent
    {
        [Key, ForeignKey("OrderItem")]
        public int OrderItemID { get; set; }
        public OrderItem OrderItem { get; set; }

        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }

        public OrderItemRent(OrderItem orderItem, int durationInDays)
        {
            OrderItem = orderItem;
            OrderItemID = orderItem.OrderID;
            DueDate = DateTime.Now.AddDays(durationInDays);
            IsReturned = false;
        }

        public void MarkAsReturned()
        {
            IsReturned = true;
        }
    }

}