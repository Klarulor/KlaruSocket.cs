using KlaruSocket.Features.Structures;

namespace KlaruSocket.Features
{
    public class KlaruRequest
    {
        public readonly string Content;
        private readonly MyRequestMessage _reqMessage;
        private readonly KlaruSocketClient _client;
        internal KlaruRequest(KlaruSocketClient client, string content, MyRequestMessage reqMessage)
        {
            Content = content;
            _reqMessage = reqMessage;
            _client = client;
        }

        public void Reply(string content)
        {
            MyResponseMessage res = new MyResponseMessage();
            res.content = content;
            res.sessionId = _reqMessage.sessionId;
            res.responseCode = "OK";
            
            if (_client.IncomingRequests.ContainsKey(res.sessionId))
            {
                _client.IncomingRequests[res.sessionId].Dispose();
                _client.IncomingRequests.Remove(res.sessionId);
                _client.SendPacket(res);
            }
        }
    }
}