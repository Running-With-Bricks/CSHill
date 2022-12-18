using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Ionic.Zlib;
using System.IO;
using System.Threading;

public class Player
{
    public int NetId;
    public static int _NetId = 0; //global netid for all players
    public string Token;

    public Vector3 Position;
    public Vector3 Scale;
    public float Rotation;
    public float CamRotation;

    public string Name;
    public int userId;
    public bool admin;
    public int membership;

    public Player(string _IpPort)
    {
        IpPort = _IpPort;
        _NetId++;
        NetId = _NetId;
    }

    public void Message(string message)
    {
        new PacketBuilder(6)
            .String(message)
            .send(IpPort);
    }
    public void Kick(string reason)
    {
        new PacketBuilder(7)
            .String("kick")
            .String(reason)
            .send(IpPort);
        Server.server.DisconnectClient(IpPort);
    }

    //NETWORKING STUFF

    public string IpPort;
    private byte[] CurrentBytes = { };
    public void HandleBytes(byte[] newBytes)
    {

        CurrentBytes = CurrentBytes.Concat(newBytes).ToArray();//add new bytes
        var (messageSize, end) = UIntV.ReadUIntV(CurrentBytes);//get uintv stuff
        if (messageSize + end > CurrentBytes.Length) return; //return if there arent enough bytes

        if (Game.Debug.PacketInspector) Console.WriteLine("\nmessageSize: {0} | end: {1} | netId: {2} | remain: {3}", messageSize, end, NetId, CurrentBytes.Length);//debug stuff

        byte[] workBytes = new byte[messageSize]; //make new array with the size to fit the bytes we need
        Array.Copy(CurrentBytes, end, workBytes, 0, messageSize + end - 1); //add the bytes we need
        CurrentBytes = CurrentBytes.Skip(messageSize + end).ToArray(); //remove the bytes we just took from the current bytes array

        if (workBytes[0] == 120)//check for compression
            workBytes = ZlibStream.UncompressBuffer(workBytes);

        if (Game.Debug.PacketInspector)
        {
            foreach (var data in workBytes)
                Console.Write(data.ToString() + " ");
            Console.WriteLine();
        }

        if (CurrentBytes.Length > 0) HandleBytes(new byte[0]);

        //now do stuff here with workBytes
        PacketHandler packet = new(workBytes);
        switch (packet.u8())
        {
            case 1://authenitication
                {
                    if (Token != null) return;
                    Token = packet.String();
                    //Console.WriteLine(packet.String());//version
                    try
                    {
                        var data = api.checkAuth(Token);
                        Name = data.Name;
                        userId = data.userId;
                        admin = data.admin;
                        membership = data.membership;
                    }
                    catch (Exception e)
                    {
                        Kick(e.Message);
                    }



                    Message(Game.MOTD);
                    Game.MessageAll($"<color:FF7A00>[SERVER] : \\c0 {Name} connected to the server.");

                    new PacketBuilder(1)
                           .u32((uint)NetId)   //netid
                           .u32((uint)(Game.Bricks.Count))//brickcount
                           .u32((uint)userId)    //userid
                           .String(Name)      //name
                           .Bool(admin)               //admin
                           .u8((uint)membership)               //membership
                           .u32(1)              //gameid
                           .String("CSHill Test")//gamename
                           .send(IpPort);

                    new scripts.player.SendEnv(IpPort);

                    new Thread(() => new scripts.world.SendBRK(IpPort, NetId)).Start();

                    break;
                }
            case 2://position
                {

                    break;
                }
            case 3://commands and chat
                {
                    if (packet.String() != "chat") return;
                    new PacketBuilder(6)
                        .String(packet.String())
                        .send(IpPort);

                    break;
                }
            default:
                break;
        }


    }

}
