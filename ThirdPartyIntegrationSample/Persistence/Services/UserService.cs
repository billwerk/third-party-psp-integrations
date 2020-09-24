using System.Threading.Tasks;
using Persistence.Interfaces;
using Persistence.Models;

namespace Persistence.Services
{
    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private readonly IGlobalSettings _globalSettings; 

        public UserService(IGlobalSettings globalSettings)
        {
            _globalSettings = globalSettings;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            return await Task.Run(() =>
            {
                if (_globalSettings.TestUser.UserName != username || _globalSettings.TestUser.Password != password)
                    return null;
                
                var user = new User
                {
                    UserName = _globalSettings.TestUser.UserName,
                    FirstName = _globalSettings.TestUser.FirstName,
                    LastName = _globalSettings.TestUser.LastName
                };
                
                user.ForceId();

                return user;

            });
        }
    }
}