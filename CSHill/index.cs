using System;
using System.Text;
using SimpleTCP;
using System.IO;
using System.Linq;

class Server
{
    static void Main() {
        var server = new SimpleTcpServer();

        server.ClientConnected += (sender, e) =>
        {
            Console.WriteLine($"Client ({e.Client.RemoteEndPoint}) connected!");
            Game.Players.Add(new Player(e));
            var plyr = Game.Players.Find(p => p.NetId == Player._NetId);
            Console.WriteLine(plyr.NetId);
            
        };

        server.ClientDisconnected += (sender, e) => Console.WriteLine($"Client ({e.Client.RemoteEndPoint}) disconnected!");

        server.DataReceived += (sender, e) =>
        {
            foreach(var data in e.Data)
                Console.Write(data.ToString()+" ");
            Console.WriteLine();
        };

        server.Start(42480);

        while (true)
        {

        }
    } 
}



