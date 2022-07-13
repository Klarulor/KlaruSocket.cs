using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using KlaruSocket.Features;
using KlaruSocket.Features.Structures;
using WebSocketSharp;

namespace KlaruSocket
{
    public class KlaruSocketClient
    {
        private WebSocket _socket;
        public IPEndPoint Host { get; private set; }
        public bool AutoReconnection { get; set; }
        public bool Connected { get; private set; }
        private readonly string _tag;
        private string _connectionKey = String.Empty;
        internal readonly Dictionary<string, Task> IncomingRequests = new Dictionary<string, Task>(); // server --> client
        internal readonly Dictionary<string, Action<KlaruResponse>> OutcomingRequests = new Dictionary<string, Action<KlaruResponse>>(); // client --> server
        internal readonly Dictionary<string, List<Action<KlaruRequest>>> Keys = new Dictionary<string, List<Action<KlaruRequest>>>();
        public KlaruSocketClient(string tag)
        {
            this._tag = tag;
        }

        public void Subscribe(string keyword, Action<KlaruRequest> callback)
        {
            if(!Keys.ContainsKey(keyword))
                Keys.Add(keyword, new List<Action<KlaruRequest>>());
            Keys[keyword].Add(callback);
        }

        public void Unsubscribe(string keyword, Action<KlaruRequest> callback)
        {
            if (Keys.ContainsKey(keyword) && Keys[keyword].Any(x => x == callback))
                Keys[keyword].Remove(callback);
        }
        public void Connect(string host, int port) => Connect(IPAddress.Parse(host == "localhost" ? "127.0.0.1" : host), port);
        public void Connect(IPAddress host, int port) => Connect(new IPEndPoint(host, port));
        public void Connect(IPEndPoint server, bool autoReconnection = false, string connectionKey = "")
        {
            Host = server;
            AutoReconnection = autoReconnection;
            _connectionKey = connectionKey;
            _socket = new WebSocket($"ws://{Host.Address}:{Host.Port}");
            _socket.OnMessage += OnMessage;
            _socket.OnClose += Socket_OnClose;
            _socket.OnOpen += Socket_OnOpen;
            _socket.Connect();
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            Connected = true;
            Task.Delay(250).ContinueWith(Auth);
        }
        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            Connected = false;
            if (AutoReconnection)
            {
                Task.Delay(500).ContinueWith(_ =>
                {
                    _socket.Connect();
                });
            }
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
            _socket.Send(Newtonsoft.Json.JsonConvert.SerializeObject(packet));
        }

        private readonly Random _random = new Random();
        public Task<KlaruResponse> Get(string keyword, string content)
        {
            MyRequestMessage reqMessage = new MyRequestMessage();
            reqMessage.content = content;
            reqMessage.keyword = keyword;
            reqMessage.ttl = 5000;
            reqMessage.sessionId = _random.Next(1, 1000000).ToString();
            TaskCompletionSource<KlaruResponse> task = new TaskCompletionSource<KlaruResponse>();;
            OutcomingRequests.Add(reqMessage.sessionId, res => task.SetResult(res));
            SendPacket(reqMessage);
            return task.Task;
        }
        private void OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                MyMessage packet = Newtonsoft.Json.JsonConvert.DeserializeObject<MyMessage>(e.Data);
                switch (packet.type)
                {
                    case 0:
                        throw new Exception("Cannot init, it is client, not server!");
                        break;
                    case 1:

                        break;
                    case 2:

                        break;
                    case 3:
                    {
                        MyRequestMessage reqMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<MyRequestMessage>(packet.content);
                        if (Keys.ContainsKey(reqMessage.keyword))
                        {
                            KlaruRequest request = new KlaruRequest(this, reqMessage.content, reqMessage);
                            foreach(var action in Keys[reqMessage.keyword])
                                action.Invoke(request);
                            var task = Task.Delay(1000).ContinueWith(__ =>
                            {
                                IncomingRequests.Remove(reqMessage.sessionId);
                                MyResponseMessage resMessage = new MyResponseMessage();
                                resMessage.responseCode = "TIMEOUT";
                                resMessage.sessionId = reqMessage.sessionId;
                                resMessage.content = "__null";
                                SendPacket(resMessage);
                            });
                            IncomingRequests.Add(reqMessage.sessionId, task);
                            task.Start();
                        }
                        else
                        {
                            MyResponseMessage response = new MyResponseMessage();
                            response.content = "__null";
                            response.responseCode = "BAD_KEY";
                            response.sessionId = reqMessage.sessionId;
                            SendPacket(response);
                        }
                    }
                        break;
                    case 4:
                    {
                        MyResponseMessage resMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<MyResponseMessage>(packet.content);
                        if (OutcomingRequests.ContainsKey(resMessage.content))
                        {
                            KlaruResponse response = new KlaruResponse(this, resMessage.content, resMessage);
                            OutcomingRequests[resMessage.sessionId].Invoke(response);
                        }
                    }
                        break;
                }
            }catch(Exception error){throw new PolicyException($"Cannot handle message");
            }
        }
    }
}