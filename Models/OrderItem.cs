using System;

namespace BOZea
{
    public class OrderItem
    {
        // Attributes sesuai class diagram
        public string orderItemID { get; private set; }
        public int quantity { get; private set; }
        public float price { get; private set; }  // Total price (quantity * pricePerItem)

        // Additional properties for functionality
        private Product product;
        private Order parentOrder;
        public float pricePerItem { get; private set; }

        // Constructor
        public OrderItem(string id, Product product, int quantity, Order order)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            this.orderItemID = id;
            this.product = product;
            this.quantity = quantity;
            this.parentOrder = order;
            this.pricePerItem = product.Price;
            this.price = quantity * pricePerItem;
        }

        // Getter methods
        public Product GetProduct() => product;
        public Order GetParentOrder() => parentOrder;

        // Additional methods
        public float CalculateSubtotal()
        {
            return quantity * pricePerItem;
        }

        public bool UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                return false;

            if (newQuantity > product.Stock)
                return false;

            this.quantity = newQuantity;
            this.price = CalculateSubtotal();
            return true;
        }

        // Method untuk mendapatkan informasi item
        public string GetItemInfo()
        {
            return $"Product: {product.ProductName}, " +
                   $"Quantity: {quantity}, " +
                   $"Price per item: ${pricePerItem:F2}, " +
                   $"Total: ${price:F2}";
        }
    }
}