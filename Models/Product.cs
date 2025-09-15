namespace BOZea
{
    public enum Condition
    {
        New = 1,
        Good = 2,
        Fair = 3,
        Poor = 4
    }
    public enum Status
    {
        Available = 1,
        Sold = 2,
        Rented = 3
    }
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public float Price { get; set; }
        public float RentPRice { get; set; }
        public int Stock { get; set; }

        public Condition Condition { get; set; }
        public Status Status { get; set; }

        public Product(int ID, string Name, float Price)
        {
            this.ID = ID;
            this.Name = Name;
            this.Price = Price;
        }

        public void UpdateStatus(Status newStatus)
        {
            Status = newStatus;
        }
    }
}