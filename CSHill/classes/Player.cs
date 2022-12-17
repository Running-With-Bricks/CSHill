﻿using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Ionic.Zlib;
using System.IO;

public class Player
{
    public int NetId;
    public string Name;
    public static int _NetId = 0; //global netid for all players
    public string Token;
    public Vector3 Position;
    public Player(string _IpPort)
    {
        IpPort = _IpPort;
        _NetId++;
        NetId = _NetId;
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
        PacketHandler packet = new PacketHandler(workBytes);
        switch (packet.u8())
        {
            case 1://authenitication
                {
                    if (Token != null) return;
                    Token = packet.String();
                    Console.WriteLine(packet.String());//version

                    //check for auth here

                    new PacketBuilder(1)
                           .u32((uint)NetId)
                           .u32(1)
                           .u32((uint)NetId)
                           .String("josh")
                           .u8(0)
                           .u8(0)
                           .u32(1)
                           .String("CSHill Test")
                           .send(IpPort);

                    new PacketBuilder(17)

                        .u32(1)
                        .u32(1)

                        .Float(1)
                        .Float(1)
                        .Float(1)

                        .Float(4)
                        .Float(4)
                        .Float(4)

                        .u32(256)
                        .Float(1)
                        .send(IpPort);
                    break;
                }
            case 2:
                break;
            case 3:
                {
                    if (packet.String() != "chat") return;
                    PacketBuilder package = new PacketBuilder(6)
                        .String(packet.String())
                        .send(IpPort);

                    break;
                }
            default:
                break;
        }


    }

}
