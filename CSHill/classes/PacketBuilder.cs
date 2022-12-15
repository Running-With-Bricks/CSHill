using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

public class PacketBuilder
{
    public static (int,int) ReadUIntV(byte[] buffer)
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
            return ((int)((BitConverter.ToUInt32(buffer, 0) / 8) + 0x204080),4);
        }
    }
}
