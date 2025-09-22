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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Address { get; set; }
        public string? Image { get; set; }
        public DateTime CreateDate { get; set; }
        public bool HasShop { get; set; }

        public User() { }

        public User(int id, string name, string email, string phone, string username, string password, string address, string image)

        {
            ID = id;
            Name = name;
            Email = email;
            Phone = phone;
            Username = username;
            Password = password;
            Address = address;
            Image = image;
            CreateDate = DateTime.UtcNow;
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

        public void createShop(int id, string name, string description, string address)
        {
            Shop newShop = new Shop
            {
                ID = id,
                Name = name,
                Description = description,
                Address = address,
                User = this,
                UserID = this.ID,
                CreateDate = DateTime.UtcNow,
                Products = new List<Product>()
            };
            HasShop = true;
        }
    }
}