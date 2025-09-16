using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class OrderRent : Order
    {
        [Key]
        public int ID { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsReturned { get; set; }

        public OrderRent(int durationInDays)
        {
            DueDate = DateTime.Now.AddDays(durationInDays);
            IsReturned = false;
        }

        public void MarkAsReturned()
        {
            IsReturned = true;
        }
    }
}