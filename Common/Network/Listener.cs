using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSServerCommon.Network.Encryption;

namespace DSServerCommon.Network
{
    public class Listener
    {
        public event EventHandler<Exception> OnException;

        public string Host { get; private set; }
        public int Port { get; private set; }
        public ConcurrentDictionary<Guid, NetState> Clients { get; private set; }
        public RijndaelEncryption _rijn;

        bool _keepAlive;
        Socket _socket;
        EventWaitHandle _handle;
        Task _listenerTask;
        CancellationTokenSource _listenerSource;
        Type _netstateType;

        public Listener(string host, int port)
        {
            if (port == 0)
                throw new ArgumentOutOfRangeException(nameof(port));
            else if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host));

            Clients = new ConcurrentDictionary<Guid, NetState>();
            Host = host;
            Port = port;
        }

        public void Start<T>() where T : NetState
        {
            _keepAlive = true;
            _netstateType = typeof(T);
            _handle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(Host), Port));
            _listenerSource = new CancellationTokenSource();
            _listenerTask = new Task(Listen, _listenerSource.Token);

            _socket.Listen(5);
            _listenerTask.Start();
        }

        void Listen()
        {
            while(_keepAlive)
            {
                _socket.BeginAccept(new AsyncCallback(EndAccept), null);
                _handle.WaitOne();
            }
        }

        void EndAccept(IAsyncResult ar)
        {
            Socket s = _socket.EndAccept(ar);

            if (s == null)
            {
                _handle.Set();
                return;
            }

            NetState netState = Activator.CreateInstance(_netstateType, s) as NetState;

            if (netState == null)
            {
                OnException?.Invoke(this, new NullReferenceException("Unable to create NetState instance through type: " + _netstateType.FullName));
                return;
            }

            netState.OnDisconnected += OnDisconnected;
            netState.OnException += Exception;
            netState.Start();
            _ = Clients.TryAdd(netState.Id, netState);

            _handle.Set();
        }

        public void OnDisconnected(object sender, Guid id)
        {
            _ = Clients.TryRemove(id, out NetState _);
        }

        void Exception(object sender, Exception ex)
        {
            OnException?.Invoke(sender, ex);
        }
    }
}
