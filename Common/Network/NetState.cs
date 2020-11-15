using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSServerCommon.Network.Encryption;
using DSServerCommon.Network.Packets;

namespace DSServerCommon.Network
{
    public class NetState
    {
        public event EventHandler<Exception> OnException;
        public event EventHandler<Guid> OnDisconnected;
        public event Action<NetState> OnStarted;

        public Guid Id { get; private set; }
        public RSAEncryption RSA { get; set; }
        public RijndaelEncryption Rijndael { get; set; }
        public bool UseEncryption
        {
            get => _useEncryption;
            set => _useEncryption = value;
        }
        public bool Connected => _client?.Connected ?? false;

        internal int WriterQueueCount => _writerQueue?.Count ?? -1;

        Socket _client;

        Task _readerTask;
        CancellationTokenSource _readerSource;
        EventWaitHandle _readerHandle;
        List<byte> _readerBuffer;

        Task _writerTask;
        CancellationTokenSource _writerSource;
        ConcurrentQueue<Packet> _writerQueue;

        bool _keepAlive;
        const int _READ_BUFFER_SIZE = 1024;

        volatile bool _useEncryption;

        readonly object _syncRoot = new object();

        public NetState(Socket client)
        {
            _client = client;
            Id = Guid.NewGuid();
        }

        public NetState()
        {
            Id = Guid.NewGuid();
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            lock(_syncRoot)
            {
                if (_keepAlive)
                    return;

                _keepAlive = true;
                _readerHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
                _readerBuffer = new List<byte>();
                _writerQueue = new ConcurrentQueue<Packet>();

                _readerSource = new CancellationTokenSource();
                _writerSource = new CancellationTokenSource();

                _readerTask = new Task(Read, _readerSource.Token);
                _readerTask.Start();

                _writerTask = new Task(Write, _writerSource.Token);
                _writerTask.Start();

                OnStarted?.Invoke(this);
            }
        }

        public void Stop()
        {
            lock(_syncRoot)
            {
                if (!_keepAlive)
                    return;

                _keepAlive = false;

                _ = TryDisconnect();

                _readerSource.Cancel();
                _writerSource.Cancel();

                _readerTask.Wait();
                _writerTask.Wait();
            }
        }

        public bool TryDisconnect()
        {
            try
            {
                _client.Disconnect(false);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(this, ex);
                OnDisconnected?.Invoke(this, Id);
                return false;
            }

            OnDisconnected?.Invoke(this, Id);
            return true;
        }

        public void Write(Packet p)
        {
            _writerQueue.Enqueue(p);
        }

        public void Connect(IPEndPoint ipep, bool useEncryption = true)
        {
            _client.Connect(ipep);

            if (useEncryption)
            {
                CEncryptPacket encrypt = new CEncryptPacket();
                Write(encrypt);
            }
        }

        void Read()
        {
            try
            {
                while (_keepAlive)
                {
                    StateObject sobj = new StateObject(_READ_BUFFER_SIZE);
                    _client.BeginReceive(sobj.Buffer, 0, _READ_BUFFER_SIZE, SocketFlags.None, new AsyncCallback(EndRead), sobj);

                    _readerHandle.WaitOne();
                }
            }
            catch (Exception ex)
            {
                OnException?.Invoke(this, ex);
                _ = TryDisconnect();
            }
        }

        void Write()
        {
            try
            {
                while (_keepAlive)
                {
                    if (_writerQueue.TryDequeue(out Packet p))
                    {
                        byte[] toSend;
                        if (UseEncryption)
                            toSend = p.ToArray(Rijndael);
                        else
                            toSend = p.ToArray();

                        _client.Send(toSend);
                    }

                    Task.Delay(25).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                OnException?.Invoke(this, ex);
                _ = TryDisconnect();
            }
        }

        void EndRead(IAsyncResult ar)
        {
            lock (_syncRoot)
            {
                try
                {
                    int length = _client.EndReceive(ar);

                    if (length <= 0)
                        return;

                    StateObject sobj = (StateObject)ar.AsyncState;
                    sobj.Resize(length);

                    byte[] data = sobj.Buffer;

                    for (int i = 0; i < data.Length; i++)
                    {
                        _readerBuffer.Add(data[i]);

                        if (IsPacketEnding())
                        {
                            byte[] array = _readerBuffer.GetRange(0, _readerBuffer.Count - 5).ToArray();
                            _readerBuffer.Clear();

                            Task.Run(() => HandlePacket(ref array)).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    _ = TryDisconnect();
                }

                _readerHandle.Set();
            }
        }

        void HandlePacket(ref byte[] packet)
        {
            try
            {
                int id = BitConverter.ToInt32(packet, 0);

                PacketId pkId = (PacketId)id;

                if (!PacketHandler.Packets.ContainsKey(id))
                    return;
                else if (!UseEncryption &&
                         (id != (int)PacketId.SEncrypt &&
                          id != (int)PacketId.CEncrypt))
                    return;

                Type type = PacketHandler.Packets[id];
                Packet p = Activator.CreateInstance(type, new object[] { packet, this }) as Packet;

                p.InvokePacket(this);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(this, ex);
            }
        }

        bool IsPacketEnding(ref byte[] data, int pos)
        {
            if (data == null ||
                data.Length <= pos + 5 ||
                data[pos + 4] != 0xFF ||
                data[pos + 3] != 0xFA ||
                data[pos + 2] != 0xFF ||
                data[pos + 1] != 0xFA ||
                data[pos] != 0xFF)
                return false;

            return true;
        }

        bool IsPacketEnding()
        {
            if (_readerBuffer == null ||
                _readerBuffer.Count <= 5 ||
                _readerBuffer[_readerBuffer.Count - 1] != 0xFF ||
                _readerBuffer[_readerBuffer.Count - 2] != 0xFA ||
                _readerBuffer[_readerBuffer.Count - 3] != 0xFF ||
                _readerBuffer[_readerBuffer.Count - 4] != 0xFA ||
                _readerBuffer[_readerBuffer.Count - 5] != 0xFF)
                return false;

            return true;
        }

        struct StateObject
        {
            public byte[] Buffer => _buffer;
            byte[] _buffer;

            public StateObject(int length) : this()
            {
                _buffer = new byte[length];
            }

            public void Resize(int newLength)
            {
                if (_buffer.Length == newLength)
                    return;

                Array.Resize(ref _buffer, newLength);
            }
        }
    }
}
