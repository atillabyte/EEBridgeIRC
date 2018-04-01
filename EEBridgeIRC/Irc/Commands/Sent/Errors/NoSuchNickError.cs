namespace EEBridgeIrc.Irc.Commands.Sent.Errors
{
    public class NoSuchNickError : ISentCommand
    {
        private const int ResponseCode = 401;
        private const string Message = "No such nick/channel";

        public string SenderAddress { get; set; }
        public string SenderNickName { get; set; }
        public string RecipientNickName { get; set; }
        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} {3} :{4}",
                                 SenderAddress, ResponseCode, SenderNickName, RecipientNickName, Message);
        }
    }
}
