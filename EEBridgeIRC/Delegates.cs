using EEBridgeIrc.Irc;

namespace EEBridgeIrc
{
    public delegate void MessageReceivedDelegate(NetworkClient client, string message);

    public delegate void ClientDisconnectedDelegate(NetworkClient client);

    public delegate void IrcCommandReceivedDelegate(IrcClient client, string command);

    public delegate void UserActivatedDelegate(IrcClient client);
}
