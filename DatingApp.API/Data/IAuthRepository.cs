using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
         Task<bool> UserExists(string userName);

         Task<User> Register(User user, string password);

         Task<User> Login(string userName, string password);
         
    }
}