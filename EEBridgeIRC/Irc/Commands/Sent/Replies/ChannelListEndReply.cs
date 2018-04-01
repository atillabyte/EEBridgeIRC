namespace EEBridgeIrc.Irc.Commands.Sent.Replies
{

    public class ChannelListEndReply : ISentCommand
    {
        private const int ResponseCode = 323;
        private const string Message = "End of /LIST";

        public string SenderAddress { get; set; }
        public string ClientNick { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} :{3}",
                                 SenderAddress, ResponseCode, ClientNick, Message);
        }
    }
}
