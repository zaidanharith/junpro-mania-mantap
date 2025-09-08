namespace BOZea
{
    public class User
    {
        public string userID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public bool hasShop { get; set; }

        public User(string userID, string name, string email, string username, string password, string address)
        {
            this.userID = userID;
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

        public void createShop(string shopID, string name, string description, float rating)
        {
            Shop newShop = new Shop(shopID, name, description, rating);
            this.hasShop = true;
        }
    }
}