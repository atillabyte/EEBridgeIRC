using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EEBridgeIrc
{
    public class NetworkClient
    {
        public string RemoteAddress { get; }
        public bool IsActive { get; set; }
        public int Id { get; }
        public TcpClient Socket { get; }
        internal NetworkStream NetworkStream;
        public Task ReceiveInputTask { get; }

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
            NetworkStream = this.Socket.GetStream();

            using (var reader = new StreamReader(NetworkStream)) {
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
            if (!this.IsActive)
                return;

            try
            {
                // don't use a using statement as we do not want the stream closed after the write is completed
                var writer = new StreamWriter(NetworkStream);
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
            this.IsActive = false;

            ClientDisconnected?.Invoke(this);
        }
    }
}
