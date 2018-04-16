namespace EEBridgeIrc.Irc.Commands.Sent.Announcements
{
    public class UserModeAnnouncement : ISentCommand
    {
        private const string CommandName = "MODE";

        public string SenderMask { get; set; }
        public string Channel { get; set; }
        public string Permission { get; set; }
        public string Recipient { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} {1} {2} {3} {4}",
                                 SenderMask, CommandName, Channel, Permission, Recipient);
        }
    }
}


// this.IrcClient.SendMessage(string.Format(":-SYSTEM-!world@system.ee MODE #{0} +v {1}", this.IrcChannel.Name, player.Username));