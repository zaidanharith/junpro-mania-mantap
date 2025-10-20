using System.Threading.Tasks;
using junpro_mania_mantap.Models;
using junpro_mania_mantap.Repositories;

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
            if (user == null) return null;

            if (user.Password == password)
                return user;

            return null;
        }
    }
}
