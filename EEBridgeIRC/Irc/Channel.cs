using System.Collections.Generic;
using System.Linq;
using EEBridgeIrc.Irc.Commands.Sent;
using EEBridgeIrc.Irc.Commands.Sent.Announcements;
using EEBridgeIrc.Irc.Commands.Sent.Replies;
using PlayerIOClient;

namespace EEBridgeIrc.Irc
{
    public class Channel
    {
        private readonly List<IrcClient> _clients;

        public IEnumerable<IrcClient> Clients => _clients;

        public Dictionary<IrcClient, Connection> Connections =
            new Dictionary<IrcClient, Connection>();

        public Dictionary<IrcClient, Dictionary<int, string>> RoomUsernames =
            new Dictionary<IrcClient, Dictionary<int, string>>();

        public Dictionary<IrcClient, string> RoomName =
            new Dictionary<IrcClient, string>();

        public int UserCount => _clients.Count;

        public string Name { get; }
        public string Topic { get; set; }

        public Channel(string name)
        {
            _clients = new List<IrcClient>();

            this.Name = name;
            this.Topic = "No Topic Set";
        }

        public void JoinClient(IrcClient client)
        {
            if (_clients.Contains(client))
                return; // already in this channel

            _clients.Add(client);

            Connections[client] = client._client.Multiplayer.CreateJoinRoom(this.Name,
                    "Everybodyedits" + client._client.BigDB.Load("config", "config")["version"], true, null, null);
            RoomUsernames.Add(client, new Dictionary<int, string>());

            Connections[client].OnDisconnect += (s, e) => {
                client.Disconnect();
            };

            Connections[client].OnMessage += (s, e) => {
                switch (e.Type) {
                    case "init":
                        if (!RoomName.ContainsKey(client)) // just in case..? ;-;
                            RoomName.Add(client, (string)e[1]);

                        new ChannelTopicReply() {
                            ChannelName = this.Name,
                            RecipientNickName = client.NickName,
                            SenderAddress = Server.HostName,
                            Topic = $"\"{e[1]}\" by {e[0]} | {e[2]} plays {e[4]} likes {e[3]} favourites."
                        }.SendMessageToClient(client);

                        Connections[client].Send("init2");
                        break;
                    case "add":
                        RoomUsernames[client].Add((int)e[0], (string)e[1]);

                        new UserJoinedChannelAnnouncement {
                            UserMask = string.Format("{0}!~{1}@{2}", e[1], e[1], "world.ee"),
                            ChannelName = Name
                        }.SendMessageToClient(client);
                        break;
                    case "left": {
                            var username = RoomUsernames[client][(int)e[0]];

                            RoomUsernames[client].Remove((int)e[0]);
                            new UserPartedChannelAnnouncement() {
                                SenderMask = string.Format("{0}!~{1}@{2}", username, username, "world.ee"),
                                ChannelName = this.Name
                            }.SendMessageToClient(client);
                        }
                        break;
                    case "say": {
                            var username = RoomUsernames[client][(int)e[0]];

                            new PrivateMessageAnnouncement() {
                                SenderMask = string.Format("{0}!~{1}@{2}", username, username, "world.ee"),
                                Recipient = "#" + this.Name,
                                Message = (string)e[1]
                            }.SendMessageToClient(client);
                        }
                        break;
                    case "write":
                        new PrivateMessageAnnouncement() {
                            SenderMask = string.Format("{0}!~{1}@{2}", "-SYSTEM-", "write", "system.ee"),
                            Recipient = "#" + this.Name,
                            Message = (string)e[0] + ": " + (string)e[1]
                        }.SendMessageToClient(client);
                        break;
                    case "say_old":
                        new PrivateMessageAnnouncement() {
                            SenderMask = string.Format("{0}!~{1}@{2}", $"_{e[0]}_", "old-msg", "system.ee"),
                            Recipient = "#" + this.Name,
                            Message = (string)e[1]
                        }.SendMessageToClient(client);
                        break;
                    case "updatemeta":
                        if (RoomName.ContainsKey(client) && RoomName[client] == (string)e[1])
                            break;

                        if (!RoomName.ContainsKey(client))
                            RoomName.Add(client, (string)e[1]);
                        else RoomName[client] = (string)e[1];

                        new ChannelTopicReply() {
                            ChannelName = this.Name,
                            RecipientNickName = client.NickName,
                            SenderAddress = Server.HostName,
                            Topic = $"\"{e[1]}\" by {e[0]} | {e[2]} plays {e[4]} likes {e[3]} favourites."
                        }.SendMessageToClient(client);
                        break;
                }
            };

            Connections[client].Send("init");
        }

        public void PartClient(IrcClient client)
        {
            if (!_clients.Contains(client))
                return;

            // relay the client parting to all clients
            foreach (var ircClient in _clients)
            {
                new UserPartedChannelAnnouncement
                {
                    SenderMask = client.UserMask,
                    ChannelName = Name
                }.SendMessageToClient(ircClient);
            }

            _clients.Remove(client);
        }

        public void BroadcastMessage(IrcClient source, string message)
        {
            foreach (var ircClient in _clients)
            {
                if (ircClient == source)
                    continue;

                new PrivateMessageAnnouncement
                {
                    SenderMask = source.UserMask,
                    Recipient = "#" + Name,
                    Message = message
                }.SendMessageToClient(ircClient);
            }
        }
    }
}
