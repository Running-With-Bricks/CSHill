using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public static class Game
{
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
    public static class Config
    {
        public static string mapName;
        public static string mapDirectory;
        public static string hostKey;
        public static string local;
    }

    public static void MessageAll(string message)
    {
        new PacketBuilder(6)
            .String(message)
            .broadcast();
    }

}
