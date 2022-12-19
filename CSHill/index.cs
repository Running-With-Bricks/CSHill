using System;
using System.Text;
using SuperSimpleTcp;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Collections.Generic;

class Server
{
    public static SimpleTcpServer server;

    static void Main()
    {
        XmlDocument doc = new();
        doc.Load("./config.xml");
        Game.Config.mapName = doc.GetElementsByTagName("mapName")[0].InnerText;
        Game.Config.mapDirectory = doc.GetElementsByTagName("mapDirectory")[0].InnerText;
        Game.Config.hostKey = doc.GetElementsByTagName("hostKey")[0].InnerText;
        Game.Config.local = doc.GetElementsByTagName("local")[0].InnerText;

        server = new("0.0.0.0", int.Parse(doc.GetElementsByTagName("port")[0].InnerText));
        server.Start();
        Console.WriteLine("Listening for connections on port {0}", doc.GetElementsByTagName("port")[0].InnerText);

        new scripts.world.LoadBRK(Game.Config.mapDirectory + Game.Config.mapName);

        server.Events.ClientConnected += (sender, e) =>
        {
            Console.WriteLine($"Client ({e.IpPort}) connected!");
            Game.Players.Add(new Player(e.IpPort));
        };

        server.Events.ClientDisconnected += (sender, e) =>
        {
            Console.WriteLine($"Client ({e.IpPort}) disconnected!");

            Player plyr = Game.Players.Find(p => p.IpPort == e.IpPort);

            plyr._RemovePlayer();

        };

        server.Events.DataReceived += (sender, e) =>
        {
            Game.Players.Find(p => p.IpPort == e.IpPort).HandleBytes(e.Data.ToArray());
        };

        while (true)
        {

        }

    }
}



