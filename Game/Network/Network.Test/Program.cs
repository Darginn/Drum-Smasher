using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Network.Test
{
    class Program
    {
        static void Main(string[] args)
            => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        static async Task MainTask(string[] args)
        {
            try
            {
                TCPServer server = new TCPServer(IPAddress.Any, 45000);
                TCPClient client = new TCPClient(IPAddress.Parse("127.0.0.1"), 45000);

                server.Start();

                while (!server.IsStarted)
                    await Task.Delay(1);

                Console.WriteLine("Started");

                client.Connect();

                while (!client.IsConnected)
                    await Task.Delay(1);

                Console.WriteLine("Connected");

                await Task.Delay(500);

                client.Send(Encoding.UTF8.GetBytes("Hello World"));
                Console.WriteLine("Sent");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(-1);
        }
    }

    public class UDPServer : NetCoreServer.UdpServer
    {
        public UDPServer(IPAddress ip, int port) : base(ip, port)
        {

        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            Console.WriteLine(Encoding.UTF8.GetString(buffer));
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine(error);
        }
    }

    public class UDPClient : NetCoreServer.UdpClient
    {
        public UDPClient(IPAddress ip, int port) : base(ip, port)
        {

        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            Console.WriteLine(Encoding.UTF8.GetString(buffer));
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine(error);
        }
    }
    public class TCPServer : NetCoreServer.TcpServer
    {
        public TCPServer(IPAddress ip, int port) : base(ip, port)
        {

        }

        protected override void OnConnected(NetCoreServer.TcpSession session)
        {
            Console.WriteLine("New connection from " + ((IPEndPoint)session.Socket.RemoteEndPoint).ToString());
        }

        protected override NetCoreServer.TcpSession CreateSession()
        {
            return new TCPSession(this);
        }


        protected override void OnError(SocketError error)
        {
            Console.WriteLine(error);
        }
    }

    public class TCPSession : NetCoreServer.TcpSession
    {
        public TCPSession(TCPServer server) : base(server)
        {

        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            Console.WriteLine(Encoding.UTF8.GetString(buffer));
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine(error);
        }
    }

    public class TCPClient : NetCoreServer.TcpClient
    {
        public TCPClient(IPAddress ip, int port) : base(ip, port)
        {

        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            Console.WriteLine(Encoding.UTF8.GetString(buffer));
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine(error);
        }
    }
}
