using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Game
{
    public static List<Player> Players = new List<Player>();
    public static List<Brick> Bricks = new List<Brick>();
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
    }
}
