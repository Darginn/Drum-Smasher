using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Network
{
    public class User : IEquatable<User>
    {
        public bool Authenticated;
        public DateTime LastPing;
        public EncryptionKey EncryptionKey;

        public IPEndPoint IPEndPoint { get; private set; }
        public UdpClient Client { get; private set; }

        public Server Server { get; private set; }

        public User(IPEndPoint ipEndPoint)
        {
            IPEndPoint = ipEndPoint;
            Client = new UdpClient(ipEndPoint);
        }

        public User(UdpClient client)
        {
            Client = client;
            IPEndPoint = (IPEndPoint)client.Client.LocalEndPoint;
            LastPing = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public bool Equals([AllowNull] User other)
        {
            return other != null &&
                   EqualityComparer<IPEndPoint>.Default.Equals(IPEndPoint, other.IPEndPoint);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IPEndPoint);
        }

        public static bool operator ==(User left, User right)
        {
            return EqualityComparer<User>.Default.Equals(left, right);
        }

        public static bool operator !=(User left, User right)
        {
            return !(left == right);
        }
    }
}
