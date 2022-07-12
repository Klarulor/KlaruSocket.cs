namespace KlaruSocket.Features.Structures
{
    internal class MyResponseMessage : IMessage
    {
        public string sessionId { get; set; }
        public string responseCode { get; set; }
        public string cntent { get; set; }
    }
}