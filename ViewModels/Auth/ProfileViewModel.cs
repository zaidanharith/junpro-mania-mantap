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
        private RelayCommand? _navigateHomeCommand;
        private RelayCommand? _logoutCommand;
        private RelayCommand? _submitReviewCommand;

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
        public ICommand NavigateHomeCommand => _navigateHomeCommand ??= new RelayCommand(ExecuteNavigateHome);
        public ICommand LogoutCommand => _logoutCommand ??= new RelayCommand(ExecuteLogout);
        public ICommand SubmitReviewCommand => _submitReviewCommand ??= new RelayCommand(ExecuteSubmitReview);

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

                // Load reviews for each order item
                var reviewRepo = new ReviewRepository(_dbContext);
                foreach (var order in orders)
                {
                    foreach (var item in order.OrderItems)
                    {
                        var existingReview = await reviewRepo.GetByProductIdAsync(item.ProductID);
                        var userReview = existingReview.FirstOrDefault(r => r?.UserID == userId);

                        if (userReview != null)
                        {
                            item.HasReview = true;
                            item.ReviewID = userReview.ID;
                            item.ReviewComment = userReview.Comment;
                            item.ReviewRating = userReview.Rating;
                            item.ReviewStars = Enumerable.Range(1, userReview.Rating).ToList();
                        }
                        else
                        {
                            item.HasReview = false;
                            item.TempRating = 5; // Default rating
                        }
                    }
                }

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

        private void ExecuteNavigateHome(object? parameter)
        {
            try
            {
                Console.WriteLine("[ProfileVM] Navigating back to Dashboard...");

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is ViewModels.MainViewModel mainViewModel)
                {
                    // ✅ Gunakan DashboardViewModel singleton yang sudah terdaftar
                    var dashboardVM = Services.NavigationService.GetDashboardInstance();
                    if (dashboardVM != null)
                    {
                        mainViewModel.CurrentViewModel = dashboardVM;
                        Console.WriteLine("[ProfileVM] Successfully navigated to Dashboard");
                    }
                    else
                    {
                        Console.WriteLine("[ProfileVM] ERROR: DashboardViewModel instance not found!");
                    }
                }
                else
                {
                    Console.WriteLine("[ProfileVM] ERROR: MainViewModel not found!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileVM] Error navigating home: {ex.Message}");
                MessageBox.Show($"Error navigating to dashboard: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExecuteLogout(object? parameter)
        {
            try
            {
                Console.WriteLine("[ProfileVM] Logout requested");

                // Konfirmasi logout
                var result = MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Logout Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    Console.WriteLine("[ProfileVM] Logout cancelled by user");
                    return;
                }

                Console.WriteLine("[ProfileVM] Logging out...");

                // Clear user session
                UserSession.ClearUser();
                Console.WriteLine("[ProfileVM] UserSession cleared");

                // Navigate to Login
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is ViewModels.MainViewModel mainViewModel)
                {
                    // ✅ LANGSUNG AKSES LoginViewModel dari MainViewModel
                    mainViewModel.CurrentViewModel = mainViewModel.LoginViewModel;
                    Console.WriteLine("[ProfileVM] Successfully navigated to Login");

                    MessageBox.Show(
                        "You have been logged out successfully.",
                        "Logout Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    Console.WriteLine("[ProfileVM] ERROR: MainViewModel not found!");
                    MessageBox.Show("Error: Cannot navigate to login page.",
                        "Navigation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileVM] Error during logout: {ex.Message}");
                MessageBox.Show($"Error during logout: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void ExecuteSubmitReview(object? parameter)
        {
            try
            {
                if (parameter is not OrderItem orderItem)
                {
                    Console.WriteLine("[ProfileVM] Invalid parameter for SubmitReview");
                    return;
                }

                if (string.IsNullOrWhiteSpace(orderItem.TempReviewComment))
                {
                    MessageBox.Show("Please enter a review comment.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (orderItem.TempRating < 1 || orderItem.TempRating > 5)
                {
                    MessageBox.Show("Please select a rating between 1-5.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Console.WriteLine($"[ProfileVM] Submitting review for product {orderItem.ProductID}");

                // ✅ Load User dan Product dari database untuk satisfy required constraint
                var user = await _dbContext.Users.FindAsync(CurrentUser!.ID);
                var product = await _dbContext.Products.FindAsync(orderItem.ProductID);

                if (user == null || product == null)
                {
                    Console.WriteLine("[ProfileVM] User or Product not found in database");
                    MessageBox.Show("Error: User or Product not found.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // ✅ Create new review with ALL required fields set
                var review = new Review
                {
                    User = user,              // ✅ Set navigation property
                    UserID = user.ID,
                    Product = product,        // ✅ Set navigation property
                    ProductID = product.ID,
                    Rating = orderItem.TempRating,
                    Comment = orderItem.TempReviewComment,
                    Date = DateTime.UtcNow
                };

                Console.WriteLine($"[ProfileVM] Creating review: UserID={review.UserID}, ProductID={review.ProductID}, Rating={review.Rating}");

                // Save to database
                _dbContext.Reviews.Add(review);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine($"[ProfileVM] Review saved successfully with ID: {review.ID}");

                // Update UI on dispatcher thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    orderItem.ReviewID = review.ID;
                    orderItem.ReviewComment = review.Comment;
                    orderItem.ReviewRating = review.Rating;
                    orderItem.HasReview = true;
                    orderItem.ReviewStars = Enumerable.Range(1, review.Rating).ToList();

                    // Clear temp data
                    orderItem.TempReviewComment = string.Empty;
                    orderItem.TempRating = 5;

                    MessageBox.Show("Review submitted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                });

                Console.WriteLine("[ProfileVM] UI updated with review data");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"[ProfileVM] Database error: {dbEx.Message}");
                Console.WriteLine($"[ProfileVM] Inner exception: {dbEx.InnerException?.Message}");
                Console.WriteLine($"[ProfileVM] Stack trace: {dbEx.StackTrace}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileVM] Error submitting review: {ex.Message}");
                Console.WriteLine($"[ProfileVM] Stack trace: {ex.StackTrace}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error submitting review: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}