using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApiLibrary;
using WebApiLibrary.Abstraction;
using WebApiLibrary.DataStore.Entities;
using WebApiLibrary.DataStore.Models;
using WebApiLibrary.DataStore.Response;

namespace MessageApi.Services
{
    public class MessageService : IMessageService
    {
        private readonly Func<AppDbContext> _context;
        private readonly IMapper _mapper;

        public MessageService(Func<AppDbContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public MessageResponse GetNewMessages(string recipientEmail)
        {
            var response = new MessageResponse();
            using (var context = _context())
            {
                var messages = context.Messages
                    .Include(x => x.Recipient)
                    .Include(x => x.Sender)
                    .Where(x => x.Recipient.Email == recipientEmail && !x.IsRead).ToList();

                foreach (var message in messages)
                {
                    message.IsRead = true;
                }

                context.UpdateRange(messages);
                context.SaveChanges();

                response.Messages.AddRange(messages.Select(_mapper.Map<MessageModel>));
                response.IsSuccess = true;
            }
            return response;
        }

        public MessageResponse SendMessage(MessageModel model)
        {
            var response = new MessageResponse();
            using (var context = _context())
            {
                var sender = context.Users.AsNoTracking().FirstOrDefault(x => x.Email == model.SenderEmail);
                var recipient = context.Users.AsNoTracking().FirstOrDefault(x => x.Email == model.RecipientEmail);
                if (sender == null || recipient == null)
                {
                    response.IsSuccess = false;
                    response.Errors.Add(new ErrorModel { Message = "Один из учатников не найден!" });
                    return response;
                }

                var message = new MessageEntity
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    RecipientId = recipient.Id,
                    SenderId = sender.Id,
                    Text = model.Text
                };

                context.Messages.Add(message);
                context.SaveChanges();

                var entity = context.Messages
                    .Include(x => x.Recipient)
                    .Include(x => x.Sender)
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == message.Id);

                response.Messages.Add(_mapper.Map<MessageModel>(entity));
                response.IsSuccess = true;
            }
            return response;
        }
    }
}
