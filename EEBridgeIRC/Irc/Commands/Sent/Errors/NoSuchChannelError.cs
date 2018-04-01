namespace EEBridgeIrc.Irc.Commands.Sent.Errors
{
    public class NoSuchChannelError : ISentCommand
    {
        private const int ResponseCode = 403;
        private const string Message = "No such channel";

        public string SenderAddress { get; set; }
        public string ClientNick { get; set; }
        public string ChannelName { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} {3} :{4}",
                                 SenderAddress, ResponseCode, ClientNick, ChannelName, Message);
        }
    }
}
