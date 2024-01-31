using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApiLibrary;
using WebApiLibrary.Abstraction;
using WebApiLibrary.DataStore.Entities;
using WebApiLibrary.DataStore.Models;

namespace UserApi.Services
{
    public class UserService : IUserService
    {
        private readonly Func<AppDbContext> _context;
        private readonly IMapper _mapper;
        private readonly Account _account;

        public UserService(IMapper mapper, Func<AppDbContext> context, Account account)
        {
            _mapper = mapper;
            _context = context;
            _account = account;
        }
        public UserResponse Authentificate(LoginModel model)
        {
            UserEntity user = null;

            using (var context = _context())
                user = context.Users.Include(x => x.RoleType).FirstOrDefault(x => x.Email == model.Email);

            if (user == null)
            {
                return UserResponse.UserNotFound();
            }

            if (CheckPassword(user.Salt, model.Password, user.Password))
            {
                var response = UserResponse.OK();
                response.Users.Add(_mapper.Map<UserModel>(user));
                return response;
            }

            return UserResponse.PasswordWrong();
        }

        public UserResponse AddAdmin(RegistrationModel model)
        {
            var response = UserResponse.OK();
            using (var context = _context())
            {
                var userExist = context.Users.Count(x => x.RoleType.Role == UserRole.Administrator);

                if (userExist > 0)
                    return UserResponse.AddAdminError();

                var entity = _mapper.Map<UserEntity>(model);

                entity.RoleType = new RoleEntity
                {
                    Role = UserRole.Administrator
                };

                context.Users.Add(entity);
                context.SaveChanges();

                response.UserId = entity.Id;
            }

            return response;
        }

        public UserResponse AddUser(RegistrationModel model)
        {
            var response = UserResponse.OK();
            using (var context = _context())
            {
                var userExist = context.Users.FirstOrDefault(x => x.Email == model.Email.ToLower());

                if (userExist != null)
                    return UserResponse.UserExist();

                var entity = _mapper.Map<UserEntity>(model);
                entity.RoleType = new RoleEntity { Role = UserRole.User };

                context.Users.Add(entity);
                context.SaveChanges();

                response.UserId = entity.Id;
            }
            return response;
        }


        public UserResponse DeleteUser(Guid? userId, string? email)
        {
            if (_account.Role != UserRole.Administrator)
                return UserResponse.AccessDenied();

            using (var context = _context())
            {
                var query = context.Users.Include(x => x.RoleType).AsQueryable();
                if (!string.IsNullOrEmpty(email))
                    query = query.Where(x => x.Email == email);
                if (userId.HasValue)
                    query = query.Where(x => x.Id == userId);


                var userExist = query.FirstOrDefault();

                if (userExist == null)
                    return UserResponse.UserNotFound();

                if (userExist.RoleType.Role == UserRole.Administrator)
                    return new UserResponse
                    {
                        IsSuccess = false,
                        Errors = new List<ErrorModel> { new ErrorModel { Message = "Нельзя удалить администратора" } }
                    };

                context.Users.Remove(userExist);
                context.SaveChanges();
            }
            return UserResponse.OK();
        }

        public UserResponse GetUser(Guid? userId, string? email)
        {
            var user = new UserEntity();

            using (var context = _context())
            {
                var query = context.Users.Include(x => x.RoleType).AsQueryable();
                if (!string.IsNullOrEmpty(email))
                    query = query.Where(x => x.Email == email);
                if (userId.HasValue)
                    query = query.Where(x => x.Id == userId);

                user = query.FirstOrDefault();
            }

            if (user == null)
                return UserResponse.UserNotFound();

            if (_account.Role == UserRole.Administrator || _account.Id == userId)
                return new UserResponse
                {
                    IsSuccess = true,
                    Users = new List<UserModel> { _mapper.Map<UserModel>(user) }
                };

            return UserResponse.AccessDenied();
        }

        public UserResponse GetUsers()
        {
            var users = new List<UserModel>();

            if (_account.Role != UserRole.Administrator)
                return UserResponse.AccessDenied();

            using (var context = _context())
                users.AddRange(context.Users.Include(x => x.RoleType).Select(x => _mapper.Map<UserModel>(x)).ToList());

            return new UserResponse
            {
                IsSuccess = true,
                Users = users
            };
        }


        private bool CheckPassword(byte[] salt, string password, byte[] dbPassword)
        {
            var data = Encoding.ASCII.GetBytes(password).Concat(salt).ToArray();
            SHA512 shaM = new SHA512Managed();
            var pass = shaM.ComputeHash(data);

            return dbPassword.SequenceEqual(pass);
        }

    }
}
