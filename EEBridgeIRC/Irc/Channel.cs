using System.Collections.Generic;
using System.Linq;
using EEBridgeIrc.Irc.Commands.Sent;
using EEBridgeIrc.Irc.Commands.Sent.Announcements;
using EEBridgeIrc.Irc.Commands.Sent.Replies;
using PlayerIOClient;

namespace EEBridgeIrc.Irc
{
    public class ClientWorld
    {
        public Channel IrcChannel { get; set; }
        public IrcClient IrcClient { get; set; }
        public Connection Connection { get; set; }
        public string RoomName { get; set; }
        public string OwnerName { get; set; }

        public List<ClientWorldPlayer> Players = new List<ClientWorldPlayer>();

        public ClientWorld(Channel channel, IrcClient client, Connection connection)
        {
            this.IrcChannel = channel;
            this.IrcClient = client;
            this.Connection = connection;

            this.Connection.OnDisconnect += (s, e) => {
                new PrivateMessageAnnouncement() {
                            SenderMask = string.Format("{0}!~{1}@{2}", "-EEBridgeIRC-", "bridge", "system.irc"),
                            Recipient = "#" + this.IrcChannel.Name,
                            Message = $"4You have disconnected from the room."
                }.SendMessageToClient(this.IrcClient);
            };

            this.Connection.OnMessage += (s, e) => {
                switch (e.Type) {
                    case "init":
                        this.OwnerName = (string)e[0];
                        this.RoomName = (string)e[1];

                        new ChannelTopicReply() {
                            ChannelName = this.IrcChannel.Name,
                            RecipientNickName = this.IrcClient.NickName,
                            SenderAddress = Server.HostName,
                            Topic = $"\"{e[1]}\" by [violet]({e[0]}) | [violet]({e[2]}) [bold, red](plays), [violet]({e[4]}) [bold, blue](likes), [violet]({e[3]}) [bold, lightgreen](favourites).".FormatIRC()
                        }.SendMessageToClient(this.IrcClient);

                        this.Connection.Send("init2");
                        break;
                    case "add": {
                            // If the xuser already exists within the list of players, retrieve them.
                            // and update their relevant properties. Otherwise, add them to the list.
                            var player = this.Players.Find(p => p.ConnectUserId == (string)e[2]);
                            if (player != null)
                                player.Id = (int)e[0];
                            else { 
                                player = new ClientWorldPlayer() {
                                    Id = (int)e[0],
                                    Username = (string)e[1],
                                    ConnectUserId = (string)e[2],

                                    IsFriend = (bool)e[12],
                                    CanEdit = (bool)e[24]
                                };

                                this.Players.Add(player);
                            }

                            var permission = this.OwnerName == player.Username ? "@" :
                                             player.CanEdit ? "%" :
                                             player.IsFriend ? "+" :
                                             player.IsStaff() ? "&" :
                                             player.IsLocalAdmin() ? "~" : "";

                            new UserJoinedChannelAnnouncement {
                                UserMask = string.Format("{0}{1}!~{2}@{3}", permission, e[1], e[1], "world.ee"),
                                ChannelName = this.IrcChannel.Name
                            }.SendMessageToClient(this.IrcClient);
                        }
                        break;
                    case "left": {
                            var player = this.Players.Find(p => p.Id == (int)e[0]);

                            new UserPartedChannelAnnouncement() {
                                SenderMask = string.Format("{0}!~{1}@{2}", player.Username, player.Username, "world.ee"),
                                ChannelName = this.IrcChannel.Name
                            }.SendMessageToClient(this.IrcClient);

                            this.Players.Remove(player);
                        }
                        break;
                    case "say": {
                            var player = this.Players.Find(p => p.Id == (int)e[0]);

                            new PrivateMessageAnnouncement() {
                                SenderMask = string.Format("{0}!~{1}@{2}", player.Username, player.Username, "world.ee"),
                                Recipient = "#" + this.IrcChannel.Name,
                                Message = (string)e[1]
                            }.SendMessageToClient(this.IrcClient);
                        }
                        break;
                    case "write":
                        new PrivateMessageAnnouncement() {
                            SenderMask = string.Format("{0}!~{1}@{2}", "-SYSTEM-", "write", "system.ee"),
                            Recipient = "#" + this.IrcChannel.Name,
                            Message = $"4{(string)e[0]}: {(string)e[1]}"
                        }.SendMessageToClient(this.IrcClient);
                        break;
                    case "say_old":
                        new PrivateMessageAnnouncement() {
                            SenderMask = string.Format("{0}!~{1}@{2}", $"{e[0]}", "old-msg", "system.ee"),
                            Recipient = "#" + this.IrcChannel.Name,
                            Message = $"15{(string)e[1]}"
                        }.SendMessageToClient(this.IrcClient);
                        break;
                    case "updatemeta":
                        if (this.RoomName != (string)e[1]) {
                            this.RoomName = (string)e[1];

                            new ChannelTopicReply() {
                                ChannelName = this.IrcChannel.Name,
                                RecipientNickName = this.IrcClient.NickName,
                                SenderAddress = Server.HostName,
                                Topic = $"\"{e[1]}\" by [violet]({e[0]}) | [violet]({e[2]}) [bold, red](plays), [violet]({e[4]}) [bold, blue](likes), [violet]({e[3]}) [bold, lightgreen](favourites).".FormatIRC()
                            }.SendMessageToClient(this.IrcClient);
                        }
                        break;
                    case "clear":
                        new PrivateMessageAnnouncement() {
                            SenderMask = string.Format("{0}!~{1}@{2}", "-SYSTEM-", "clear", "system.ee"),
                            Recipient = "#" + this.IrcChannel.Name,
                            Message = $"4* SYSTEM: The world has been cleared."
                        }.SendMessageToClient(this.IrcClient);
                        break;
                    case "editRights":  {
                        var player = this.Players.Find(p => p.Id == (int)e[0]);

                        if (player.CanEdit == (bool)e[1])
                            break;

                        player.CanEdit = (bool)e[1];
                        
                        new UserModeAnnouncement {
                            SenderMask = string.Format("{0}!~{1}@{2}", "-SYSTEM-", "mode", "system.ee"),
                            Channel = "#" + this.IrcChannel.Name,
                            Permission = player.CanEdit ? "+h" : "-h",
                            Recipient = player.Username
                        }.SendMessageToClient(this.IrcClient);
                    } break;
                    case "access":
                        new UserModeAnnouncement {
                            SenderMask = string.Format("{0}!~{1}@{2}", "-SYSTEM-", "mode", "system.ee"),
                            Channel = "#" + this.IrcChannel.Name,
                            Permission = "+h",
                            Recipient = this.IrcClient.NickName
                        }.SendMessageToClient(this.IrcClient);
                        break;
                    case "lostaccess":
                        new UserModeAnnouncement {
                            SenderMask = string.Format("{0}!~{1}@{2}", "-SYSTEM-", "mode", "system.ee"),
                            Channel = "#" + this.IrcChannel.Name,
                            Permission = "-h",
                            Recipient = this.IrcClient.NickName
                        }.SendMessageToClient(this.IrcClient);
                        break;
                }
            };
        }
    }

