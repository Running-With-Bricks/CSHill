using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;

public class Player
{
    public static int _NetId = 0;

    public TcpClient Socket;
    public int NetId;
    public string Name;


    public Player(TcpClient socket)
    {
        Socket = socket;
        _NetId++;
        NetId = _NetId;
    }
}
