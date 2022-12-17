using System;
using System.Text;
using SimpleTCP;
using System.IO;
using System.Linq;
using System.Net.Sockets;
class Server
{
    public static SimpleTcpServer server = new();
    static void Main()
    {

        server.ClientConnected += (sender, e) =>
        {
            Console.WriteLine($"Client ({e.Client.RemoteEndPoint}) connected! {e}");
            Game.Players.Add(new Player(e));
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