    public class ClientWorldPlayer
    {
        public int Id { get; set; }
        public string ConnectUserId { get; set; }
        public string Username { get; set; }

        public bool IsFriend { get; set; }
        public bool CanEdit { get; set; }

        public bool IsStaff() => Server.EEStaff.Any(p => p == Username.ToLower());
        public bool IsLocalAdmin() => new List<string> { "atilla" }.Any(p => p == Username.ToLower());
    }

    public class Channel
    {
        private readonly List<IrcClient> _clients;

        public IEnumerable<IrcClient> Clients => _clients;

        public List<ClientWorld> Connections = new List<ClientWorld>();

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

            this.Connections.Add(new ClientWorld(this, client, client._client.Multiplayer.CreateJoinRoom(
                    this.Name, "Everybodyedits" + client._client.BigDB.Load("config", "config")["version"], true, null, null)));

            // announce the join to all clients in the channel
            foreach (var channelClient in _clients) {
                new UserJoinedChannelAnnouncement {
                    UserMask = client.UserMask,
                    ChannelName = Name
                }.SendMessageToClient(channelClient);
            }

            new ChannelUserListReply {
                SenderAddress = Server.HostName,
                RecipientNickName = client.NickName,
                ChannelName = this.Name,
                Users = _clients.Where(x => x != client).Select(x => x.NickName).ToArray()
            }.SendMessageToClient(client);

            new ChannelUserListEndReply() {
                SenderAddress = Server.HostName,
                ClientNick = client.NickName,
                ChannelName = Name
            }.SendMessageToClient(client);

            this.Connections.First(x => x.IrcClient == client).Connection.Send("init");
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
