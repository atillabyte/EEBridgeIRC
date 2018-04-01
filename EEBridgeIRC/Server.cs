using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using EEBridgeIrc.Irc;
using PlayerIOClient;

namespace EEBridgeIrc
{
    public class Server
    {
        private readonly TcpListener _listener;
        private readonly List<IrcClient> _ircClients;
        private readonly IrcCommandProcessor _commandProcessor;
        private readonly IrcController _controller;
        private Task _clientListenTask;

        public static Client EEGuestClient { get; internal set; }

        public static string HostName { get; internal set; }

        public bool IsRunning { get; private set; }

        public Server(IPAddress ip, int port)
        {
            EEGuestClient = PlayerIO.QuickConnect.SimpleConnect("everybody-edits-su9rn58o40itdbnw69plyw", "guest", "guest", null);

            _listener = new TcpListener(ip, port);
            _ircClients = new List<IrcClient>();
            _controller = new IrcController(_ircClients);
            _commandProcessor = new IrcCommandProcessor(_controller);
        }

        public void Run()
        {
            _listener.Start();
            IsRunning = true;

            _clientListenTask = ListenForClients();
        }

        private void ClientConnected(TcpClient client, int clientNumber)
        {
            var user = new IrcClient(client, clientNumber);
            user.IrcCommandReceived += _commandProcessor.ProcessCommand;
            user.IrcUserActivated += _controller.SendActivationMessages;
            _ircClients.Add(user);

            Console.WriteLine("Client {0} Connected", clientNumber);
        }

        private async Task ListenForClients()
        {
            var numClients = 0;
            while (IsRunning)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                ClientConnected(tcpClient, numClients);
                numClients++;
            }

            _listener.Stop();
        }
    }
}
