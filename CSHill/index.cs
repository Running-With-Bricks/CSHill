using System;
using System.Text;
using SimpleTCP;
using System.IO;
using System.Linq;
using System.Net.Sockets;
class Server
{
    static void Main()
    {
        var server = new SimpleTcpServer();

        server.ClientConnected += (sender, e) =>
        {
            Console.WriteLine(e);
            Console.WriteLine($"Client ({e.Client.RemoteEndPoint}) connected!");
            Game.Players.Add(new Player(e));
            //var plyr = Game.Players.Find(p => p.NetId == Player._NetId);
            Console.WriteLine(Game.Players.Count);
        };

        server.ClientDisconnected += (sender, e) =>
        {
            Game.Players.Remove(Game.Players.Find(p => p.Socket == e));
            Console.WriteLine($"Client ({e.Client.RemoteEndPoint}) disconnected!");
        };

        server.DataReceived += (sender, e) =>
        {
            var plyr = Game.Players.Find(p => p.Socket == e.TcpClient);
            plyr.HandleBytes(e.Data);

        };

        server.Start(42480);

        while (true)
        {

        }
    }
}



