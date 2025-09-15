using System;

namespace BOZea
{
    public class Review
    {
        // Attributes sesuai class diagram
        public string reviewID { get; private set; }
        public int rating { get; private set; }
        public string comment { get; set; }
        public DateTime reviewDate { get; private set; }

        // Referensi ke User dan Product
        private User reviewer;
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