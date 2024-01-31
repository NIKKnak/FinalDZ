using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiLibrary.Abstraction;
using WebApiLibrary.DataStore.Models;

namespace MessageApi.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;

        public MessageController(IMapper mapper, IMessageService messageService)
        {
            _mapper = mapper;
            _messageService = messageService;
        }

        [Authorize(Roles = "Administrator, User")]
        [HttpGet("get")]
        public ActionResult GetNewMessage()
        {
            var senderEmail = GetUserEmailFromToken().GetAwaiter().GetResult();

            var response = _messageService.GetNewMessages(senderEmail);
            if (!response.IsSuccess)
                return BadRequest(response.Errors.FirstOrDefault().Message);

            return Ok(response.Messages);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost("send")]
        public ActionResult SendMessage(string recipientEmail, string text)
        {
            var senderEmail = GetUserEmailFromToken().GetAwaiter().GetResult();

            var message = new MessageModel
            {
                RecipientEmail = recipientEmail,
                SenderEmail = senderEmail,
                Text = text
            };

            var response = _messageService.SendMessage(message);
            if (!response.IsSuccess)
                return BadRequest(response.Errors.FirstOrDefault().Message);

            return Ok(response.Messages);
        }

        private async Task<string> GetUserEmailFromToken()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var claim = jwtToken!.Claims.Single(x => x.Type.Contains("emailaddress"));

            return claim.Value;

        }
    }
}
