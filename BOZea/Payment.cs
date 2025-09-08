namespace BOZea
{
    public class Payment
    {
        public string ID { get; set; }
        public string paymentMethod { get; set; }
        public float amount { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public Payment(string ID, string paymentMethod, float amount, DateTime date, string status)
        {
            this.ID = ID;
            this.paymentMethod = paymentMethod;
            this.amount = amount;
            this.date = DateTime.Now;
            this.status = status;
        }

        public void processPayment()
        {
            // Logic to process payment
            this.status = "Processed";
        }

        public void verifyPayment()
        {
            // Logic to verify payment
            this.status = "Verified";
        }

    }
}