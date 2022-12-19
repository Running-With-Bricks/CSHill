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
using System.Threading;

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
        Game.Config.port = int.Parse(doc.GetElementsByTagName("port")[0].InnerText);


        server = new("0.0.0.0", Game.Config.port);
        server.Start();
        Console.WriteLine("Listening for connections on port {0}", Game.Config.port);


        try
        {
            new scripts.world.LoadBRK(Game.Config.mapDirectory + Game.Config.mapName);
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR WHILE LOADING THE BRK:\n" + e);
        }


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

        if (Game.Config.local.ToLower() == "false")
            new SetInterval().New(api.postServer, 30000);

        while (true)
        {

        }
    }
}
public class SetInterval
{
    public int Delay;
    public Action Action;
    public bool Destroyed = false;
    public async void New(Action action, int delay)
    {
        action();
        await Task.Delay(delay);
        New(action, delay);
    }
}


