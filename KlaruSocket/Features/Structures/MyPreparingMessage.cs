namespace KlaruSocket.Features.Structures
{
    public class MyPreparingMessage : IMessage
    {
        public string tag { get; set; }
        public string connectionKey { get; set; }
        public string content { get; set; }
    }
}