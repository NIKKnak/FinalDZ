using WebApiLibrary.DataStore.Models;

namespace WebApiLibrary
{
    public class Account : UserModel
    {
        private string? Token { get; set; }

        public string RefreshToken(string newToken)
        {
            Token = newToken;
            return Token;
        }
        public string GetAccessToken() => Token;

        public void Logout()
        {
            Id = null;
            Email = null;
            Name = null;
            Surname = null;
            Role = null;
            Token = null;
        }
        public void Login(UserModel model)
        {
            Id = model.Id;
            Email = model.Email;
            Name = model.Name;
            Surname = model.Surname;
            Role = model.Role;
        }
    }
}
