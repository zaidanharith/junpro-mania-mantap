using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public DateTime CreateDate { get; set; }
        public bool HasShop { get; set; }

<<<<<<< HEAD
        public User(int id, string name, string email, string phone, string username, string password, string address, string image, DateTime createDate)
=======
        public User() { }

        public User(int id, string name, string email, string username, string password, string address, string image, DateTime date)
>>>>>>> refs/remotes/origin/master
        {
            ID = id;
            Name = name;
            Email = email;
            Phone = phone;
            Username = username;
            Password = password;
            Address = address;
            Image = image;
            CreateDate = createDate;
            HasShop = false;
        }
        public void updateProfile(string name, string email, string phone, string username, string password, string address, string image)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Username = username;
            Password = password;
            Address = address;
            Image = image;
        }

        public void createShop(int id, string name, string description, float rating)
        {
            Shop newShop = new Shop(id, name, description, rating, this);
            HasShop = true;
        }
    }
}