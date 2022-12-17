using System;
using System.Text;
using SuperSimpleTcp;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

class Server
{
    public static SimpleTcpServer server = new("0.0.0.0",42480);
    static void Main()
    {

        server.Start();

        //foreach (var file in Directory.GetFiles("./"))
        //{
        //    Console.WriteLine(file);
        //}

        server.Events.ClientConnected += (sender, e) =>
        {
            Console.WriteLine($"Client ({e.IpPort}) connected!");
            Game.Players.Add(new Player(e.IpPort));
        };

        server.Events.ClientDisconnected += (sender, e) =>
        {
            Game.Players.Remove(Game.Players.Find(p => p.IpPort == e.IpPort));
            Console.WriteLine($"Client ({e.IpPort}) disconnected!");
        };

        server.Events.DataReceived += (sender, e) =>
        {
            var plyr = Game.Players.Find(p => p.IpPort == e.IpPort);
            plyr.HandleBytes(e.Data.ToArray());

        };

        while (true)
        {

        }

    }
}



