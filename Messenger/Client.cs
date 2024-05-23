using System.Net.Sockets;
using System.Text;

namespace Messenger
{
    class Client
    {
        private Socket server;
        public string Username => AuthorizationWindow.Username;
        public EventHandler<string> NewMessageHasBeenReceived;
        public EventHandler<string> UserHasBeenConnected;
        public EventHandler<string> UserHasBeenDisconnected;

        public Client(string ipServer)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(ipServer, 8888);

            ReceiveMessage();
        }

        public async Task ReceiveMessage()
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await server.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);

                switch(message[0])
                {
                    case '#':
                        NewMessageHasBeenReceived?.Invoke(this, message.Substring(1));
                        break;
                    case '~':
                        UserHasBeenConnected?.Invoke(this, message.Substring(1));
                        break;
                    case '@':
                        UserHasBeenDisconnected?.Invoke(this, message.Substring(1));
                        break;
                    //case 'N':
                    //    SendMessage(server, $"~{Username}");
                    //    break;
                    default:
                        throw new ArgumentException(nameof(message));
                }
            }
        }

        public async void Disconnect()
        {
            server.Disconnect(false);
            await SendMessage($"@{Username}");
        }

        public async Task SendMessage(string message)
        {
            string time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
            await SendMessage(server, $"#{time} [{Username}] : {message}");
        }

        private async Task SendMessage(Socket server, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await server.SendAsync(bytes, SocketFlags.None);
        }
    }
}
