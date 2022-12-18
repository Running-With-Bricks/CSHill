using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Net.Sockets;
using System.Text.RegularExpressions;

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
                var ATTRIBUTE = DATA[0].Replace("\t+", "");
                var VALUE = string.Join(" ",DATA.Skip(1).ToArray());
                //Console.WriteLine("{0}", ATTRIBUTE);
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
                            bricks[currentBrick].LightEnabled = true;
                            bricks[currentBrick].LightRange = int.Parse(lightRange);
                            bricks[currentBrick].LightColor = new Color(double.Parse(colors[0], CultureInfo.InvariantCulture), double.Parse(colors[1], CultureInfo.InvariantCulture), double.Parse(colors[2], CultureInfo.InvariantCulture)); ;
                            continue;
                        }
                    case "SCRIPT":
                        {
                            continue;
                        }
                }
                
                if (DATA.Length == 10)
                {
                    try
                    {
                        Brick newBrick = new(
                            new Vector3(float.Parse(DATA[0], CultureInfo.InvariantCulture), float.Parse(DATA[1], CultureInfo.InvariantCulture), float.Parse(DATA[2], CultureInfo.InvariantCulture)),
                            new Vector3(float.Parse(DATA[3], CultureInfo.InvariantCulture), float.Parse(DATA[4], CultureInfo.InvariantCulture), float.Parse(DATA[5], CultureInfo.InvariantCulture)),
                            new Color(float.Parse(DATA[6], CultureInfo.InvariantCulture), float.Parse(DATA[7], CultureInfo.InvariantCulture), float.Parse(DATA[8], CultureInfo.InvariantCulture)));

                        newBrick.Visibility = double.Parse(DATA[9], CultureInfo.InvariantCulture);

                        bricks.Add(newBrick);
                        currentBrick++;
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            Game.Bricks = bricks;
            Console.WriteLine("Loaded map {0}", Game.Config.mapDirectory + Game.Config.mapName);

        }
    }
    public class SendBRK
    {
        public SendBRK(string IpPort, int NetId)
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
                var attributes = "";
                if (brick.Rotation != 0)
                    attributes += "A";
                if (brick.Shape != null)
                    attributes += "B";
                if (brick.LightEnabled)
                    attributes += "D";
                if (!brick.Collision)
                    attributes += "F";
                if (brick.Clickable)
                    attributes += "G";
                pack.String(attributes);
                for (var i = 0; i < attributes.Length; i++)
                {
                    var ID = attributes[i];
                    switch (ID)
                    {
                        case 'A':
                            pack.i32(brick.Rotation);
                            break;
                        case 'B':
                            
                            pack.String(brick.Shape);
                            break;
                        case 'D':
                            pack.u32((uint)brick.LightColor.dec());
                            pack.u32((uint)brick.LightRange);
                            break;
                        case 'G':
                            pack.Bool(brick.Clickable);
                            pack.u32(brick.ClickDistance);
                            break;
                    }
                }
            }
            pack
                .deflate()
                .send(IpPort);
            {
                string attributes = "bci";
                new PacketBuilder(4)
                    .String(attributes)
                    .u32((uint)NetId)
                    .String("orbit")
                    .u32((uint)NetId)
                    .u8(1)
                    .send(IpPort);
            }
        }
    }
}

