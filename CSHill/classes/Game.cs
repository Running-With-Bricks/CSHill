using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public static class Game
{
    public class _setData
    {
        public class Data
        {

            [JsonProperty("id")]
            public uint id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("visits")]
            public string visits { get; set; }
        }
        [JsonProperty("data")]
        public Data data { get; set; }


    }
    public static _setData SetData = new();

    public static List<Player> Players = new();
    public static List<Brick> Bricks = new();
    public static List<Tool> Tools = new();
    public static List<Team> Teams = new();
    public static List<Bot> Bots = new();

    public static string MOTD = "This server is hosted using <color:E36600>CSHill<color:FFFFFF>!";

    public static class Debug
    {
        public static bool PacketInspector = false;
        public static bool senbrkins = false;
    }
    public static class Environment
    {
        public static Color ambient;
        public static Color baseColor;
        public static Color skyColor;
        public static int baseSize;
        public static int sunIntensity;
    }

    private static Dictionary<string, List<dynamic>> Events = new();

    public static void on(string Event, dynamic func)
    {
        if (!Events.ContainsKey(Event))
        {
            Events.Add(Event, new List<dynamic>());
        }
        Events[Event].Add(func);
    }
    public static void off(string Event, dynamic func)
    {
        if (!Events.ContainsKey(Event))
        {
            Events.Add(Event, new List<dynamic>());
        }
        Events[Event].Remove(func);
    }
    public static void emit(string Event, params dynamic[] args)
    {
        if (!Events.ContainsKey(Event))
        {
            return;
        }

        foreach (dynamic func in Events[Event])
        {
            func(args);
        }
    }

    public static void MessageAll(string message)
    {
        new PacketBuilder(6)
            .String(message)
            .broadcast();
    }

}
