using System;

namespace EEBridgeIrc.Irc.Commands.Sent.Replies
{
    public class ChannelTopicReply : ISentCommand
    {
        private const int ResponseCode = 332;

        public string SenderAddress { get; set; }
        public string RecipientNickName { get; set; }
        public string Topic { get; set; }
        public string ChannelName { get; set; }
        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} #{3} :{4}",
                                 SenderAddress, ResponseCode, RecipientNickName, ChannelName, Topic);
        }
    }
}
