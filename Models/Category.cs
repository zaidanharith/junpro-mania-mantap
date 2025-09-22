using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ID { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public Category() { }

        public Category(int id, string name, string description)
        {
            ID = id;
            Name = name;
            Description = description;
        }
    }
}