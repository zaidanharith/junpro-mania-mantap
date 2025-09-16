using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace junpro_mania_mantap.Models
{
    public class Review
    {
        [Key]
        public int ID { get; private set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        [ForeignKey("User")]
        private User reviewer;

        [ForeignKey("Product")]
        private Product reviewedProduct;

        // Constructor
        public Review(string id, User user, Product product, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating harus antara 1-5");

            this.reviewID = id;
            this.reviewer = user;
            this.reviewedProduct = product;
            this.rating = rating;
            this.comment = comment;
            this.reviewDate = DateTime.Now;
        }

        // Method
        public bool submitReview()
        {
            try
            {
                // Validasi dasar
                if (string.IsNullOrEmpty(comment))
                    return false;

                if (rating < 1 || rating > 5)
                    return false;

                // Update rating produk
                reviewedProduct.AddReview(this);

                // Update review history user
                reviewer.AddReview(this);

                Console.WriteLine($"Review untuk produk {reviewedProduct.ProductName} berhasil disubmit.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal submit review: {ex.Message}");
                return false;
            }
        }

        // Getter methods
        public User GetReviewer() => reviewer;
        public Product GetReviewedProduct() => reviewedProduct;
    }
}