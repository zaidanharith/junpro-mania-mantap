using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class User
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public bool HasShop { get; set; }

        public User(string ID, string Name, string Email, string Username, string Password, string Address)
        {
            this.ID = ID;
            this.Name = Name;
            this.Email = Email;
            this.Username = Username;
            this.Password = Password;
            this.Address = Address;
            this.HasShop = false;
        }
        public void updateProfile(string Name, string Email, string Username, string Password, string Address)
        {
            this.Name = Name;
            this.Email = Email;
            this.Username = Username;
            this.Password = Password;
            this.Address = Address;
        }

        public void createShop(string ID, string Name, string Description, float Rating)
        {
            Shop newShop = new Shop(ID, Name, Description, Rating);
            this.HasShop = true;
        }
    }
}