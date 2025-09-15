namespace BOZea
{
    public class Cart
    {
        public int ID { get; set; }
        public List<CartItem> Items { get; }
        public Cart(int id)
        {
            ID = id;
            Items = new List<CartItem>();
        }
        public void AddItem(Product product, int quantity)
        {
            var existingItem = Items.Find(i => i.Product.ID == product.ID);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem(product, quantity));
            }
        }
        public void RemoveItem(int productId)
        {
            var existingItem = Items.Find(i => i.Product.ID == productId);
            if (existingItem != null)
            {
                Items.Remove(existingItem);
            }
        }
        public void UpdateQuantity(int productId, int newQuantity)
        {
            var existingItem = Items.Find(i => i.Product.ID == productId);
            if (existingItem != null)
            {
                existingItem.Quantity = newQuantity;
            }
        }
        public float GetTotalPrice()
        {
            return Items.Sum(i => i.GetTotalPrice());
        }

        public void Checkout()
        {
            var total = GetTotalPrice();
            if (total > 0)
            {
                Console.WriteLine($"Checkout berhasil. Total harga: {total}");
                Items.Clear();
            }
            else
            {
                Console.WriteLine("Keranjang kosong. Tidak dapat checkout.");
            }
        }
    }
}