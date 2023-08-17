using PostMessage.Models;

namespace PostMessage.BLL
{
    public interface IPostMessage
    {
        public Task<bool> PostUserMessageAsync(string message, Guid userId);
        public Task<IEnumerable<Message>> GetMessages(FetchMessageParams fetchParams);
       
    }
}
