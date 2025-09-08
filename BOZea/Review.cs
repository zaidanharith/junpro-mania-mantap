using System;

namespace BOZea
{
    public class Review
    {
        // Attributes
        public string ReviewID { get; private set; }
        public int Rating { get; set; } // e.g., 1 to 5
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public string UserID { get; private set; } // ID pengguna yang memberi review
        public string ProductID { get; private set; } // ID produk yang direview

        // Constructor
        public Review(string id, string userID, string productID, int rating, string comment)
        {
            ReviewID = id;
            UserID = userID;
            ProductID = productID;
            Rating = rating;
            Comment = comment;
            ReviewDate = DateTime.Now;
        }

        // Method
        public bool SubmitReview()
        {
            // Logika untuk validasi dan menyimpan review
            if (Rating >= 1 && Rating <= 5)
            {
                Console.WriteLine("Review berhasil disubmit.");
                return true;
            }
            else
            {
                Console.WriteLine("Gagal submit review: Rating tidak valid.");
                return false;
            }
        }
    }
}