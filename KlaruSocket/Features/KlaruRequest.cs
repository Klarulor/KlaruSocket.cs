using KlaruSocket.Features.Structures;

namespace KlaruSocket.Features
{
    public class KlaruRequest
    {
        public readonly string Content;
        private readonly MyRequestMessage _reqMessage;
        internal KlaruRequest(string content, MyRequestMessage reqMessage)
        {
            Content = content;
            _reqMessage = reqMessage;
        }

        public void Reply(string content)
        {
            MyResponseMessage res = new MyResponseMessage();
            res.cntent = content;
            res.sessionId = _reqMessage.sessionId;
            res.responseCode = "OK";
            
        }
    }
}