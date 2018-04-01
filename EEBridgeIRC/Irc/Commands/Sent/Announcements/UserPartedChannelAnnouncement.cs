namespace EEBridgeIrc.Irc.Commands.Sent.Announcements
{
    public class UserPartedChannelAnnouncement : ISentCommand
    {
        private const string CommandName = "PART";

        public string SenderMask { get; set; }
        public string ChannelName { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} #{2}",
                SenderMask, CommandName, ChannelName);
        }
    }
}
