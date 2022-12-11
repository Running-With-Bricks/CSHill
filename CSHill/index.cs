using System;
using System.Text;
using SimpleTCP;

class Server
{
    static void Main() {
        var server = new SimpleTcpServer();

        server.ClientConnected += (sender, e) =>
        {
            Console.WriteLine($"Client ({e.Client.RemoteEndPoint}) connected!");
            Console.WriteLine(e);
        };

        server.ClientDisconnected += (sender, e) => Console.WriteLine($"Client ({e.Client.RemoteEndPoint}) disconnected!");

        server.DataReceived += (sender, e) =>
        {
            foreach(var data in e.Data)
                Console.Write(data.ToString()+" ");
            Console.WriteLine();
            //Console.WriteLine(e.Data.ToString());
            //Console.WriteLine(e.TcpClient.Client);
        };

        server.Start(42480);

        while (true)
        {

        }
    } 
}




