namespace EEBridgeIrc.Irc.Commands.Sent.Errors
{
    public class NeedMoreParamsError : ISentCommand
    {
        private const int ResponseCode = 461;
        private const string ResponseText = ":Not enough parameters";

        public string SenderAddress { get; set; }
        public string Command { get; set; }
        public string SenderNickName { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} {3} {4}",
                SenderAddress, ResponseCode, SenderNickName, Command, ResponseText);
        }
    }
}
