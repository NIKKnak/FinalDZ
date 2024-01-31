
namespace WebApiLibrary.DataStore.Models
{
    public class UserResponse
    {
        public bool IsSuccess { get; set; }
        public Guid? UserId { get; set; }
        public List<ErrorModel> Errors = new List<ErrorModel>();
        public List<UserModel> Users = new List<UserModel>();
        public string Token { get; set; }

        public static UserResponse AddAdminError()
        {
            return new UserResponse
            {
                IsSuccess = false,
                Errors = new List<ErrorModel> {
                    new ErrorModel
                    {
                        Message = "Администратор может быть только один",
                        StatusCode = 409
                    } }
            };
        }

        public static UserResponse UserNotFound()
        {
            return new UserResponse
            {
                IsSuccess = false,
                Errors = new List<ErrorModel> {
                    new ErrorModel
                    {
                        Message = "Пользователь не найден",
                        StatusCode = 404
                    }
                }
            };
        }

        public static UserResponse UserExist()
        {
            return new UserResponse
            {
                IsSuccess = false,
                Errors = new List<ErrorModel> {
                    new ErrorModel
                    {
                        Message = "Пользователь уже существует",
                        StatusCode = 409
                    }
                }
            };
        }

        public static UserResponse PasswordWrong()
        {
            return new UserResponse
            {
                IsSuccess = false,
                Errors = new List<ErrorModel> {
                    new ErrorModel
                    {
                        Message = "пароль неверный",
                    }
                }
            };
        }

        public static UserResponse AccessDenied()
        {
            return new UserResponse
            {
                IsSuccess = false,
                Errors = new List<ErrorModel> {
                    new ErrorModel
                    {
                        Message = "Доступ запрещен",
                    }
                }
            };
        }

        public static UserResponse OK()
        {
            return new UserResponse
            {
                IsSuccess = true,
            };
        }
    }
}
