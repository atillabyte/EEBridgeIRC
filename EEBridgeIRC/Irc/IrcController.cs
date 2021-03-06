﻿using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using EEBridgeIrc.Irc.Commands.Sent;
using EEBridgeIrc.Irc.Commands.Sent.Announcements;
using EEBridgeIrc.Irc.Commands.Sent.Errors;
using EEBridgeIrc.Irc.Commands.Sent.Replies;

namespace EEBridgeIrc.Irc
{
    public class IrcController
    {
        private readonly List<IrcClient> _clients;
        private readonly List<Channel> _channels;

        public IEnumerable<IrcClient> Clients => _clients;
        public IEnumerable<Channel> Channels => _channels;

        public IrcController(List<IrcClient> clients)
        {
            _clients = clients;
            _channels = new List<Channel>();
        }

        public void SendMessage(IrcClient client, string message)
        {
            client.SendMessage(message);
        }

        public void SendActivationMessages(IrcClient client)
        {
            var motd_file_path = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                                 "motd.txt");

            var message = new WelcomeReply
            {
                Message = File.Exists(motd_file_path) ? File.ReadAllText(motd_file_path).FormatIRC().Replace("%username%", client.UserName)
                                                  : "Welcome to the Everybody Edits IRC Bridge!",
                SenderAddress = Server.HostName,
                SenderNickName = client.NickName
            }.FormFullResponseString();

            client.SendMessage(message);
        }

        public void SendPrivateMessageToUser(IrcClient sender, string recipientNickName, string message)
        {
            // Find the client with the nickname
            var client = _clients.Find(x => x.NickName.Equals(recipientNickName));
            if (client == null || !client.ConnectionActive)
            {
                var errorMessage = new NoSuchNickError
                {
                    SenderAddress = Server.HostName,
                    SenderNickName = sender.NickName,
                    RecipientNickName = recipientNickName
                }.FormFullResponseString();

                sender.SendMessage(errorMessage);
                return;
            }

            new PrivateMessageAnnouncement
            {
                SenderMask = sender.UserMask,
                Recipient = recipientNickName,
                Message = message
            }.SendMessageToClient(client);
        }

        public bool NickNameInUse(string nickname)
        {
            return _clients.Any(x => x.NickName.Equals(nickname, StringComparison.InvariantCultureIgnoreCase));
        }

        public void JoinChannel(IrcClient client, string channelName)
        {
            // if the channel doesn't exist yet, create it
            var channel = _channels.Find(x => x.Name.Equals(channelName, StringComparison.InvariantCultureIgnoreCase));

            if (channel == null)
            {
                channel = new Channel(channelName);
                _channels.Add(channel);
            }

            channel.JoinClient(client);
        }

        public void PartChannel(IrcClient client, string channelName)
        {
            var channel = _channels.Find(x => x.Name.Equals(channelName, StringComparison.InvariantCultureIgnoreCase));

            if (channel == null)
                return;

            channel.Connections.First(t => t.IrcClient == client).Connection.Disconnect();
            channel.PartClient(client);
        }

        public void SendMessageToChannel(IrcClient sender, string channelName, string message)
        {
            var channel = _channels.Find(x => x.Name.Equals(channelName, StringComparison.InvariantCultureIgnoreCase));

            if (channel == null)
                return;

            channel.Connections.First(t => t.IrcClient == sender).Connection.Send("say", message);
        }
    }
}
