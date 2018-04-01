using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EEBridgeIrc
{
    public class NetworkClient
    {
        private NetworkStream _networkStream;

        public bool IsActive { get; set; }
        public int Id { get; }
        public TcpClient Socket { get; }
        public Task ReceiveInputTask { get; }
        public string RemoteAddress { get; }

        public event MessageReceivedDelegate MessageReceived;
        public event ClientDisconnectedDelegate ClientDisconnected;

        public NetworkClient(TcpClient socket, int id)
        {
            this.Socket = socket;
            this.Id = id;

            RemoteAddress = this.Socket.Client.RemoteEndPoint.ToString();
        }

        public async Task ReceiveInput()
        {
            IsActive = true;
            _networkStream = this.Socket.GetStream();

            using (var reader = new StreamReader(_networkStream)) {
                while (IsActive) {
                    try {
                        var content = await reader.ReadLineAsync();

                        // If content is null, that means the connection has been gracefully disconnected
                        if (content == null) {
                            MarkAsDisconnected();

                            return;
                        }

                        MessageReceived?.Invoke(this, content);
                    }

                    // If the tcp connection is ungracefully disconnected, it will throw an exception
                    catch (IOException) {
                        MarkAsDisconnected();

                        return;
                    }
                }
            }
        }

        public async Task SendLine(string line)
        {
            if (!IsActive)
                return;

            try
            {
                // Don't use a using statement as we do not want the stream closed
                //    after the write is completed
                var writer = new StreamWriter(_networkStream);
                await writer.WriteLineAsync(line);
                writer.Flush();
            }
            catch (IOException)
            {
                MarkAsDisconnected(); // socket closed
            }
        }

        private void MarkAsDisconnected()
        {
            IsActive = false;
            ClientDisconnected?.Invoke(this);
        }
    }
}
