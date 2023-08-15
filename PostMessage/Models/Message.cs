using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostMessage.Models
{
    public class Message
    {
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTime TimeOfCreation { get; set; }
    }
}
