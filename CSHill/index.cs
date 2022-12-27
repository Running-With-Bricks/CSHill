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
using System.Net.Http;
using Newtonsoft.Json;

class Server
{
    public static SimpleTcpServer server;
    public static V8ScriptEngine engine;
    public static HttpClient client = new();
    public static bool keepAlive = true;
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
            plyr = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

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

                    dynamic setup = engine.Evaluate(@"(impl => {
                        let queue = [], nextId = 0;
                        const maxId = 1000000000000, getNextId = () => nextId = (nextId % maxId) + 1;
                        const add = entry => {
                            const index = queue.findIndex(element => element.due > entry.due);
                            index >= 0 ? queue.splice(index, 0, entry) : queue.push(entry);
                        }
                        function set(periodic, func, delay) {
                            const id = getNextId(), now = Date.now(), args = [...arguments].slice(3);
                            add({ id, periodic, func: () => func(...args), delay, due: now + delay });
                            impl.Schedule(queue.length > 0 ? queue[0].due - now : -1);
                            return id;
                        };
                        function clear(id) {
                            queue = queue.filter(entry => entry.id != id);
                            impl.Schedule(queue.length > 0 ? queue[0].due - Date.now() : -1);
                        };
                        globalThis.setTimeout = set.bind(undefined, false);
                        globalThis.setInterval = set.bind(undefined, true);
                        globalThis.clearTimeout = globalThis.clearInterval = clear.bind();
                        impl.Initialize(() => {
                            const now = Date.now();
                            while ((queue.length > 0) && (now >= queue[0].due)) {
                                const entry = queue.shift();
                                if (entry.periodic) add({ ...entry, due: now + entry.delay });
                                entry.func();
                            }
                            return queue.length > 0 ? queue[0].due - now : -1;
                        });
                    })");
                    setup(new TimerImpl());
                    Console.WriteLine("V8 JavaScript engine initialized");
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
        while (keepAlive)
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
    public static void ForEach<T>(this List<T> list, dynamic function)
    {
        foreach (var item in list)
        {
            function(item);
        }
    }
}

public class SetInterval //pee pee poo poo shitstain 
{
    public int Delay;
    public Action Action;
    public async void New(Action action, int delay)
    {
        action();
        await Task.Delay(delay);
        New(action, delay);
    }
}

public sealed class TimerImpl
{
    private Timer _timer;
    private Func<double> _callback = () => Timeout.Infinite;
    public void Initialize(dynamic callback) => _callback = () => (double)callback();
    public void Schedule(double delay)
    {
        if (delay < 0)
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
        else
        {
            if (_timer == null) _timer = new Timer(_ => Schedule(_callback()));
            _timer.Change(TimeSpan.FromMilliseconds(delay), Timeout.InfiniteTimeSpan);
        }
    }
}

public class console
{
    public static void log(dynamic input = null)
    {
        Console.WriteLine(JsonConvert.SerializeObject(input));
    }
}


