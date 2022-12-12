using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Brick
{

    public string Name;
    public Vector3 Position;
    public Vector3 Scale;
    public float Rotation;
    public string Color;

    public Brick( Vector3 position, Vector3 scale, string color)
    {
        Position = position;
        Scale = scale;
        Color = color;
    }
}