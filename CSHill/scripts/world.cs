using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace scripts.world
{
    class LoadBRK
    {
        public LoadBRK(string mapName)
        {
            string[] lines = File.ReadAllLines($"./maps/{mapName}.brk");
            var totalLines = -1;
            //var currentBrick = -1;
            foreach (string line in lines)
            {
                totalLines++;
                switch (totalLines)
                {
                    case 0:
                        if (line != "B R I C K  W O R K S H O P  V0.2.0.0") return;
                        continue;
                    case 3:
                        {
                            var glColor = line.Split(' ');
                            Game.Environment.ambient = new Color(double.Parse(glColor[0]), double.Parse(glColor[1]), double.Parse(glColor[2]));
                            continue;
                        }
                    case 4:
                        {
                            var glColor = line.Split(' ');
                            Game.Environment.baseColor = new Color(double.Parse(glColor[0]), double.Parse(glColor[1]), double.Parse(glColor[2]));
                            continue;
                        }
                    case 5:
                        {
                            var glColor = line.Split(' ');
                            Game.Environment.skyColor = new Color(double.Parse(glColor[0]), double.Parse(glColor[1]), double.Parse(glColor[2]));
                            continue;
                        }
                    case 6:
                        {
                            Game.Environment.baseSize = int.Parse(line);
                            continue;
                        }
                    case 7:
                        {
                            Game.Environment.sunIntensity = int.Parse(line);
                            continue;
                        }
                }
                string[] DATA = line.Split(' ');
                var ATTRIBUTE = DATA[0].Replace("+", "");
                var VALUE = DATA.Skip(1).ToArray();
                Console.WriteLine("{0} {1} {2}", ATTRIBUTE, VALUE);
                switch (ATTRIBUTE)
                {
                    default:
                        break;
                }
            }
        }
    }
}
