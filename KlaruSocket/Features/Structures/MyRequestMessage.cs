namespace KlaruSocket.Features.Structures
{
    internal class MyRequestMessage : IMessage
    {
        public string keyword { get; set; }
        public string sessionId { get; set; }
        public int ttl { get; set; }
        public string cntent { get; set; }
    }
}