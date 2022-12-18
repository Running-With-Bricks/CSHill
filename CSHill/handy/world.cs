using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace scripts.world
{
    class LoadBRK
    {
        public LoadBRK(string mapName)
        {
            List<Brick> bricks = new();
            string[] lines = File.ReadAllLines(mapName);
            var totalLines = 0;
            var currentBrick = -1;
            foreach (string line in lines)
            {
                totalLines++;
                switch (totalLines)
                {
                    case 1:
                        {
                            if (line != "B R I C K  W O R K S H O P  V0.2.0.0") return;
                            continue;
                        }
                    case 3:
                        {
                            var glColor = line.Split(' ');
                            Game.Environment.ambient = new Color(double.Parse(glColor[0], CultureInfo.InvariantCulture), double.Parse(glColor[1], CultureInfo.InvariantCulture), double.Parse(glColor[2], CultureInfo.InvariantCulture));
                            continue;
                        }
                    case 4:
                        {
                            var glColor = line.Split(' ');
                            Game.Environment.baseColor = new Color(double.Parse(glColor[0], CultureInfo.InvariantCulture), double.Parse(glColor[1], CultureInfo.InvariantCulture), double.Parse(glColor[2], CultureInfo.InvariantCulture));
                            continue;
                        }
                    case 5:
                        {
                            var glColor = line.Split(' ');
                            Game.Environment.skyColor = new Color(double.Parse(glColor[0], CultureInfo.InvariantCulture), double.Parse(glColor[1], CultureInfo.InvariantCulture), double.Parse(glColor[2], CultureInfo.InvariantCulture));
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
                var VALUE = string.Join(" ",DATA.Skip(1).ToArray());
                //Console.WriteLine("{0}", ATTRIBUTE, VALUE);
                switch (ATTRIBUTE)
                {
                    case "NAME":
                        {
                            bricks[currentBrick].Name = VALUE;
                            continue;
                        }
                    case "ROT":
                        {
                            bricks[currentBrick].Rotation = int.Parse(VALUE);
                            continue;
                        }
                    case "SHAPE":
                        {
                            bricks[currentBrick].Shape = VALUE;
                            continue;
                        }
                    case "MODEL":
                        {
                            bricks[currentBrick].Model = int.Parse(VALUE);
                            continue;
                        }
                    case "NOCOLLISION":
                        {
                            bricks[currentBrick].Collision = false;
                            continue;
                        }
                    //case "COLOR":
                    //    {
                    //        var colors = VALUE.split(" ");
                    //        var color = convertRGB(colors[0], colors[1], colors[2]);
                    //        var team = new Team(teams[teams.length - 1], rgbToHex(color[0], color[1], color[2]));
                    //        teams[teams.length - 1] = team;
                    //        continue;
                    //    }
                    case "LIGHT":
                        {
                            var colors = VALUE.Split(' ');
                            var lightRange = colors[3];
                            bricks[currentBrick].lightEnabled = true;
                            bricks[currentBrick].lightRange = int.Parse(lightRange);
                            bricks[currentBrick].lightColor = new Color(double.Parse(colors[0], CultureInfo.InvariantCulture), double.Parse(colors[1], CultureInfo.InvariantCulture), double.Parse(colors[2], CultureInfo.InvariantCulture)); ;
                            continue;
                        }
                    case "SCRIPT":
                        {
                            continue;
                        }
                }
                
                if (DATA.Length == 10)
                {

                    //foreach (var item in DATA)
                    //{
                    //    Console.Write(item+" ");
                    //}
                    //Console.WriteLine();
                    Brick newBrick = new(
                        new Vector3(float.Parse(DATA[0], CultureInfo.InvariantCulture), float.Parse(DATA[1], CultureInfo.InvariantCulture), float.Parse(DATA[2], CultureInfo.InvariantCulture)),
                        new Vector3(float.Parse(DATA[3], CultureInfo.InvariantCulture), float.Parse(DATA[4], CultureInfo.InvariantCulture), float.Parse(DATA[5], CultureInfo.InvariantCulture)),
                        new Color(float.Parse(DATA[6], CultureInfo.InvariantCulture), float.Parse(DATA[7], CultureInfo.InvariantCulture), float.Parse(DATA[8], CultureInfo.InvariantCulture)));

                    newBrick.Visibility = double.Parse(DATA[9], CultureInfo.InvariantCulture);
                    
                    bricks.Add(newBrick);
                    currentBrick++;
                }
            }

            Game.Bricks = bricks;

        }
    }
    public class SendBRK
    {
        public SendBRK(string IpPort)
        {
            PacketBuilder pack = new PacketBuilder(17)
                .u32((uint)(Game.Bricks.Count));
            foreach (var brick in Game.Bricks)
            {

                if (Game.Debug.senbrkins)
                {
                    Console.WriteLine("{0} {1} {2}", brick.Position.x, brick.Position.y, brick.Position.z);
                    Console.WriteLine("{0} {1} {2}", brick.Scale.x, brick.Scale.y, brick.Scale.z);
                    Console.WriteLine((uint)(brick.Color.dec()));
                    Console.WriteLine((float)(brick.Visibility));
                }

                pack
                    .u32(brick.NetId)

                    .Float(brick.Position.x)
                    .Float(brick.Position.y)
                    .Float(brick.Position.z)

                    .Float(brick.Scale.x)
                    .Float(brick.Scale.y)
                    .Float(brick.Scale.z)

                    .u32((uint)(brick.Color.dec()))
                    .Float((float)(brick.Visibility));
                var attribute = "";
                //add attribute stuff, too lazy rn
                pack.String(attribute);
                foreach (var item in pack.Data)
                {
                    Console.Write(item+ " ");
                }
                Console.WriteLine();   
            }
            pack.send(IpPort);
        }
    }
}
