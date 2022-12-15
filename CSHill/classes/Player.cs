using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Ionic.Zlib;
using System.IO;

public class Player
{
    public int NetId;
    public string Name;

    public Player(TcpClient socket)
    {
        Socket = socket;
        _NetId++;
        NetId = _NetId;
    }

    //NETWORKING STUFF
    public static int _NetId = 0; //global netid for all players
    public TcpClient Socket;

    private byte[] CurrentBytes = { };
    public void HandleBytes(byte[] newBytes)
    {
        CurrentBytes = CurrentBytes.Concat(newBytes).ToArray();//add new bytes
        var (messageSize, end) = PacketBuilder.ReadUIntV(CurrentBytes);//get uintv stuff

        Console.WriteLine("messageSize: {0} | end: {1} | netId: {2} | curlen: {3}", messageSize, end, NetId, CurrentBytes.Length);//debug stuff
        if (messageSize + end > CurrentBytes.Length) //return if there arent enough bytes
        {
            foreach (var data in CurrentBytes)
                Console.Write(data.ToString() + " ");
            Console.WriteLine();
            Console.WriteLine("--------------------End---------Fail--------------------");
            return;
        }

        byte[] workBytes = new byte[messageSize]; //make new array with the size to fit the bytes we need
        Array.Copy(CurrentBytes, end, workBytes, 0, messageSize + end - 1); //add the bytes we need
        CurrentBytes = CurrentBytes.Skip(messageSize + end).ToArray(); //remove the bytes we just took from the current bytes array

        if (workBytes[0] == 120)//check for compression
        {
            byte[] decompressedWorkBytes = ZlibStream.UncompressBuffer(workBytes);
            foreach (var data in decompressedWorkBytes)
                Console.Write(data.ToString() + " ");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Not compressed!");
            foreach (var data in workBytes)
                Console.Write(data.ToString() + " ");
        }
        Console.WriteLine();
        Console.WriteLine("--------------------End------Success-----------------------");
        Console.WriteLine();
    }

}
