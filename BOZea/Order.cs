using System;
using System.Collections.Generic;
using System.Linq;

namespace BOZea
{
    public class Order
    {
        // Attributes
        public string OrderID { get; private set; }
        public DateTime OrderDate { get; private set; }
        public float TotalAmount { get; private set; }
        public string Status { get; set; } // e.g., "Pending", "Shipped", "Delivered"
        public List<OrderItem> Items { get; private set; }

        // Constructor
        public Order(string id, Cart cart)
        {
            OrderID = id;
            OrderDate = DateTime.Now;
            Status = "Pending Payment";
            Items = new List<OrderItem>();

            // Salin item dari keranjang ke pesanan
            foreach (var cartItem in cart.Items)
            {
                var orderItem = new OrderItem(Guid.NewGuid().ToString(), cartItem.ItemProduct, cartItem.Quantity);
                Items.Add(orderItem);
            }
            TotalAmount = Items.Sum(i => i.PricePerItem * i.Quantity);
        }

        // Methods
        public void TrackOrder()
        {
            Console.WriteLine($"Status pesanan {OrderID} saat ini: {Status}.");
        }

        public void ConfirmDelivery()
        {
            Status = "Delivered";
            Console.WriteLine($"Pesanan {OrderID} telah diterima.");
        }
    }
}