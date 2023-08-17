using PostMessage.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace PostMessage.BLL
{

    public class FetchMessageParams
    {
        public Guid? userId { get; set; }
        public int limit { get; set; }

        public FetchMessageParams(Guid userId, int limit)
        {
            this.userId = userId;
            this.limit = limit;
        }

        public FetchMessageParams(int limit)
        {
            this.limit = limit;
        }
    }

    public class PostMessage : IPostMessage
    {
        private readonly IWebHostEnvironment _appEnv;

        public PostMessage(IWebHostEnvironment appEnvironment)
        {
            _appEnv = appEnvironment;
        }

        public async Task<IEnumerable<Message>> GetMessages(FetchMessageParams fetchParams)
        {
            var filePath = _appEnv.WebRootPath + WC.FilePath + WC.FileName;
            IEnumerable<Message> tenUserMessages = new List<Message>();

            if (!File.Exists(filePath))
            {
                CreateNewJsonFile(filePath);
                return tenUserMessages;
            }

            var messages = await ReadJsonFileAsync(filePath);
            if (fetchParams.userId != null)
            {
                return messages.Where(x => x.UserId == fetchParams.userId).Take(fetchParams.limit);
            }

            return messages.Take(fetchParams.limit);
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
