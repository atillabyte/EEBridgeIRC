namespace EEBridgeIrc.Irc.Commands
{
    public interface IReceivedCommand
    {
        void ProcessCommand(string[] args, IrcClient client, IrcController controller);
    }
}
