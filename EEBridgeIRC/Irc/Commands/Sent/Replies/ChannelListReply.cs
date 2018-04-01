using System;

namespace EEBridgeIrc.Irc.Commands.Sent.Replies
{
    public class ChannelListReply : ISentCommand
    {
        private const int ResponseCode = 322;

        public string SenderAddress { get; set; }
        public string RecipientNickName { get; set; }
        public string Topic { get; set; }
        public string ChannelName { get; set; }
        public string ChannelTopic { get; set; }
        public int ChannelUserCount { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} {3} {4} :{5}",
                                 SenderAddress, ResponseCode, RecipientNickName, ChannelName, ChannelUserCount, ChannelTopic);
        }
    }
}
