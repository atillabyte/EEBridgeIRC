namespace EEBridgeIrc.Irc.Commands.Sent.Replies
{

    public class ChannelListBeginReply : ISentCommand
    {
        private const int ResponseCode = 321;

        public string SenderAddress { get; set; }
        public string ClientNick { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} Channel :Users  Name",
                                 SenderAddress, ResponseCode, ClientNick);
        }
    }
}
