using AutoMapper;
using WebApiLibrary.DataStore.Entities;
using WebApiLibrary.DataStore.Models;

namespace MessageApi.Mapper
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<MessageEntity, MessageModel>().ConvertUsing(new EntityToModelConverter());

        }
        private class EntityToModelConverter : ITypeConverter<MessageEntity, MessageModel>
        {
            public MessageModel Convert(MessageEntity source, MessageModel destination, ResolutionContext context)
            {
                return new MessageModel
                {
                    Id = source.Id,
                    CreatedAt = source.CreatedAt,
                    IsRead = source.IsRead,
                    RecipientEmail = source.Recipient.Email,
                    SenderEmail = source.Sender.Email,
                    Text = source.Text

                };
            }
        }
    }
}
