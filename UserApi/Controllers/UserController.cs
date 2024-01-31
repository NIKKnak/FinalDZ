using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiLibrary;
using WebApiLibrary.Abstraction;
using WebApiLibrary.DataStore.Models;
using WebApiLibrary.rsa;
using WebApiLibrary.Util;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly Account _account;
        private readonly IConfiguration _configuration;


        public UserController(IUserService userService, Account account, IConfiguration configuration)
        {
            _account = account;
            _userService = userService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([Description("Аутентификация пользователя"), FromBody] LoginModel model)
        {
            if (!Helper.CheckEmail(model.Email))
                return BadRequest($"Email:'{model.Email}' - Invalid Format");

            if (_account.GetAccessToken() != null)
                return BadRequest("Вы уже вошли в систему");
            var response = _userService.Authentificate(model);
            if (!response.IsSuccess)
                return NotFound(response.Errors.FirstOrDefault()?.Message);

            _account.Login(response.Users[0]);
            _account.RefreshToken(GenerateToken(_account));

            return Ok(_account.GetAccessToken());
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("add")]
        public ActionResult AddUser(RegistrationModel model)
        {
            if (!Helper.CheckEmail(model.Email))
                return BadRequest($"Email:'{model.Email}' - Invalid Format");
            if (!Helper.CheckPassword(model.Password))
                return BadRequest($"Password:'{model.Password}' - Invalid Format");

            var response = _userService.AddUser(model);
            if (!response.IsSuccess)
                return BadRequest(response.Errors.FirstOrDefault().Message);

            return Ok(response.UserId);
        }

        [AllowAnonymous]
        [HttpPost("add_admin")]
        public ActionResult AddAdmin(RegistrationModel model)
        {
            if (!Helper.CheckEmail(model.Email))
                return BadRequest($"Email:'{model.Email}' - Invalid Format");
            if (!Helper.CheckPassword(model.Password))
                return BadRequest($"Password:'{model.Password}' - Invalid Format");

            var response = _userService.AddAdmin(model);
            if (!response.IsSuccess)
                return BadRequest(response.Errors.FirstOrDefault().Message);

            return Ok(response.UserId);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("many")]
        public ActionResult GetUsers()
        {
            var response = _userService.GetUsers();
            if (!response.IsSuccess)
                return BadRequest(response.Errors.FirstOrDefault().Message);

            return Ok(response.Users);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("one")]
        public ActionResult GetUser(Guid? userId, string? email)
        {
            var response = _userService.GetUser(userId, email);
            if (!response.IsSuccess)
                return BadRequest(response.Errors.FirstOrDefault().Message);

            return Ok(response.Users);
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("delete")]
        public ActionResult DeleteUser(Guid? userId, string? email)
        {
            var response = _userService.DeleteUser(userId, email);
            if (!response.IsSuccess)
                return BadRequest(response.Errors.FirstOrDefault().Message);

            return Ok();
        }

        [HttpPost("logout")]
        public ActionResult LogOut()
        {
            _account.Logout();
            return Ok();
        }

        private string GenerateToken(Account model)
        {
            var key = new RsaSecurityKey(RSAService.GetPrivateKey());
            var credential = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Role, model.Role.ToString()),
                new Claim("UserId", model.Id.ToString())
            };
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credential);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
