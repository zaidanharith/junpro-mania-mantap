using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOZea.Models
{
    public class OrderItemRent
    {
        [Key, ForeignKey("OrderItem")]
        public int OrderItemID { get; set; }
        public required OrderItem OrderItem { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int Duration { get; set; }
        public bool IsReturned { get; set; }

        public OrderItemRent() { }

        public OrderItemRent(OrderItem orderItem, int duration)
        {
            OrderItem = orderItem;
            OrderItemID = orderItem.ID;
            Duration = duration;
            StartDate = null;
            DueDate = null;
            IsReturned = false;
        }

        public void StartRental()
        {
            if (OrderItem.Status == OrderItemStatus.Delivered && StartDate == null)
            {
                StartDate = DateTime.Now;
                DueDate = StartDate.Value.AddDays(Duration);
            }
        }

        public void MarkAsReturned()
        {
            IsReturned = true;
        }
    }

}