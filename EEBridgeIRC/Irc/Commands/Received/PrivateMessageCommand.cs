using System;

namespace EEBridgeIrc.Irc.Commands.Received
{
    [IrcCommand("PRIVMSG")]
    public class PrivateMessageCommand : IReceivedCommand
    {
        public void ProcessCommand(string[] args, IrcClient client, IrcController controller)
        {
            if (args.Length < 2)
                return;

            // Detect if this is being sent to a user or a channel 
            if (args[0].StartsWith("#"))
            {
                var channel = args[0].Substring(1);
                controller.SendMessageToChannel(client, channel, args[1]);
            }
            else
            {
                controller.SendPrivateMessageToUser(client, args[0], args[1]);
            }
        }
    }
}
