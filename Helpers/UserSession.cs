using BOZea.Models;

namespace BOZea.Helpers
{
    public static class UserSession
    {
        private static User? _currentUser;

        public static User? CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public static bool IsLoggedIn => _currentUser != null;

        public static int? CurrentUserId => _currentUser?.ID;

        public static string? CurrentUserName => _currentUser?.Name;

        public static string? CurrentUserEmail => _currentUser?.Email;

        public static void SetUser(User user)
        {
            _currentUser = user;
        }

        public static void ClearUser()
        {
            _currentUser = null;
        }

        public static bool HasShop => _currentUser?.HasShop ?? false;
    }
}