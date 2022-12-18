using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net.Sockets;
using Ionic.Zlib;

public class UIntV
{
    public static (int, int) ReadUIntV(byte[] buffer)
    {
        if ((buffer[0] & 1) != 0)
        {
            return (buffer[0] >> 1, 1);
        }
        else if ((buffer[0] & 2) != 0)
        {
            return (BitConverter.ToUInt16(buffer, 0) >> 2, 2);
        }
        else if ((buffer[0] & 4) != 0)
        {
            return ((buffer[2] << 13) + (buffer[1] << 5) + (buffer[0] >> 3) + 0x4080, 3);
        }
        else
        {
            return ((int)((BitConverter.ToUInt32(buffer, 0) / 8) + 0x204080), 4);
        }
    }
    public static List<byte> WriteUIntV(List<byte> buffer)
    {
        var length = buffer.Count;
        // 1 Byte
        if (length < 0x80)
        {
            PacketBuilder size = new(0);
            size.u8((uint)((length << 1) + 1));
            return size.Data.Concat(buffer).ToList();
            // 2 Bytes
        }
        else if (length < 0x4080)
        {
            //const size = Buffer.alloc(2);
            //size.writeUInt16LE(((length - 0x80) << 2) + 2, 0);
            //return Buffer.concat([size, buffer]);
            PacketBuilder size = new(0);
            size.u16((ushort)(((length - 0x80) << 2) + 2));
            return size.Data.Concat(buffer).ToList();
            // 3 Bytes
        }
        else if (length < 0x204080)
        {
            //const size = Buffer.alloc(3);
            //const writeValue = ((length - 0x4080) << 3) + 4;
            //size.writeUInt8((writeValue & 0xFF), 0);
            //size.writeUInt16LE(writeValue >> 8, 1);
            //return Buffer.concat([size, buffer]);
            PacketBuilder size = new(0);
            var writeValue = ((length - 0x4080) << 3) + 4;
            size.u8((uint)(writeValue & 0xFF));
            size.u16((ushort)(writeValue >> 8));
            return size.Data.Concat(buffer).ToList();
            // 4 Bytes
        }
        else
        {
            //const size = Buffer.alloc(4);
            //size.writeUInt32LE((length - 0x204080) * 8, 0);
            //return Buffer.concat([size, buffer]);
            PacketBuilder size = new(0);
            size.u32((uint)((length - 0x204080) * 8));
            return size.Data.Concat(buffer).ToList();
        }
    }
}
public class PacketBuilder
{
    public List<byte> Data;
    public PacketBuilder(int id)
    {
        if (id != 0)
        {
            Data = new List<byte>();
            Data.Add((byte)id);
        }
        else
        {
            Data = new List<byte>();
        }
    }
    public PacketBuilder u8(uint bite)
    {
        Data.Add((byte)bite);
        return this;
    }
    public PacketBuilder u16(UInt16 bite)
    {
        Data.AddRange(BitConverter.GetBytes(bite));
        return this;
    }
    public PacketBuilder u32(UInt32 bite)
    {
        Data.AddRange(BitConverter.GetBytes(bite));
        return this;
    }
    public PacketBuilder i32(Int32 bite)
    {
        Data.AddRange(BitConverter.GetBytes(bite));
        return this;
    }
    public PacketBuilder String(string bite)
    {
        Data.AddRange(Encoding.ASCII.GetBytes(bite));
        this.u8(0);
        return this;
    }
    public PacketBuilder Float(float bite)
    {
        Data.AddRange(BitConverter.GetBytes(bite));
        return this;
    }
    public PacketBuilder Bool(bool bite)
    {
        var bit = bite ? 1 : 0;
        Data.Add((byte)bit);
        return this;
    }
    public PacketBuilder send(string IpPort)
    {
        Data = UIntV.WriteUIntV(Data);
        Server.server.Send(IpPort, Data.ToArray());
        return this;
    }
    public PacketBuilder broadcast()
    {
        Data = UIntV.WriteUIntV(Data);

        foreach (var IpPort in Server.server.GetClients())
        {
            Server.server.Send(IpPort, Data.ToArray());
        }

        return this;
    }
    public PacketBuilder deflate()
    {
        Data = ZlibStream.CompressBuffer(Data.ToArray()).ToList();
        return this;
    }
}
public class PacketHandler
{
    public PacketHandler(byte[] _bytes)
    {
        Data = _bytes;
        CurrentIndex = 0;
    }
    public byte[] Data;
    public int CurrentIndex;

    public uint u8()
    {
        CurrentIndex++;
        return Convert.ToUInt32(Data[CurrentIndex - 1]);
    }
    public uint u16()
    {
        var output = BitConverter.ToUInt16(Data, CurrentIndex);
        CurrentIndex += 2;
        return output;
    }
    public uint u32()
    {
        var output = BitConverter.ToUInt32(Data, CurrentIndex);
        CurrentIndex += 4;
        return output;
    }
    public float Float()
    {
        var output = BitConverter.ToSingle(Data, CurrentIndex);
        CurrentIndex += 4;
        return output;
    }
    public string String()
    {
        int stringEnd = Array.FindIndex(Data, CurrentIndex, bite => bite == 0);
        string output = Encoding.UTF8.GetString(Data, CurrentIndex, stringEnd - CurrentIndex);
        CurrentIndex = stringEnd + 1;
        return output;
    }
}
