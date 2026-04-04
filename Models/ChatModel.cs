using System;
using System.Collections.Generic;
using System.Text;

namespace DonutMessager.Models
{
    internal class ChatModel
    {
        public int Id { get; set; }
        public string Title { get; set; }   // имя собеседника или название группы
        public string AvatarUrl { get; set; }

    }
}
