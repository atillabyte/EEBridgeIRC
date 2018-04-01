using System;

namespace EEBridgeIrc.Irc.Commands.Sent
{
    public static class SentCommandExtensions
    {
        public static void SendMessageToClient(this ISentCommand command, IrcClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            client.SendMessage(command.FormFullResponseString());
        }
    }
}
