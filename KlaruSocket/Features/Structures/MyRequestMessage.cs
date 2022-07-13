namespace KlaruSocket.Features.Structures
{
    public class MyRequestMessage : IMessage
    {
        public string keyword { get; set; }
        public string sessionId { get; set; }
        public int ttl { get; set; }
        public string content { get; set; }
    }
}