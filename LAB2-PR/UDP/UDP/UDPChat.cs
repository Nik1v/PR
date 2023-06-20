using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDP
{
    internal class UDPChat
    {
        private string _multicastIP;
        private int _port;
        private Socket _multicastSocket;
        private Socket _senderSocket;

        public UDPChat(string multicastIP, int port)
        {
               _multicastIP = multicastIP;
               _port = port;

               try
               {
                    _multicastSocket = new Socket(AddressFamily.InterNetwork,
                         SocketType.Dgram, ProtocolType.Udp);

                    var hostIP = new IPEndPoint(IPAddress.Parse("192.168.100.78"), port); // adresa IP de la rețeaua locală
                    _multicastSocket.Bind(hostIP);

                    _multicastSocket.SetSocketOption(SocketOptionLevel.IP,
                         SocketOptionName.AddMembership,
                         new MulticastOption(IPAddress.Parse(_multicastIP), IPAddress.Parse("192.168.100.78"))); // adresa IP de la rețeaua locală

                    _senderSocket = new Socket(AddressFamily.InterNetwork,
                         SocketType.Dgram, ProtocolType.Udp);

                    _senderSocket.SetSocketOption(SocketOptionLevel.IP,
                         SocketOptionName.AddMembership,
                         new MulticastOption(IPAddress.Parse(_multicastIP), IPAddress.Any));
               }
               catch (Exception e)
               {
                    Console.WriteLine($"Error occurred while setting up the sockets: {e.Message}");
               }
        }

        public void StartReceiveLoop()
        {
            Task.Run(() => receive());
        }

        private void receive()
        {
               try
               {
                    while (true)
                    {
                         byte[] buffer = new byte[1024];
                         EndPoint remoteEndpoint = new IPEndPoint(0, 0);

                         _multicastSocket.ReceiveFrom(buffer, ref remoteEndpoint);
                         string text = Encoding.UTF8.GetString(buffer);

                         Console.WriteLine($"{remoteEndpoint}: {text}");
                    }
               }
               catch(Exception e)
               {
                    Console.WriteLine($"Error occurred while receiving data: {e.Message}");
               }
        }

        public void SendTo(string ip, string text)
        {
               try
               {
                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    var remoteEndpoint = new IPEndPoint(IPAddress.Parse(ip), _port);

                    _senderSocket.SendTo(bytes, remoteEndpoint);
               }
               catch(Exception e) 
               {
                    Console.WriteLine($"Error occurred while sending data: {e.Message}");
               } 
        }

        public void SendGeneral(string text)
        {
               try
               {
                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    var remoteEndpoint = new IPEndPoint(IPAddress.Parse(_multicastIP), _port);

                    _senderSocket.SendTo(bytes, remoteEndpoint);
               }
               catch (Exception e)
               {
                    Console.WriteLine($"Error occurred while sending data: {e.Message}");
               }
          }
    }
}