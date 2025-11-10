using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BOZea.Helpers;
using BOZea.Models;

namespace BOZea.ViewModels.Auth
{
    public class OrderDisplayViewModel : INotifyPropertyChanged
    {
        private readonly Models.Order _order;

        public OrderDisplayViewModel(Models.Order orderModel)
        {
            _order = orderModel;
            
            // Wrap OrderItems
            OrderItems = new ObservableCollection<OrderItemViewModel>();
            if (_order.OrderItems != null)
            {
                foreach (var item in _order.OrderItems)
                {
                    OrderItems.Add(new OrderItemViewModel(item));
                }
            }
        }

        public int ID => _order.ID;
        public DateTime Date => _order.Date;
        public Models.Payment Payment => _order.Payment;
        public Models.User User => _order.User;

        public ObservableCollection<OrderItemViewModel> OrderItems { get; }

        public decimal TotalAmount => _order.TotalPrice;

        public string FormattedTotalAmount => CurrencyManager.Instance.FormatPrice(TotalAmount);

        public void RefreshPrices()
        {
            OnPropertyChanged(nameof(FormattedTotalAmount));
            foreach (var item in OrderItems)
            {
                item.RefreshPrice();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class OrderItemViewModel : INotifyPropertyChanged
    {
        private readonly OrderItem _orderItem;

        public OrderItemViewModel(OrderItem orderItem)
        {
            _orderItem = orderItem;
            
            // Subscribe to OrderItem property changes to forward them
            _orderItem.PropertyChanged += (s, e) =>
            {
                // Forward all property changes
                OnPropertyChanged(e.PropertyName);
            };
        }

        public int ID => _orderItem.ID;
        public Models.Product Product => _orderItem.Product;
        public int Quantity => _orderItem.Quantity;
        public decimal Price => _orderItem.Price;
        public OrderItemStatus Status => _orderItem.Status;

        // Review properties - delegate ke OrderItem
        public int? ReviewID
        {
            get => _orderItem.ReviewID;
            set => _orderItem.ReviewID = value;
        }

        public bool HasReview
        {
            get => _orderItem.HasReview;
            set => _orderItem.HasReview = value;
        }

        public int ReviewRating
        {
            get => _orderItem.ReviewRating;
            set => _orderItem.ReviewRating = value;
        }

        public string? ReviewComment
        {
            get => _orderItem.ReviewComment;
            set => _orderItem.ReviewComment = value;
        }

        public string TempReviewComment
        {
            get => _orderItem.TempReviewComment;
            set => _orderItem.TempReviewComment = value;
        }

        public int TempRating
        {
            get => _orderItem.TempRating;
            set => _orderItem.TempRating = value;
        }

        public List<int> ReviewStars
        {
            get => _orderItem.ReviewStars;
            set => _orderItem.ReviewStars = value;
        }

        public string FormattedPrice => CurrencyManager.Instance.FormatPrice(Price * Quantity);

        public void RefreshPrice()
        {
            OnPropertyChanged(nameof(FormattedPrice));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
