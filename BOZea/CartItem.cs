namespace BOZea
{
    public class CartItem
    {
        public Product Product { get; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
        public float GetTotalPrice()
        {
            return Product.Price * Quantity;
        }
    }
}