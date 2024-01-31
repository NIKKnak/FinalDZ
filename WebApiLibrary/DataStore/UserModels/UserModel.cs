using WebApiLibrary.DataStore.Entities;

namespace WebApiLibrary.DataStore.Models
{
    public class UserModel
    {
        public Guid? Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public UserRole? Role { get; set; }
    }
}
