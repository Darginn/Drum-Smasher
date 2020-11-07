using DSServerCommon.Packets;
using DSServerCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Server.Packets
{
    public class JoinChatPacket : BasePacket<DSSession>
    {
        private bool _isJoined;
        private long _userId;
        private string _channelName;
        private long _channelId;


        public JoinChatPacket(ILogger logger) : base(PacketId.JoinChat, logger)
        {

        }

        public JoinChatPacket(long userId, long channelId, string channelName, bool isJoined, ILogger logger) : base(PacketId.JoinChat, logger)
        {
            _userId = userId;
            _channelId = channelId;
            _isJoined = isJoined;
            _channelName = channelName;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, DSSession from)
        {
            _logger.Log($"Reading join request from {from.AccountData.Id}:{from.AccountData.Name}");

            _channelId = reader.ReadInt64();
            from.TryJoinChat(_channelId);

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            _logger.Log($"Writing join request for {_userId}");

			writer.Write(_isJoined);
            writer.Write(_userId);
            writer.Write(_channelId);
            writer.Write(_channelName);
            return writer;
        }
    }
}
