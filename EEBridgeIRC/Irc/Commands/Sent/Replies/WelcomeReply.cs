using System;

namespace EEBridgeIrc.Irc.Commands.Sent.Replies
{
    public class WelcomeReply : ISentCommand
    {
        private const int ResponseCode = 001;
        public string Message = "Welcome to the Everybody Edits IRC bridge.";

        public string SenderAddress { get; set; }
        public string SenderNickName { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} {3}",
                                 SenderAddress, ResponseCode.ToString("000"), SenderNickName, Message);
        }
    }
}
