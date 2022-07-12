namespace KlaruSocket.Features.Structures
{
    internal class MyPreparingMessage : IMessage
    {
        public string tag { get; set; }
        public string connectionKey { get; set; }
        public string cntent { get; set; }
    }
}