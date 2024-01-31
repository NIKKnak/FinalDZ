using WebApiLibrary.DataStore.Models;

namespace WebApiLibrary.DataStore.Response
{
    public class MessageResponse
    {
        public bool IsSuccess { get; set; }
        public List<MessageModel> Messages { get; set; } = new List<MessageModel>();
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
