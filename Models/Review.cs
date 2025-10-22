using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOZea.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public required User User { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public required Product Product { get; set; }

        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime Date { get; set; }

        public Review() { }

        public Review(int id, User user, Product product, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating harus antara 1-5");
            ID = id;
            User = user;
            UserID = user.ID;
            Product = product;
            ProductID = product.ID;
            Rating = rating;
            Comment = comment;
            Date = DateTime.Now;
        }

        public bool SubmitReview()
        {
            try
            {
                Product.Reviews.Add(this);
                // Rating shop akan otomatis dihitung dari AverageRating property
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal submit review: {ex.Message}");
                return false;
            }
        }

        public User GetReviewer() => User;
        public Product GetReviewedProduct() => Product;
    }
}