namespace EEBridgeIrc.Irc.Commands.Sent.Replies
{
    public class ChannelUserListEndReply : ISentCommand
    {
        private const int ResponseCode = 366;
        private const string Message = "End of /NAMES list.";

        public string SenderAddress { get; set; }
        public string ChannelName { get; set; }
        public string ClientNick { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} #{3} :{4}",
                                 SenderAddress, ResponseCode, ClientNick, ChannelName, Message);
        }
    }
}
