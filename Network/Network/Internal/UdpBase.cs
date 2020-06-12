using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Network.Internal
{
    public abstract class UdpBase
    {
        protected UdpClient _client;
        protected IPEndPoint _ipEndPoint;

        private bool _keepListening;

        protected UdpBase(IPAddress ip, int port)
        {
            _ipEndPoint = new IPEndPoint(ip, port);
            _client = new UdpClient(_ipEndPoint);
        }

        protected UdpBase(UdpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Recieves a datagram from this client
        /// </summary>
        /// <returns>Recieved datagram</returns>
        public async Task<UdpReceiveResult> ReceiveAsync()
        {
            return await _client.ReceiveAsync();
        }

        /// <summary>
        /// Sends a datagram to this client
        /// </summary>
        /// <param name="data">Datagram to send</param>
        /// <returns>Bytes sent</returns>
        public async Task<int> SendAsync(byte[] data, UdpClient user)
        {
            return await user.SendAsync(data, data.Length);
        }

        public void StartReceiving()
        {
            _keepListening = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(o => Listen()));
        }

        public void StopReceiving()
        {
            _keepListening = false;
        }

        private void Listen()
        {
            while(_keepListening)
            {
                UdpReceiveResult result = ReceiveAsync().Result;

                ThreadPool.QueueUserWorkItem(new WaitCallback(o => HandleReceivedData(result)));
            }
        }

        protected abstract void HandleReceivedData(UdpReceiveResult result);
    }
}
