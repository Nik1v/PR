using Server;

ServerSocket server = new ServerSocket("127.0.0.1", 5050);

server.BindAndListen(10);

Console.ReadLine();