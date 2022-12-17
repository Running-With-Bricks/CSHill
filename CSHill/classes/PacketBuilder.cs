using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net.Sockets;

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
    public static byte[] WriteUIntV(byte[] buffer)
    {
        var length = buffer.Length;
        // 1 Byte
        if (length < 0x80)
        {
            PacketBuilder size = new(0);
            size.u8((uint)((length << 1) + 1));
            return size.Data.Concat(buffer).ToArray();
            // 2 Bytes
        }
        else if (length < 0x4080)
        {
            //const size = Buffer.alloc(2);
            //size.writeUInt16LE(((length - 0x80) << 2) + 2, 0);
            //return Buffer.concat([size, buffer]);
            PacketBuilder size = new(0);
            size.u16((ushort)(((length - 0x80) << 2) + 2));
            return size.Data.Concat(buffer).ToArray();
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
            return size.Data.Concat(buffer).ToArray();
            // 4 Bytes
        }
        else
        {
            //const size = Buffer.alloc(4);
            //size.writeUInt32LE((length - 0x204080) * 8, 0);
            //return Buffer.concat([size, buffer]);
            PacketBuilder size = new(0);
            size.u32((uint)((length - 0x204080) * 8));
            return size.Data.Concat(buffer).ToArray();
        }
    }
}
public class PacketBuilder
{
    public byte[] Data;
    public PacketBuilder(int id)
    {
        if (id != 0)
        {
            Data = new byte[1];
            Data[0] = (byte)id;
        }
        else
        {
            Data = new byte[0];
        }
    }
    public PacketBuilder u8(uint bite)
    {
        Data = Data.Append((byte)bite).ToArray();
        return this;
    }
    public PacketBuilder u16(UInt16 bite)
    {
        Array.Resize(ref Data, Data.Length + 2);
        BitConverter.GetBytes(bite).CopyTo(Data, Data.Length - 2);
        return this;
    }
    public PacketBuilder u32(UInt32 bite)
    {
        Array.Resize(ref Data, Data.Length + 4);
        BitConverter.GetBytes(bite).CopyTo(Data, Data.Length - 4);
        return this;
    }
    public PacketBuilder String(string bite)
    {
        Array.Resize(ref Data, Data.Length + bite.Length);
        Encoding.ASCII.GetBytes(bite).CopyTo(Data, Data.Length - bite.Length);
        this.u8(0);
        return this;
    }
    public PacketBuilder Float(float bite)
    {
        Array.Resize(ref Data, Data.Length + 4);
        BitConverter.GetBytes(bite).CopyTo(Data, Data.Length - 4);
        return this;
    }

    public PacketBuilder send(TcpClient Socket)
    {
        Data = UIntV.WriteUIntV(Data);
        NetworkStream ns = Socket.GetStream();
        ns.Write(Data, 0, Data.Length);

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
