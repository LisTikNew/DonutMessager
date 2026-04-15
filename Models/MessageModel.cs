namespace DonutMessager.Models
{
    public class MessageModel
    {
        public string Text { get; set; }
        public int SenderId { get; set; }
        public string SenderAvatar { get; set; }
        public DateTime Timestamp { get; set; }
        public static int CurrentUserId { get; set; }
        public bool IsMine => SenderId == CurrentUserId;
    }
}