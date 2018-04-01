using EEBridgeIrc;
using EEBridgeIrc.Irc;
using EEBridgeIrc.Irc.Commands;

namespace EEBridgeIRC.Irc.Commands.Received
{
    [IrcCommand("PASS", false)]
    public class PassCommand : IReceivedCommand
    {
        public void ProcessCommand(string[] args, IrcClient client, IrcController controller)
        {
            if (args.Length < 1)
                return;

            client.Password = args[0];

            client.AttemptUserActivation();
        }
    }
}
