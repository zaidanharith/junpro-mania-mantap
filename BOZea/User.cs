namespace BOZea
{
    public class User
    {
        public string ID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public bool hasShop { get; set; }

        public User(string ID, string name, string email, string username, string password, string address)
        {
            this.ID = ID;
            this.name = name;
            this.email = email;
            this.username = username;
            this.password = password;
            this.address = address;
            this.hasShop = false;
        }
        public void updateProfile(string name, string email, string username, string password, string address)
        {
            this.name = name;
            this.email = email;
            this.username = username;
            this.password = password;
            this.address = address;
        }

        public void createShop(string ID, string name, string description, float rating)
        {
            Shop newShop = new Shop(ID, name, description, rating);
            this.hasShop = true;
        }
    }
}