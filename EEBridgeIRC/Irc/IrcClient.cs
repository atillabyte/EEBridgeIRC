using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using EEBridgeIrc.Irc.Commands.Sent;
using EEBridgeIrc.Irc.Commands.Sent.Replies;
using PlayerIOClient;

namespace EEBridgeIrc.Irc
{
    public class IrcClient
    {
        private readonly NetworkClient _networkClient;
        private readonly Task _receiveInputTask;

        public string NickName { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserMask { get; private set; }
        public bool ConnectionActive { get { return _networkClient.IsActive; } }
        public bool UserActivated { get; private set; }

        internal Client _client;

        public event IrcCommandReceivedDelegate IrcCommandReceived;
        public event UserActivatedDelegate IrcUserActivated;

        public string Password { get; set; }
        public string UsernameOrEmail { get; set; }

        public IrcClient(TcpClient tcpClient, int clientId)
        {
            _networkClient = new NetworkClient(tcpClient, clientId);
            _networkClient.ClientDisconnected += ClientSocketDisconnected;
            _networkClient.MessageReceived += ProcessClientCommand;

            _receiveInputTask = _networkClient.ReceiveInput();

            this.NickName = "*";
        }

        public async void SendMessage(string message)
        {
            await _networkClient.SendLine(message);
        }

        public void AttemptUserActivation()
        {
            if (string.IsNullOrEmpty(this.UsernameOrEmail) || string.IsNullOrEmpty(this.Password))
                return;

            var clientSuccess = false;

            try {
                _client = PlayerIO.QuickConnect.SimpleConnect("everybody-edits-su9rn58o40itdbnw69plyw", this.UsernameOrEmail, this.Password, null);
                clientSuccess = true;
            }
            catch (PlayerIOError error) {
                if (error.ErrorCode == ErrorCode.InvalidPassword)
                    new WelcomeReply { Message = "You have supplied an incorrect password for the account specified." }.SendMessageToClient(this);
            }

            if (!clientSuccess)
                return;

            this.NickName = _client.BigDB.LoadMyPlayerObject().GetString("name", this.UserName.ToLower());
            this.FullName = this.NickName;

            this.UserActivated = true;
            IrcUserActivated?.Invoke(this);
        }

        public void Disconnect()
        {
            _networkClient.Socket.Close();
        }

        public void SetUserMask()
        {
            this.UserMask = string.Format("{0}!~{1}@{2}", UserName, UserName, "world.ee.ircd");
        }

        private void ClientSocketDisconnected(NetworkClient client)
        {
            if (client != _networkClient)
                return;

            client.IsActive = false;
            Console.WriteLine("User {0} disconnected", NickName);
        }

        private void ProcessClientCommand(NetworkClient client, string command)
        {
            if (client != _networkClient)
                return;
            
            IrcCommandReceived?.Invoke(this, command);
        }
    }
}
