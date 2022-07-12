namespace KlaruSocket.Features.Structures
{
    internal class MyMessage
    {
        public string content { get; set; } // IMessage
        public byte type { get; set; }

        public MyMessage(string content, byte type)
        {
            this.content = content;
            this.type = type;
        }
    }
}