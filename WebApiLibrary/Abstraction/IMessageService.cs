using WebApiLibrary.DataStore.Models;
using WebApiLibrary.DataStore.Response;

namespace WebApiLibrary.Abstraction
{
    public interface IMessageService
    {
        MessageResponse GetNewMessages(string recipientEmail);
        MessageResponse SendMessage(MessageModel model);
    }
}
