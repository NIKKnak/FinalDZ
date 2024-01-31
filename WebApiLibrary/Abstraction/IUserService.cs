using WebApiLibrary.DataStore.Models;

namespace WebApiLibrary.Abstraction
{
    public interface IUserService
    {

        UserResponse Authentificate(LoginModel model);
        UserResponse AddUser(RegistrationModel model);
        UserResponse AddAdmin(RegistrationModel model);
        UserResponse GetUsers();
        UserResponse GetUser(Guid? userId, string? email);
        UserResponse DeleteUser(Guid? userId, string? email);
    }
}
