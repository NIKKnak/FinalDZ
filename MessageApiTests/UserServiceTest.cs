using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebApiLibrary.DataStore.Entities;
using WebApiLibrary.Util;

namespace UserServiceTests
{
    [TestFixture]
    public class UserServiceTest
    {
        [Test]
        public void CheckPasswordTest()
        {
            string pass = "123";
            byte[] pw = Encoding.UTF8.GetBytes(pass);
            byte[] salt = Encoding.UTF8.GetBytes("asd");
            UserEntity userEntity = new UserEntity()
            {
                Id = new Guid(),
                Email = "test@mail.ru",
                Password = pw,
                Salt = salt,
                Name = "Test",
                Surname = "Test"
            };

            var expected = pw.Concat(salt).ToArray();
            SHA512 shaM = new SHA512Managed();
            var expPass = shaM.ComputeHash(expected);

            var data = userEntity.Password.Concat(salt).ToArray();
            var password = shaM.ComputeHash(data);
            CollectionAssert.AreEqual(expPass, password);
        }

        [Test]
        public void AuthentificateUserNullTest()
        {
            UserEntity user = null;

            Assert.IsNull(user);
        }

        [Test]
        public void AuthentificateCorrectPasswordTest()
        {
            string pass = "123";
            byte[] pw = Encoding.UTF8.GetBytes(pass);
            byte[] salt = Encoding.UTF8.GetBytes("asd");
            UserEntity userEntity = new UserEntity()
            {
                Id = new Guid(),
                Email = "test@mail.ru",
                Password = pw,
                Salt = salt,
                Name = "Test",
                Surname = "Test"
            };

            var expected = pw.Concat(salt).ToArray();
            SHA512 shaM = new SHA512Managed();
            var expPass = shaM.ComputeHash(expected);

            var data = userEntity.Password.Concat(salt).ToArray();
            var password = shaM.ComputeHash(data);
            CollectionAssert.AreEqual(expPass, password);
        }

        [Test]
        public void AuthentificateIncorrectPasswordTest()
        {
            string pass = "123";
            byte[] pw = Encoding.UTF8.GetBytes(pass);
            byte[] salt = Encoding.UTF8.GetBytes("asd");
            UserEntity userEntity = new UserEntity()
            {
                Id = new Guid(),
                Email = "test@mail.ru",
                Password = pw,
                Salt = salt,
                Name = "Test",
                Surname = "Test"
            };

            SHA512 shaM = new SHA512Managed();
            var data = userEntity.Password.Concat(salt).ToArray();
            var password = shaM.ComputeHash(data);
            CollectionAssert.AreNotEqual(Encoding.UTF8.GetBytes("232"), password);
        }

        [Test]
        public void CheckPasswordWrong()
        {
            Assert.IsTrue(!Helper.CheckPassword("12354"));
        }

        [Test]
        public void CheckPasswordSuccess()
        {
            Assert.IsTrue(Helper.CheckPassword("Q2w3e4r5"));
        }

        [Test]
        public void CheckEmailWrong()
        {
            Assert.IsTrue(!Helper.CheckEmail("shlyapa.ru"));
        }

        [Test]
        public void CheckEmailSuccess()
        {
            Assert.IsTrue(Helper.CheckEmail("shlyapa@mail.ru"));
        }

    }
}