using EEBridgeIrc.Irc.Commands.Sent;
using EEBridgeIrc.Irc.Commands.Sent.Errors;

namespace EEBridgeIrc.Irc.Commands.Received
{
    [IrcCommand("JOIN")]
    public class JoinCommand : IReceivedCommand
    {
        public void ProcessCommand(string[] args, IrcClient client, IrcController controller)
        {
            if (args.Length == 0)
            {
                new NeedMoreParamsError
                {
                    Command = "JOIN",
                    SenderAddress = Server.HostName,
                    SenderNickName = client.NickName
                }.SendMessageToClient(client);
                return;
            }

            var name = args[0].Trim();
            if (!name.StartsWith("#"))
            {
                new NoSuchChannelError
                {
                    SenderAddress = Server.HostName,
                    ChannelName = name,
                    ClientNick = client.NickName
                }.SendMessageToClient(client);

                return;
            }

            if (name.StartsWith("#"))
                name = name.Substring(1);

            controller.JoinChannel(client, name);
        }
    }
}
