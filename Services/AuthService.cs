using junpro_mania_mantap.Models;
using junpro_mania_mantap.Repositories;
using junpro_mania_mantap.Helpers;

namespace junpro_mania_mantap.Services
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