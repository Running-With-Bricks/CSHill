using System;
using System.Text;
using SuperSimpleTcp;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
class Server
{
    public static SimpleTcpServer server = new("0.0.0.0", 42480);
    static void Main()
    {

        server.Start();

        XmlDocument doc = new();
        doc.Load("./config.xml");
        Game.Config.mapName = doc.GetElementsByTagName("mapName")[0].InnerText;
        Game.Config.mapDirectory = doc.GetElementsByTagName("mapDirectory")[0].InnerText;
        Game.Config.hostKey = doc.GetElementsByTagName("hostKey")[0].InnerText;

        try
        {
            new scripts.world.LoadBRK(Game.Config.mapDirectory + Game.Config.mapName);
        }
        catch (Exception error)
        {
            Console.WriteLine(error);
        }
        

        

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



