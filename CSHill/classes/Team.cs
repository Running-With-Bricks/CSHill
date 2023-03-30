using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Team
{
    public static uint _NetId = 0;

    public uint NetId;
    public string Name;
    public Color Color;

    public Team(string name, Color color)
    {
        _NetId++;
        Name = name;
        Color = color;
        NetId = _NetId;
    }

    public List<Player> Players()
    {
        return Game.Players.FindAll(p => p.Team == this);
    }
}