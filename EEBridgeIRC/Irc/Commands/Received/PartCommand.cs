namespace EEBridgeIrc.Irc.Commands.Received
{
    [IrcCommand("PART")]
    public class PartCommand : IReceivedCommand
    {
        public void ProcessCommand(string[] args, IrcClient client, IrcController controller)
        {
            if (args.Length == 0)
                return;

            if (!args[0].StartsWith("#") || args[0].Length < 2)
                return;

            var channel = args[0].Substring(1);
            controller.PartChannel(client, channel);
        }
    }
}
