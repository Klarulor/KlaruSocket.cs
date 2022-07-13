using KlaruSocket.Features.Structures;

namespace KlaruSocket.Features
{
    public class KlaruResponse
    {
        public readonly string Content;
        private readonly MyResponseMessage _resMessage;
        private readonly KlaruSocketClient _client;
        internal KlaruResponse(KlaruSocketClient client, string content, MyResponseMessage resMessage)
        {
            Content = content;
            _resMessage = resMessage;
            _client = client;
        }
    }
}