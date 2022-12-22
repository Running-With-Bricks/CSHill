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
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using System.ComponentModel;

class Server
{
    public static SimpleTcpServer server;
    public static V8ScriptEngine engine;
    static void Main()
    {
        XmlDocument doc = new();
        doc.Load("./config.xml");
        Config.mapName = doc.GetElementsByTagName("mapName")[0].InnerText;
        Config.mapDirectory = doc.GetElementsByTagName("mapDirectory")[0].InnerText;
        Config.hostKey = doc.GetElementsByTagName("hostKey")[0].InnerText;
        Config.local = bool.Parse(doc.GetElementsByTagName("local")[0].InnerText);
        Config.port = int.Parse(doc.GetElementsByTagName("port")[0].InnerText);
        Config.scriptsDirectory = doc.GetElementsByTagName("scriptsDirectory")[0].InnerText;
        Config.enableJS = bool.Parse(doc.GetElementsByTagName("enableJS")[0].InnerText);


        server = new(Config.local ? "127.0.0.1" : "0.0.0.0", Config.port);
        server.Start();
        Console.WriteLine("Listening for connections on port {0}", Config.port);


        try
        {
            new scripts.world.LoadBRK(Config.mapDirectory + Config.mapName);
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

        if (!Config.local)
        {
            Console.WriteLine("Attempting to post...");
            new SetInterval().New(api.postServer, 60000);
        }
        if (Config.enableJS)
        {

            engine = new();
            new Thread(() =>
            {
                try
                {
                    Type[] addToEngineTypes = { typeof(AssetDownloader), typeof(Bot), typeof(Brick), typeof(Color), typeof(Game), typeof(Outfit),
                        typeof(PacketBuilder), typeof(Player),typeof(Team),typeof(Tool),typeof(Vector3),typeof(Console),typeof(console)};
                    engine.AddHostTypes(addToEngineTypes);

                    engine.AddHostType(typeof(ListExtensions));

                    Action<ScriptObject, int> setTimeout = (func, delay) =>
                    {
                        var timer = new Timer(_ => func.Invoke(false));
                        timer.Change(delay, Timeout.Infinite);
                    };
                    engine.Script._setTimeout = setTimeout;
                    engine.Execute(@"
                        function setTimeout(func, delay) {
                            let args = Array.prototype.slice.call(arguments, 2);
                            _setTimeout(func.bind(undefined, ...args), delay || 0);
                        }
                        var sleep = ms => new Promise(r => setTimeout(r, ms));
                        
                    ");
                    Console.WriteLine("V8 initialized");
                    engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;

                    string[] Files = Directory.GetFiles(Config.scriptsDirectory, "*.js");
                    foreach (var File in Files)
                    {
                        engine.ExecuteDocument(File);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }).Start();
        }
        while (true)
        {

        }
    }
    public static class Config
    {
        public static string mapName;
        public static string mapDirectory;
        public static string hostKey;
        public static bool local;
        public static int port;
        public static bool enableJS;
        public static string scriptsDirectory;
    }

}
public static class ListExtensions
{
    public static T Find<T>(this List<T> list, dynamic predicate)
    {
        return list.Find(item => predicate(item));
    }
    public static int FindIndex<T>(this List<T> list, dynamic predicate)
    {
        return list.FindIndex(item => predicate(item));
    }
    public static List<T> Filter<T>(this List<T> list, dynamic predicate)
    {
        return list.Where(item => predicate(item)).ToList();
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
        if (Destroyed) return;
        New(action, delay);
    }
}

public class console
{
    public static void log(dynamic input)
    {
        Console.WriteLine(input);
    }
    public static void type(double input)
    {
        Console.WriteLine("ok");
    }
}


