namespace PostMessage.BLL
{
    public interface IPostMessage
    {
        public Task PostUserMessageAsync(string message, Guid userId);
    }
}
