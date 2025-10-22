using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOZea.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public required User User { get; set; }

        [ForeignKey("Payment")]
        public int PaymentID { get; set; }
        public required Payment Payment { get; set; }

        public DateTime Date { get; set; }
        public required ICollection<OrderItem> OrderItems { get; set; }

        public Order() { }

        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                return OrderItems?.Sum(item => item.Price * item.Quantity) ?? 0;
            }
        }

        public Order(int id, User user, Payment payment)
        {
            ID = id;
            User = user;
            UserID = user.ID;
            Payment = payment;
            PaymentID = payment.ID;
            Date = DateTime.Now;
            OrderItems = new List<OrderItem>();
        }

        public void AddOrderItem(OrderItem item)
        {
            if (OrderItems == null)
            {
                OrderItems = new List<OrderItem>();
            }
            OrderItems.Add(item);
        }

        public void AssignPayment(Payment payment)
        {
            Payment = payment;
            PaymentID = payment.ID;
        }
    }
}