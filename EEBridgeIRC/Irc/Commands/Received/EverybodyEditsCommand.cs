using EEBridgeIrc.Irc.Commands.Sent;
using EEBridgeIrc.Irc.Commands.Sent.Replies;
using EEBridgeIrc.Irc.Commands.Sent.Errors;
using System.Linq;
using System;

namespace EEBridgeIrc.Irc.Commands.Received
{
    [IrcCommand("CMD", true)]
    public class EverybodyEditsCommand : IReceivedCommand
    {
        public void ProcessCommand(string[] args, IrcClient client, IrcController controller)
        {
            if (args.Length < 3 || !args[1].StartsWith("#"))
                return;
            
            var command = args[0];
            var channel = args[1];
            var extra = string.Join(" ", args.Skip(2).ToArray());
            
            var target = controller.Channels.ToList().Find(ch => ch.Name.Equals(channel.Remove(0, 1), StringComparison.InvariantCultureIgnoreCase));
            
            if (target == null)
            {
                var errorMessage = new NoSuchChannelError
                {
                    SenderAddress = Server.HostName,
                    ClientNick = client.NickName,
                    ChannelName = channel
                }.FormFullResponseString();

                client.SendMessage(errorMessage);
                return;
            }
            
            target.Connections.First(t => t.IrcClient == client).Connection.Send("say", $"/{command} {extra}");
        }
    }
}