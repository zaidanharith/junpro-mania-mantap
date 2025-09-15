using System;
using System.Collections.Generic;
using System.Linq;

namespace BOZea
{
    public enum OrderStatus
    {
        PendingPayment,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

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

        public void confirmDelivery()
        {
            if (status != OrderStatus.Shipped)
            {
                Console.WriteLine("Order must be shipped before confirming delivery");
                return;
            }

            status = OrderStatus.Delivered;
            Console.WriteLine($"Order {orderID} has been delivered successfully");
        }

        // Additional methods
        private void CalculateTotalAmount()
        {
            totalAmount = Items.Sum(i => i.CalculateSubtotal());
        }

        public bool ProcessPayment(Payment payment)
        {
            if (status != OrderStatus.PendingPayment)
                return false;

            this.payment = payment;
            if (payment.ProcessPayment())
            {
                status = OrderStatus.Processing;
                return true;
            }
            return false;
        }

        public bool ShipOrder()
        {
            if (status != OrderStatus.Processing)
                return false;

            status = OrderStatus.Shipped;
            return true;
        }

        public bool CancelOrder()
        {
            if (status == OrderStatus.Delivered)
                return false;

            status = OrderStatus.Cancelled;
            return true;
        }

        // Getter methods
        public User GetBuyer() => buyer;
        public Payment GetPayment() => payment;
        public OrderStatus GetStatus() => status;

        // Method untuk mendapatkan ringkasan order
        public string GetOrderSummary()
        {
            var summary = $"Order ID: {orderID}\n" +
                         $"Date: {orderDate}\n" +
                         $"Status: {status}\n" +
                         $"Items:\n";

            foreach (var item in Items)
            {
                summary += $"- {item.GetItemInfo()}\n";
            }

            summary += $"Total Amount: ${totalAmount:F2}";
            return summary;
        }
    }
}