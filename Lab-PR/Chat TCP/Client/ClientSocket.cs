using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class ClientSocket
    {
        private readonly Socket _clientSocket;

        public ClientSocket()
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ip, int port)
        {
            var ipAddress = IPAddress.Parse(ip);
            var endPoint = new IPEndPoint(ipAddress, port);

            try
            {
                _clientSocket.Connect(endPoint);
                Console.WriteLine($"Conectat la server {endPoint}");
            }
            catch(Exception e)
            {
                Console.WriteLine($"Eroare de conectare {e.Message}");
            }
        }

        public void Send()
        {
            while (true)
            {
                try
                {
                    Console.Write("Scrie mesajul: ");

                    string message = Console.ReadLine() ?? "";

                    byte[] buffer = Encoding.UTF8.GetBytes(message);

                    _clientSocket.Send(buffer);

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Eroare mesaj {e.Message}");
                }
            }
        }

        public void Receive()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int byteReceived = _clientSocket.Receive(buffer);
                    string message = Encoding.UTF8.GetString(buffer, 0, byteReceived);

                    if (!string.IsNullOrEmpty(message))
                    {
                        Console.Write($"\nMesaj: {message} \nScrie mesajul: ");
                    }
                }
                catch
                {
                    Console.WriteLine("\nServerul a inchis conexiunea");
                    return;
                }
            }
        }
    }
}
