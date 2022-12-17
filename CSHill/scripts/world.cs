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
                }
            }
        }
    }
}
