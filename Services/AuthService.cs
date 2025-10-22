using BOZea.Models;
using BOZea.Repositories;
using BOZea.Helpers;

namespace BOZea.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user != null && PasswordHelper.VerifyPassword(password, user.Password))
            {
                return user;
            }

            return null;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }
    }
}