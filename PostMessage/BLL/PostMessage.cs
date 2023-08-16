using PostMessage.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PostMessage.BLL
{
    public class PostMessage : IPostMessage
    {
        private readonly IWebHostEnvironment _appEnv;

        public PostMessage(IWebHostEnvironment appEnvironment)
        {
            _appEnv = appEnvironment;
        }

        /// <summary>
        /// Вибір 10 повідомлень користувача
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Message>> Get10UserMessageAsync(Guid userId)
        {
            var filePath = _appEnv.WebRootPath + WC.FilePath + WC.FileName;
            IEnumerable<Message> tenUserMessages = new List<Message>();

            if (!File.Exists(filePath))
            {
                CreateNewJsonFile(filePath);
                return tenUserMessages;
            }

            var messages = await ReadJsonFileAsync(filePath);
            tenUserMessages = messages.Where(x => x.UserId == userId).Take(10);

            return tenUserMessages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Message>> Get20LatestUsersMessagesAsync()
        {
            var filePath = _appEnv.WebRootPath + WC.FilePath + WC.FileName;
            var messages = await ReadJsonFileAsync(filePath);

            var latest20Messages = messages.Take(20);
            return latest20Messages;
        }

        /// <summary>
        /// Збереження повідомлення в json
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> PostUserMessageAsync(string message, Guid userId)
        {
            var filePath = _appEnv.WebRootPath + WC.FilePath + WC.FileName;
            var messages = new List<Message>();

            if (!File.Exists(filePath))
            {
                CreateNewJsonFile(filePath);
                var newMessage = new Message
                {
                    Text = message,
                    UserId = userId,
                    TimeOfCreation = DateTime.Now
                };
                messages.Add(newMessage);
                await WriteJsonFileAsync(filePath, messages);
                
                return true;
            }
            else
            {
                messages = await ReadJsonFileAsync(filePath);
                var messageCount = messages.Count(x => x.UserId == userId);

                if (messageCount < 10)
                {
                    var newMessage = new Message
                    {
                        Text = message,
                        UserId = userId,
                        TimeOfCreation = DateTime.Now
                    };
                    
                    messages.Add(newMessage);
                    await WriteJsonFileAsync(filePath, messages);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Створення файлу JSON, якщо немає
        /// </summary>
        private void CreateNewJsonFile(string filePath)
        {
            if (!Directory.Exists(_appEnv.WebRootPath + WC.FilePath))
                Directory.CreateDirectory(_appEnv.WebRootPath + WC.FilePath);

            if (!File.Exists(filePath))
            {
                // Create the file
                using var file = File.CreateText(filePath);
            }
        }

        /// <summary>
        /// Зчитування з файлу Json
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<List<Message>> ReadJsonFileAsync(string filePath)
        {
            var messages = new List<Message>();

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                if (fs.Length > 0)
                {
                    var json = await JsonSerializer.DeserializeAsync<List<Message>>(fs);
                    messages = json ?? new List<Message>();
                }
            }

            return messages;
        }

        /// <summary>
        /// Запис у файл Json
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        private async Task WriteJsonFileAsync(string filePath, List<Message> messages)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync(fs, messages, new JsonSerializerOptions { WriteIndented = true });
            }
        }
    }
}
