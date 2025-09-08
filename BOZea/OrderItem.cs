using System;

namespace BOZea
{
    public class OrderItem
    {
        // Attributes
        public string OrderItemID { get; private set; }
        public string ProductID { get; private set; } // Simpan ID-nya saja
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public float PricePerItem { get; private set; }

        // Constructor
        public OrderItem(string id, Product product, int quantity)
        {
            OrderItemID = id;
            ProductID = product.ProductID;
            ProductName = product.ProductName;
            Quantity = quantity;
            PricePerItem = product.Price;
        }
    }
}