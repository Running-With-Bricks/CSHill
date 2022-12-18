using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Brick
{
    public static uint _NetId = 0;
    public uint NetId;

    public string Name;
    public Vector3 Position;
    public Vector3 Scale;
    public float Rotation;
    public Color Color;

    public string Shape;
    public int Model;
    public bool Collision = true;
    public double Visibility;

    public bool lightEnabled = false;
    public int lightRange;
    public Color lightColor;


    public Brick( Vector3 position, Vector3 scale, Color color)
    {
        _NetId++;
        Position = position;
        Scale = scale;
        Color = color;
        NetId = _NetId;
    }
}