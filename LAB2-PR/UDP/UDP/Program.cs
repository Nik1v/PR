using UDP;

string multicastIP = "239.3.5.6";
int port = 5000;

var chat = new UDPChat(multicastIP, port);

chat.StartReceiveLoop();

Console.WriteLine("Format =>  <IP>:<TEXT>"); // 192.168.1.201:SALUT, scriu în privat
Console.WriteLine("0: General"); // 0:salutare, scriu în canalul general

while(true)
{
    var input = Console.ReadLine() ?? "";
    var splitted = input.Split(':');
    var ip = splitted[0];
    var text = splitted[1];

    if (ip == "0")
    {
        chat.SendGeneral(text);
    }
    else
    {
        chat.SendTo(ip, text);
    }
}