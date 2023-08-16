using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PostMessage
{
    public static class WC
    {
        public const string FileName = "messages.json";
        public const string FilePath = "\\Files\\";
        public const string Success = "Повідомлення надіслано";
        public const string Error = "Помилка, повідомлення не надіслано. Максимально користувач може відправити 10 повідомлень.";
    }
}
