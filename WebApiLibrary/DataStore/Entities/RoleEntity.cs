namespace WebApiLibrary.DataStore.Entities
{
    public class RoleEntity
    {
        public Guid Id { get; set; }
        public UserRole Role { get; set; }
        public virtual List<UserEntity>? Users { get; set; }
    }
}
