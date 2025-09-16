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
        public int ID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        [ForeignKey("Payment")]
        public int PaymentID { get; set; }
        public Payment Payment { get; set; }

        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

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
            CalculatePrice();
        }

        public void CalculatePrice()
        {
            Price = OrderItems.Sum(item => item.Product.Price * item.Quantity);
        }

        public void AssignPayment(Payment payment)
        {
            Payment = payment;
            PaymentID = payment.ID;
        }
    }
}