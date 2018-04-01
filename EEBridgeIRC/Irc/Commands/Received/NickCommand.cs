using System;
using System.Linq;

namespace EEBridgeIrc.Irc.Commands.Received
{
    [IrcCommand("NICK", false)]
    public class NickCommand : IReceivedCommand
    {
        public void ProcessCommand(string[] args, IrcClient client, IrcController controller)
        {
            if (string.IsNullOrEmpty(client.NickName))
                client.NickName = new string(Guid.NewGuid().ToString().Substring(3, 12).Where(char.IsLetterOrDigit).ToArray());
        }
    }
}
