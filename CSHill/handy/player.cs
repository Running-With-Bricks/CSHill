using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scripts
{
    public class player
    {
        public static void SendEnv(string IpPort)
        {
            new PacketBuilder(7)
                .String("Ambient")
                .u32((uint)Game.Environment.ambient.dec())
                .send(IpPort);

            new PacketBuilder(7)
                .String("Sky")
                .u32((uint)Game.Environment.skyColor.dec())
                .send(IpPort);

            new PacketBuilder(7)
                 .String("BaseCol")
                 .u32((uint)Game.Environment.baseColor.dec())
                 .send(IpPort);

            new PacketBuilder(7)
                 .String("BaseSize")
                 .u32((uint)Game.Environment.baseSize)
                 .send(IpPort);

            new PacketBuilder(7)
                 .String("Sun")
                 .u32((uint)Game.Environment.sunIntensity)
                 .send(IpPort);

        }

        public static void SendPlayers(string IpPort)
        {
            PacketBuilder pack = new PacketBuilder(3)
                .u8((uint)Game.Players.Count -1 );

            foreach (var player in Game.Players)
            {
                if (player.IpPort != IpPort)
                {
                    pack
                        .u32(player.NetId)
                        .String(player.Name)
                        .u32(player.userId)
                        .Bool(player.admin)
                        .u8(player.membership);
                }
            }
            pack.send(IpPort);
            //send Position and other stuff
            foreach (var player in Game.Players)
            {
                createPlayerIds(player, "ALL")
                    .deflate()
                    .send(IpPort);
            }

        }

        public static PacketBuilder createPlayerIds(Player player,string ids = "")
        {
            if (ids == "ALL") ids = "ABCFGHIKLMNOPXYf";
            if (player.Team == null) ids = ids.Replace("Y", "");
            var length = ids.Length;
            PacketBuilder pack = new PacketBuilder(4)
                .String(ids)
                .u32(player.NetId);
            for (int i = 0; i < length; i++)
            {
                var ID = ids[i].ToString();
                switch (ID)
                {
                    case "A":
                        {
                            pack.Float(player.Position.x);
                            break;
                        }
                    case "B":
                        {
                            pack.Float(player.Position.y);
                            break;
                        }
                    case "C":
                        {
                            pack.Float(player.Position.z);
                            break;
                        }
                    case "F":
                        {
                            pack.Float(player.Rotation);
                            break;
                        }
                    case "G":
                        {
                            pack.Float(player.Scale.x);
                            break;
                        }
                    case "H":
                        {
                            pack.Float(player.Scale.y);
                            break;
                        }
                    case "I":
                        {
                            pack.Float(player.Scale.z);
                            break;
                        }
                    //case "J":
                    //    {
                    //        pack.u32(player.tool._slotId);
                    //        break;
                    //    }
                    case "K":
                        {
                            pack.u32(player.Colors.Head.dec());
                            break;
                        }
                    case "L":
                        {
                            pack.u32(player.Colors.Torso.dec());
                            break;
                        }
                    case "M":
                        {
                            pack.u32(player.Colors.LeftArm.dec());
                            break;
                        }
                    case "N":
                        {
                            pack.u32(player.Colors.RightArm.dec());
                            break;
                        }
                    case "O":
                        {
                            pack.u32(player.Colors.LeftLeg.dec());
                            break;
                        }
                    case "P":
                        {
                            pack.u32(player.Colors.RightLeg.dec());
                            break;
                        }
                    case "X":
                        {
                            pack.i32(player.Score);
                            break;
                        }
                    case "Y":
                        {
                            pack.u32(player.Team.NetId);
                            break;
                        }
                    case "1":
                        {
                            pack.u32(player.Speed);
                            break;
                        }
                    case "2":
                        {
                            pack.u32(player.JumpPower);
                            break;
                        }
                    case "3":
                        {
                            pack.u32(player.CameraFOV);
                            break;
                        }
                    case "4":
                        {
                            pack.i32(player.CameraDistance);
                            break;
                        }
                    case "5":
                        {
                            pack.Float(player.CameraPosition.x);
                            break;
                        }
                    case "6":
                        {
                            pack.Float(player.CameraPosition.y);
                            break;
                        }
                    case "7":
                        {
                            pack.Float(player.CameraPosition.z);
                            break;
                        }
                    case "8":
                        {
                            pack.Float(player.CameraRotation.x);
                            break;
                        }
                    case "9":
                        {
                            pack.Float(player.CameraRotation.y);
                            break;
                        }
                    case "a":
                        {
                            pack.Float(player.CameraRotation.z);
                            break;
                        }
                    case "b":
                        {
                            pack.String(player.CameraType);
                            break;
                        }
                    case "c":
                        {
                            pack.Float(player.CameraObject.NetId);
                            break;
                        }
                    case "e":
                        {
                            pack.Float(player.Health);
                            break;
                        }
                    case "f":
                        {
                            pack.String(Color.formatHex(player.Speech));
                            break;
                        }
                    case "h":
                        { // Set items + arm to -1
                            break;
                        }
                }
            }
            return pack;

        }
    }
    
}
