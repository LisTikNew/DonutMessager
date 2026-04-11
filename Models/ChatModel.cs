using System;
using System.Collections.Generic;
using System.Text;

namespace DonutMessager.Models
{
    public class ChatModel
    {
        public int Id { get; set; }

        // ID собеседника
        public int UserId { get; set; }

        public string Title { get; set; }
        public string AvatarUrl { get; set; }
    }
}