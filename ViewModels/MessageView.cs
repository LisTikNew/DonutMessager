using System;
using System.Collections.Generic;
using System.Text;

namespace DonutMessager.ViewModels
{
    public class MessageView
    {
        public string Text { get; set; }
        public bool IsMine { get; set; }
        public string SenderAvatar { get; set; }
    }
}
