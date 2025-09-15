using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class Order
    {
        // Attributes sesuai class diagram
        public string orderID { get; private set; }
        public DateTime orderDate { get; private set; }
        public OrderStatus status { get; private set; }

        // Additional properties
        public float totalAmount { get; private set; }
        public List<OrderItem> Items { get; private set; }

        // Relasi
        private User buyer;
        private Payment payment;

        // Constructor
        public Order(string id, Cart cart, User buyer)
        {
            if (cart == null || !cart.Items.Any())
                throw new ArgumentException("Cart cannot be empty");

            this.orderID = id;
            this.orderDate = DateTime.Now;
            this.status = OrderStatus.PendingPayment;
            this.buyer = buyer;
            this.Items = new List<OrderItem>();

            // Copy items from cart to order
            foreach (var cartItem in cart.Items)
            {
                var orderItem = new OrderItem(
                    Guid.NewGuid().ToString(),
                    cartItem.ItemProduct,
                    cartItem.Quantity,
                    this
                );
                Items.Add(orderItem);
            }

            CalculateTotalAmount();
        }

        // Methods from class diagram
        public void trackOrder()
        {
            Console.WriteLine($"Order {orderID} status: {status}");
            Console.WriteLine($"Order date: {orderDate}");
            Console.WriteLine($"Total amount: ${totalAmount:F2}");

            if (payment != null)
                Console.WriteLine($"Payment status: {payment.PaymentStatus}");
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