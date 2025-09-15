using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public enum TransactionType
    {
        Sale,
        Rent
    }

    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Stock { get; set; }
        public TransactionType TransactionType { get; set; }

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