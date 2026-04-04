using System;
using System.Collections.Generic;
using System.Text;

namespace DonutMessager.Models
{
    internal class MessageModel
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public bool IsMine => SenderId == CurrentUserId;

        // UI
        public string SenderAvatar { get; set; }

        // нужно установить при создании VM
        public static int CurrentUserId { get; set; }

    }
}
