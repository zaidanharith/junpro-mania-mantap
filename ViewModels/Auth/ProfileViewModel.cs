using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BOZea.Data;
using BOZea.Helpers;
using BOZea.Models;
using BOZea.Repositories;
using BOZea.ViewModels.Base;
using Microsoft.EntityFrameworkCore;

namespace BOZea.ViewModels.Auth
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private readonly UserRepository _userRepository;
        private readonly OrderRepository _orderRepository;
        private readonly AppDbContext _dbContext;
        private User? _currentUser;
        private ObservableCollection<Order> _userTransactions;
        private bool _isLoading;

        public ProfileViewModel()
        {
            Console.WriteLine("[ProfileVM] Constructor started");

            var factory = new AppDbContextFactory();
            _dbContext = factory.CreateDbContext(Array.Empty<string>());
            _userRepository = new UserRepository(_dbContext);
            _orderRepository = new OrderRepository(_dbContext);
            _userTransactions = new ObservableCollection<Order>();

            EditProfileCommand = new RelayCommand(_ => EditProfile());
            AddReviewCommand = new RelayCommand(param => AddReview(param as OrderItem));

            Console.WriteLine("[ProfileVM] Constructor completed, loading data...");

            // Load data asynchronously to avoid blocking UI
            Task.Run(async () => await LoadUserDataAsync());
        }

        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(GreetingMessage));
            }
        }

        public ObservableCollection<Order> UserTransactions
        {
            get => _userTransactions;
            set
            {
                _userTransactions = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string GreetingMessage
        {
            get
            {
                var hour = DateTime.Now.Hour;
                var greeting = hour < 12 ? "Good Morning" : hour < 18 ? "Good Afternoon" : "Good Evening";
                return $"{greeting}, {CurrentUser?.Name ?? "User"}!";
            }
        }

        public ICommand EditProfileCommand { get; }
        public ICommand AddReviewCommand { get; }

        private async Task LoadUserDataAsync()
        {
            try
            {
                Console.WriteLine("[ProfileVM] LoadUserDataAsync started");
                IsLoading = true;

                if (!UserSession.IsLoggedIn || !UserSession.CurrentUserId.HasValue)
                {
                    Console.WriteLine("[ProfileVM] User not logged in");
                    IsLoading = false;
                    return;
                }

                int userId = UserSession.CurrentUserId.Value;
                Console.WriteLine($"[ProfileVM] Loading data for user: {userId}");

                // Load user data
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    Console.WriteLine("[ProfileVM] User not found");
                    IsLoading = false;
                    return;
                }

                Console.WriteLine($"[ProfileVM] User loaded: {user.Name}");

                // Update on UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentUser = user;
                });

                // Load transactions with timeout
                Console.WriteLine("[ProfileVM] Loading transactions...");

                var ordersTask = _dbContext.Orders
                    .Where(o => o.UserID == userId)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.Payment)
                    .OrderByDescending(o => o.Date)
                    .Take(20)
                    .ToListAsync();

                // Add 5 second timeout
                var completedTask = await Task.WhenAny(ordersTask, Task.Delay(5000));

                List<Order> orders;
                if (completedTask == ordersTask)
                {
                    orders = await ordersTask;
                    Console.WriteLine($"[ProfileVM] Loaded {orders.Count} transactions");
                }
                else
                {
                    Console.WriteLine("[ProfileVM] Transaction query timeout - using empty list");
                    orders = new List<Order>();
                }

                // Update on UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserTransactions.Clear();
                    foreach (var order in orders)
                    {
                        UserTransactions.Add(order);
                    }
                });

                IsLoading = false;
                Console.WriteLine("[ProfileVM] LoadUserDataAsync completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileVM] Error loading data: {ex.Message}");
                Console.WriteLine($"[ProfileVM] StackTrace: {ex.StackTrace}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsLoading = false;
                });
            }
        }

        private void EditProfile()
        {
            Console.WriteLine("[ProfileVM] Edit profile clicked");
            // TODO: Navigate to edit profile view
        }

        private void AddReview(OrderItem? orderItem)
        {
            if (orderItem == null) return;
            Console.WriteLine($"[ProfileVM] Add review for product: {orderItem.Product?.Name}");
            // TODO: Navigate to add review view
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}