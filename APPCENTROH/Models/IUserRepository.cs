using System.Collections.Generic;
using System.Net;

namespace APPCENTROH.Models
{
    public interface IUserRepository
    {
        bool AuthenticateUser(NetworkCredential credential);
        void Add(UserModel userModel);
        UserModel GetByUsername(string username);
        //...

        List<UserModel> GetAllUsers();
    }
}
