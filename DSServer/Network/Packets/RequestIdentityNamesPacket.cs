using DSServer.ChatSystem;
using DSServer.Users;
using DSServerCommon.ChatSystem;
using DSServerCommon.Network;
using DSServerCommon.Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Network.Packets
{
    public class RequestIdentityNamesPacket : Packet
    {
        List<Guid> _ids;

        public RequestIdentityNamesPacket(Dictionary<Guid, string> identities) : base((int)PacketId.RequestIdentityNames)
        {
            Write(identities.Count);

            foreach(var pair in identities)
            {
                Write(pair.Key);
                Write(pair.Value);
            }
        }

        public RequestIdentityNamesPacket(ref byte[] data, NetState state) : base(ref data, state, (int)PacketId.RequestIdentityNames)
        {
            int count = ReadInt();
            _ids = new List<Guid>(count);

            for (int i = 0; i < count; i++)
                _ids.Add(ReadGuid());
        }

        public override void InvokePacket(NetState state)
        {
            Dictionary<Guid, string> identities = new Dictionary<Guid, string>();

            foreach(Guid id in _ids)
            {
                if (IdentityManager.TryGetIdentity(id, out ChatIdentity identity))
                    identities.Add(id, identity.Name);
                else
                    identities.Add(id, string.Empty);
            }

            RequestIdentityNamesPacket rcrnp = new RequestIdentityNamesPacket(identities);
            state.Write(rcrnp);
        }
    }
}
