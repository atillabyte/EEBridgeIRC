using System;

namespace EEBridgeIrc.Irc.Commands.Sent.Announcements
{
    public class UserJoinedChannelAnnouncement : ISentCommand
    {
        public string UserMask { get; set; }
        public string ChannelName { get; set; }

        public string FormFullResponseString()
        {
            return string.Format(":{0} JOIN #{1}", UserMask, ChannelName);
        }
    }
}
