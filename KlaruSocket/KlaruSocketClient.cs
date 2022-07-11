using System;
using System.Net;
using System.Threading.Tasks;
using WebSocketSharp;

namespace KlaruSocket
{
    public class KlaruSocketClient
    {
        private WebSocket socket;
        public IPEndPoint Host { get; private set; }
        public bool AutoReconnection { get; set; }
        public bool Connected { get; private set; }
        private readonly string _tag;
        private string _connectionKey = String.Empty;
        public KlaruSocketClient(string tag)
        {
            this._tag = tag;
        }

        public void Connect(string host, int port) => Connect(IPAddress.Parse(host == "localhost" ? "127.0.0.1" : host), port);
        public void Connect(IPAddress host, int port) => Connect(new IPEndPoint(host, port));
        public void Connect(IPEndPoint server, bool autoReconnection = false, string connectionKey = "")
        {
            Host = server;
            this._connectionKey = connectionKey;
            socket = new WebSocket($"ws://{Host.Address}:{Host.Port}");
            socket.OnMessage += OnMessage;
            socket.OnClose += Socket_OnClose;
            socket.OnOpen += Socket_OnOpen;
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            Connected = true;
            Task.Delay(250).ContinueWith(_ =>
            {
                
            });
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            Connected = false;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}