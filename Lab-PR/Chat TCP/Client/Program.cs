using Client;

ClientSocket client = new ClientSocket();

Thread clientT = new Thread(() =>
{
    client.Connect("127.0.0.1", 5050);

    Thread send = new Thread(client.Send);
    Thread receive = new Thread(client.Receive);

    send.Start();
    receive.Start();
});
clientT.Start();