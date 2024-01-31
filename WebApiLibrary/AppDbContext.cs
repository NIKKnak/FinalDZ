using Microsoft.EntityFrameworkCore;
using WebApiLibrary.DataStore.Entities;

namespace WebApiLibrary
{
    public partial class AppDbContext : DbContext
    {
        private readonly string _connectionString;
        public AppDbContext()
        {

        }

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        /*
         dotnet ef migrations add InitialCreate --context AppDbContext
         dotnet ef database update
        */
        public virtual DbSet<MessageEntity> Messages { get; set; }
        public virtual DbSet<UserEntity> Users { get; set; }
        public virtual DbSet<RoleEntity> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Email).IsUnique();

                entity.Property(e => e.Password)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasMaxLength(255);

                entity.HasOne(x => x.RoleType)
                    .WithMany(x => x.Users);

            });

            modelBuilder.Entity<MessageEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.SenderId);
                entity.Property(x => x.RecipientId);

                entity.Property(e => e.Text)
                    .HasMaxLength(1000);

                entity.HasOne(x => x.Sender)
                    .WithMany(x => x.SendMessages)
                    .HasForeignKey(x => x.SenderId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Recipient)
                    .WithMany(x => x.ReceiveMessages)
                    .HasForeignKey(x => x.RecipientId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
