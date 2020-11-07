using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DSServerCommon;
using DSServerCommon.Packets;
using NetCoreServer;
using DSServer.Server;
using System.Collections.Concurrent;

namespace DSServer
{
    public class DSServer : TcpServer
    {
        public PacketHandler<DSSession> PacketHandler { get; private set; }
        public new ConcurrentDictionary<long, DSSession> Sessions { get; private set; }

        private ILogger _logger;

        public DSServer(IPAddress ip, int port, ILogger logger) : base(ip, port)
        {
            _logger = logger;
            PacketHandler = new ServerPacketHandler(logger);
            Sessions = new ConcurrentDictionary<long, DSSession>();
        }

        public void OnUserDisconnected(long id)
        {
            if (id < 0)
                return;

            Sessions.TryRemove(id, out DSSession session);
        }

        protected override TcpSession CreateSession()
        {
            return new DSSession(this, PacketHandler, _logger);
        }

        protected override void OnError(SocketError error)
        {
            _logger.Log($"SocketError: {error}", LogLevel.Error);
        }

        protected override void OnDisconnected(TcpSession session)
        {
            base.OnDisconnected(session);
        }

        protected override void OnConnected(TcpSession session)
        {
            base.OnConnected(session);
            DSSession dsession = (DSSession)session;
            dsession.IPEndPoint = (IPEndPoint)session.Socket.RemoteEndPoint;
        }
    }
}
