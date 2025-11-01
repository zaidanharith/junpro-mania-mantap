using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace BOZea.Models
{
    public enum OrderItemStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Delivered,
        Cancelled,
        Returned
    }

    public class OrderItem : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("Order")]
        public int OrderID { get; set; }
        public required Order Order { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public required Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public OrderItemStatus Status { get; set; }

        // Review properties
        private int? _reviewID;
        private string? _reviewComment;
        private int _reviewRating;
        private bool _hasReview;
        private string _tempReviewComment = string.Empty;
        private int _tempRating = 5;
        private List<int> _reviewStars = new();

        [NotMapped]
        public int? ReviewID
        {
            get => _reviewID;
            set { _reviewID = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public string? ReviewComment
        {
            get => _reviewComment;
            set { _reviewComment = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public int ReviewRating
        {
            get => _reviewRating;
            set { _reviewRating = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public bool HasReview
        {
            get => _hasReview;
            set { _hasReview = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public string TempReviewComment
        {
            get => _tempReviewComment;
            set { _tempReviewComment = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public int TempRating
        {
            get => _tempRating;
            set { _tempRating = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public List<int> ReviewStars
        {
            get => _reviewStars;
            set { _reviewStars = value; OnPropertyChanged(); }
        }

        public OrderItem() { }

        public OrderItem(Order order, Product product, int quantity)
        {
            Order = order;
            OrderID = order.ID;
            Product = product;
            ProductID = product.ID;
            Quantity = quantity;
            Status = OrderItemStatus.Pending;
            Price = product.Price;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity > 0)
            {
                Quantity = newQuantity;
            }
            else
            {
                throw new ArgumentException("Quantity must be greater than 0");
            }
        }

        public void UpdateStatus(OrderItemStatus newStatus)
        {
            Status = newStatus;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}