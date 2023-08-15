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
        /// Збереження повідомлення в json
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PostUserMessageAsync(string message, Guid userId)
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
                }

            }
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
                var json = await JsonSerializer.DeserializeAsync<List<Message>>(fs);

                messages = json ?? new List<Message>();
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
