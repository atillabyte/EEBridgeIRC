using EEBridgeIrc.Irc.Commands.Sent;
using EEBridgeIrc.Irc.Commands.Sent.Replies;
using System.Linq;

namespace EEBridgeIrc.Irc.Commands.Received
{
    [IrcCommand("LIST", true)]
    public class ListCommand : IReceivedCommand
    {
        public void ProcessCommand(string[] args, IrcClient client, IrcController controller)
        {
            new ChannelListBeginReply {
                SenderAddress = Server.HostName,
                ClientNick = client.NickName
            }.SendMessageToClient(client);

            var rooms = client._client.Multiplayer.ListRooms("Everybodyedits" + client._client.BigDB.Load("config", "config")["version"], null, 0, 0).ToList();

            foreach (var room in rooms.OrderByDescending(ri => ri.OnlineUsers)) {
                new ChannelListReply {
                    SenderAddress = Server.HostName,
                    RecipientNickName = client.NickName,
                    ChannelName = "#" + room.Id,
                    ChannelUserCount = room.OnlineUsers,
                    ChannelTopic = room.RoomData.ContainsKey("name") ? room.RoomData["name"] : "Untitled World"
                }.SendMessageToClient(client);
            }

            new ChannelListEndReply {
                SenderAddress = Server.HostName,
                ClientNick = client.NickName
            }.SendMessageToClient(client);
        }
    }
}
