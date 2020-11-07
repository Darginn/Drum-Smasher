using DSServerCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrumSmasher.Network
{
    public class TCPClient : IDisposable
    {
        public IPEndPoint IPEndPoint { get; private set; }
        public bool IsDisposed { get; private set; }

        private Socket _socket;

        private Task _readerTask;
        private EventWaitHandle _readerHandle;
        private bool _keepReceiving;

        public TCPClient(IPAddress ip, int port)
        {
            IPEndPoint = new IPEndPoint(ip, port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Connects Asynchronous
        /// </summary>
        public async Task ConnectAsync()
        {
            await Task.Run(() => Connect());
        }

        /// <summary>
        /// Connects
        /// </summary>
        public void Connect()
        {
            Logger.Log("Connecting");

            _socket.Connect(IPEndPoint);
         
            Logger.Log("Finished connecting");
        }

        /// <summary>
        /// Disconnects Asynchronous
        /// </summary>
        public async Task DisconnectAsync()
        {
            await Task.Run(() => Disconnect());
        }

        /// <summary>
        /// Disconnects
        /// </summary>
        public virtual void Disconnect()
        {
            _socket.Disconnect(true);
        }

        /// <summary>
        /// Sends a byte array
        /// </summary>
        /// <param name="data"></param>
        public virtual void Send(byte[] data)
        {
            if (data == null || data.Length == 0)
                return;

            _socket.Send(data);
        }

        /// <summary>
        /// Sends a byte array asynchronous
        /// </summary>
        public async Task SendAsync(byte[] data)
        {
            await Task.Run(() => Send(data));
        }

        /// <summary>
        /// Starts receiving data
        /// </summary>
        public void StartReceiving()
        {
            _readerHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _keepReceiving = true;

            _readerTask = Task.Run(() =>
            {
                try
                {
                    while (_keepReceiving)
                    {
                        StateObject so = new StateObject(8192);
                        _socket.BeginReceive(so.Bytes, 0, 8192, SocketFlags.None, new AsyncCallback(EndReceive), so);
                        _readerHandle.WaitOne();
                    }
                }
                catch (Exception ex)
                {
                    Disconnect();
                    throw new Exception("Forcefully disconnected, see inner exception");
                }
            });
        }

        /// <summary>
        /// Signals to stop receiving data
        /// </summary>
        public void StopReceiving()
        {
            try
            {
                _keepReceiving = false;
                _readerHandle.Set();
                _readerTask.Dispose();
                _readerHandle.Dispose();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Disposes the client
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Called when the client receives data
        /// </summary>
        protected virtual void OnDataReceived(byte[] data)
        {

        }

        /// <summary>
        /// Ends a receive request
        /// </summary>
        private void EndReceive(IAsyncResult ar)
        {
            int length = _socket.EndReceive(ar);
            Logger.Log($"New data received, length: {length}");

            StateObject so = (StateObject)ar.AsyncState;

            so.Resize(length);

            Task.Run(() => OnDataReceived(so.Bytes));
            _readerHandle.Set();
        }

        /// <summary>
        /// Disposes the client
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                _socket.Dispose();
            }

            IsDisposed = true;
        }
    }
}
