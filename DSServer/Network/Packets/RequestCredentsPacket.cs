using DSServerCommon.Network;
using DSServerCommon.Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DSServer.Network.Packets
{
    public class RequestCredentsPacket : Packet
    {
        string _username;
        string _password;

        public RequestCredentsPacket(Guid id) : base((int)PacketId.RequestCredents)
        {
            Write((byte)1);
            Write(id);
        }

        public RequestCredentsPacket(bool isAuthenticated) : base((int)PacketId.RequestCredents)
        {
            Write((byte)2);
            Write(isAuthenticated);
        }

        public RequestCredentsPacket(ref byte[] data, NetState state) : base(ref data, state, (int)PacketId.RequestCredents)
        {
            _username = ReadString();
            _password = ReadString();
        }

        public override void InvokePacket(NetState state)
        {
            bool result = (state as Client).Authenticate(_username, _password);

            RequestCredentsPacket rcp = new RequestCredentsPacket(result);
            state.Write(rcp);

            if (!result)
                Task.Run(state.TryDisconnectAsync);
        }
    }
}
