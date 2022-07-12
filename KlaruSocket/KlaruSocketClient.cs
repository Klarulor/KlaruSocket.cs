using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KlaruSocket.Features;
using KlaruSocket.Features.Structures;
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
        internal readonly Dictionary<string, Action<KlaruRequest>> IncomingRequests = new Dictionary<string, Action<KlaruRequest>>(); // server --> client
        internal readonly Dictionary<string, Action<KlaruRequest>> OutcomingRequests = new Dictionary<string, Action<KlaruRequest>>(); // client --> server
        public KlaruSocketClient(string tag)
        {
            this._tag = tag;
        }

        public void Connect(string host, int port) => Connect(IPAddress.Parse(host == "localhost" ? "127.0.0.1" : host), port);
        public void Connect(IPAddress host, int port) => Connect(new IPEndPoint(host, port));
        public void Connect(IPEndPoint server, bool autoReconnection = false, string connectionKey = "")
        {
            Host = server;
            _connectionKey = connectionKey;
            socket = new WebSocket($"ws://{Host.Address}:{Host.Port}");
            socket.OnMessage += OnMessage;
            socket.OnClose += Socket_OnClose;
            socket.OnOpen += Socket_OnOpen;
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            Connected = true;
            Task.Delay(250).ContinueWith(Auth);
        }

        private void Auth(object _)
        {
            MyPreparingMessage message = new MyPreparingMessage();
            message.tag = _tag;
            message.connectionKey = _connectionKey;
            SendPacket(message);
        }

        internal void SendPacket(IMessage message)
        {
            byte type = 255;
            if (message.GetType() == typeof(MyPreparingMessage))
                type = 0;
            else if (message.GetType() == typeof(MyRequestMessage))
                type = 3;
            else if (message.GetType() == typeof(MyResponseMessage))
                type = 4;
            else if (message.GetType() == typeof(MyInfoMessage))
                type = 2;
            MyMessage packet = new MyMessage(Newtonsoft.Json.JsonConvert.SerializeObject(message), type);
            socket.Send(Newtonsoft.Json.JsonConvert.SerializeObject(packet));
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