using System.Threading.Tasks;
using Persistence.Models;

namespace Persistence.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
    }
}