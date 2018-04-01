namespace EEBridgeIrc.Irc.Commands.Sent.Announcements
{
    public class PrivateMessageAnnouncement : ISentCommand
    {
        private const string CommandName = "PRIVMSG";

        public string SenderMask { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} :{3}",
                                 SenderMask, CommandName, Recipient, Message);
        }
    }
}
