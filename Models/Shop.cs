namespace BOZea
{
    public class Shop
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public float rating { get; set; }
        public DateTime dateCreated { get; set; }

        public Shop(string ID, string name, string description, float rating)
        {
            this.ID = ID;
            this.name = name;
            this.description = description;
            this.rating = rating;
            this.dateCreated = DateTime.Now;
        }

        public void updateShopProfile(string name, string description, float rating)
        {
            this.name = name;
            this.description = description;
            this.rating = rating;
        }

        public void createProduct(string productID, string name, string description, string category, float price, int stock, string condition, string status)
        {
            Product newProduct = new Product(productID, name, description, category, price, stock, condition, status);
        }

        public void updateProduct(Product product, string name, string description, string category, float price, int stock, string condition, string status)
        {
            product.name = name;
            product.description = description;
            product.category = category;
            product.price = price;
            product.stock = stock;
            product.condition = condition;
            product.status = status;
        }

        public void deleteProduct(Product product)
        {

        }
    }
}