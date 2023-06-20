using DnsClient;
using System.Net;

Console.WriteLine("\n resolve <ip> \n resolve <domeniu> \n use dns <ip> \n ");

var dnsServer = new LookupClient(IPAddress.Parse("8.8.8.8"));

while (true)
{
    Console.Write($"{dnsServer.NameServers.FirstOrDefault()}> ");

    var command = Console.ReadLine();
    var tokens = command!.Split(' ');

    if (tokens[0] == "resolve")
    {
        if (tokens.Length < 2)
        {
            Console.WriteLine("Eroare: trebuie sa specificati un domeniu sau o adresa IP.");
            continue;
        }
        else
        {
            string query = tokens[1];

            try
            {
                if (IPAddress.TryParse(query, out _))
                {
                    string host = dnsServer.GetHostEntry(IPAddress.Parse(query)).HostName;

                    Console.WriteLine(host);
                }
                else
                {
                    IPAddress[] addresses = Dns.GetHostAddresses(query);

                    foreach (IPAddress address in addresses)
                    {
                        Console.WriteLine(address);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la rezolvarea DNS: {e.Message}");
            }
        }
    }
    else if (tokens[0] == "use" && tokens[1] == "dns")
    {
        if (tokens.Length < 3)
        {
            Console.WriteLine("Comanda invalida. Folositi: use dns <ip>");
            continue;
        }
        else
        {
            string newDns = tokens[2];

            try
            {
                Dns.GetHostEntry(newDns);
                dnsServer = new LookupClient(IPAddress.Parse(newDns));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la setarea DNS-ului: {e.Message}");
            }
        }
    }
    else
    {
        Console.WriteLine("Comanda necunoscuta.");
    }
}