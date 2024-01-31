using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using WebApiLibrary;
using WebApiLibrary.DataStore.Entities;
using WebApiLibrary.DataStore.Models;

namespace UserApi.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserEntity, UserModel>().ConvertUsing(new EntityToModelConverter());
            CreateMap<UserEntity, Account>(MemberList.Destination);
            CreateMap<RegistrationModel, UserEntity>().ConvertUsing(new RegisterEntityConverter());
        }

        private class RegisterEntityConverter : ITypeConverter<RegistrationModel, UserEntity>
        {
            public UserEntity Convert(RegistrationModel source, UserEntity destination, ResolutionContext context)
            {

                var entity = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Email = source.Email.ToLower(),
                    Name = source.Name,
                    Surname = source.Surname,

                };

                entity.Salt = new byte[16];
                new Random().NextBytes(entity.Salt);
                var data = Encoding.ASCII.GetBytes(source.Password).Concat(entity.Salt).ToArray();
                SHA512 shaM = new SHA512Managed();
                entity.Password = shaM.ComputeHash(data);

                return entity;
            }
        }

        private class EntityToModelConverter : ITypeConverter<UserEntity, UserModel>
        {
            public UserModel Convert(UserEntity source, UserModel destination, ResolutionContext context)
            {
                return new UserModel
                {
                    Id = source.Id,
                    Email = source.Email,
                    Name = source.Name,
                    Role = source.RoleType.Role,
                    Surname = source.Surname,

                };
            }
        }
    }
}
