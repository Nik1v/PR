using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class ServerSocket
    {
        private readonly Socket _serverSocket;
        private readonly IPEndPoint _serverEndpoint;
        private readonly List<Socket> _listClients = new List<Socket>();

        public ServerSocket(string ip, int port)
        {
            var ipAdress = IPAddress.Parse(ip);

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverEndpoint = new IPEndPoint(ipAdress, port);
        }

        public void BindAndListen(int limit)
        {
            try
            {
                _serverSocket.Bind(_serverEndpoint);
                _serverSocket.Listen(limit);

                Console.WriteLine("Serverul asculta pe " + _serverEndpoint);

                Thread accept = new Thread(AcceptAndReceive);
                accept.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare de server {e.Message}");
            }
        }

        private void AcceptAndReceive()
        {
            while (true)
            {
                try
                {
                    Socket client = _serverSocket.Accept();

                    lock (_listClients)
                    {
                        _listClients.Add(client);
                    }

                    Console.WriteLine($"Clientul {client.RemoteEndPoint} a fost acceptat");

                    Thread receiveT = new Thread(() => receive(client));
                    receiveT.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Eroare de acceptare {e.Message}");
                }
            }
        }

        private void receive(Socket client)
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int byteReceived = client.Receive(buffer);
                    string message = Encoding.UTF8.GetString(buffer, 0, byteReceived);

                    Console.WriteLine($"{client.RemoteEndPoint} : {message}");

                    lock (_listClients)
                    {
                        foreach (Socket c in _listClients)
                        {
                            if (c != client)
                            {
                                c.Send(buffer);
                            }
                        }
                    }
                }
                catch
                {
                    lock (_listClients)
                    {
                        _listClients.Remove(client);
                    }

                    Console.WriteLine($"Clientul {client.RemoteEndPoint} a parasit chat-ul");
                    return;
                }
            }
        }
    }
}
