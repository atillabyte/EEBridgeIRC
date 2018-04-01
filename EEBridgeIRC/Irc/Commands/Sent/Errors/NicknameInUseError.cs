namespace EEBridgeIrc.Irc.Commands.Sent.Errors
{
    public class NicknameInUseError : ISentCommand
    {
        private const int ResponseCode = 433;
        private const string Message = "Nickname is already in use.";

        public string SenderAddress { get; set; }
        public string SenderNickName { get; set; }
        public string AttemptedNickName { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} {3} :{4}",
                                 SenderAddress, ResponseCode, SenderNickName, AttemptedNickName, Message);
        }
    }
}
