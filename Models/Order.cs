using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class Order
    {
        [Key]
        public int ID { get; private set; }

        public DateTime Date { get; private set; }
        public string Status { get; set; }
        public decimal Price { get; private set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        
        [ForeignKey("Payment")]
        public int PaymentID { get; set; }

        public virtual User User { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; private set; }

        public Order(int orderId, int userId)
        {
            ID = orderId;
            UserID = userId;
            Date = DateTime.Now;
            Status = "Pending";
            OrderItems = new List<OrderItem>();
        }

        public void AddOrderItem(OrderItem item)
        {
            OrderItems.Add(item);
            CalculatePrice();
        }

        private void CalculatePrice()
        {
            decimal total = 0;
            foreach (var item in OrderItems)
            {
                total += item.Price * item.Quantity;
            }
            Price = total;
        }

        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }

        public void AssignPayment(string paymentId)
        {
            PaymentID = paymentId;
        }
    }
}