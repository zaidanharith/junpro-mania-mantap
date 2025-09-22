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

        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public int Duration { get; set; }
        public bool IsReturned { get; set; }

        public OrderItemRent(OrderItem orderItem, int duration)
        {
            OrderItem = orderItem;
            OrderItemID = orderItem.OrderID;
            StartDate = DateTime.Now;
            DueDate = StartDate.AddDays(duration);
            IsReturned = false;
        }

        public void MarkAsReturned()
        {
            IsReturned = true;
        }
    }

}